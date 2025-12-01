-- ============================================
-- Migration 011: Add Duration Validation to Stored Procedure
-- Blocks Time-Out if duration < 15 minutes or > 18 hours
-- ============================================
-- Date: December 1, 2025
-- Purpose: Enforce minimum 15-minute attendance duration
-- ============================================

USE student_attendance_db;

-- Drop existing procedure
DROP PROCEDURE IF EXISTS sp_record_attendance_scan_secure;

DELIMITER //

CREATE PROCEDURE sp_record_attendance_scan_secure(
    IN p_scan_data TEXT,
    IN p_device_id INT,
    IN p_location VARCHAR(100),
    IN p_validation_status VARCHAR(30),
    IN p_requires_review BOOLEAN,
    IN p_client_time DATETIME,
    IN p_server_time DATETIME,
    IN p_time_drift_seconds INT,
    IN p_tick_count BIGINT,
    IN p_tick_frequency BIGINT,
    IN p_connection_drop_count INT,
    OUT p_result VARCHAR(200),
    OUT p_student_name VARCHAR(200),
    OUT p_student_number VARCHAR(50),
    OUT p_scan_type VARCHAR(20),
    OUT p_timestamp DATETIME,
    OUT p_time_in DATETIME,
    OUT p_time_out DATETIME
)
sp_label: BEGIN
    DECLARE v_student_id INT;
    DECLARE v_student_number VARCHAR(50);
    DECLARE v_student_name VARCHAR(200);
    DECLARE v_existing_scan_id INT;
    DECLARE v_existing_time_in DATETIME;
    DECLARE v_existing_time_out DATETIME;
    DECLARE v_existing_validation_mode VARCHAR(20);
    DECLARE v_existing_time_in_tick BIGINT;
    DECLARE v_current_validation_mode VARCHAR(20);
    DECLARE v_status VARCHAR(20);
    DECLARE v_notes TEXT;
    DECLARE v_offline_minutes DOUBLE;
    DECLARE v_duration_minutes DOUBLE;
    DECLARE v_duration_hours DOUBLE;
    
    -- ===================================================
    -- STEP 1: Determine validation mode based on internet connection
    -- ===================================================
    SET v_current_validation_mode = CASE 
        WHEN p_validation_status = 'offline_mode' THEN 'offline'
        WHEN p_validation_status = 'verified' THEN 'online'
        ELSE 'unknown'
    END;
    
    -- ===================================================
    -- STEP 2: Extract student number from QR data
    -- ===================================================
    SET v_student_number = TRIM(SUBSTRING_INDEX(SUBSTRING_INDEX(p_scan_data, 'ID:', -1), '|', 1));
    
    -- Validate student exists
    SELECT student_id, CONCAT(first_name, ' ', last_name)
    INTO v_student_id, v_student_name
    FROM students
    WHERE student_number = v_student_number AND status = 'Active'
    LIMIT 1;
    
    IF v_student_id IS NULL THEN
        SET p_result = 'ERROR: Student not found or inactive';
        SET p_student_name = NULL;
        SET p_student_number = v_student_number;
        SET p_scan_type = 'ERROR';
        SET p_timestamp = NULL;
        SET p_time_in = NULL;
        SET p_time_out = NULL;
        LEAVE sp_label;
    END IF;
    
    -- ===================================================
    -- STEP 3: Check for existing Time-In today (without Time-Out)
    -- ===================================================
    SELECT scan_id, scan_datetime, time_out, time_in_validation_mode, time_in_tick_count
    INTO v_existing_scan_id, v_existing_time_in, v_existing_time_out, v_existing_validation_mode, v_existing_time_in_tick
    FROM scan_history
    WHERE student_id = v_student_id
      AND DATE(scan_datetime) = CURDATE()
      AND time_out IS NULL
    ORDER BY scan_datetime DESC
    LIMIT 1;
    
    -- ===================================================
    -- STEP 4: Determine if this is TIME-IN or TIME-OUT
    -- ===================================================
    IF v_existing_scan_id IS NOT NULL AND v_existing_time_out IS NULL THEN
        -- ====================================================================
        -- TIME-OUT SCAN - VALIDATE DURATION BEFORE ACCEPTING
        -- ====================================================================
        
        -- Calculate duration in minutes
        SET v_duration_minutes = TIMESTAMPDIFF(MINUTE, v_existing_time_in, NOW());
        SET v_duration_hours = v_duration_minutes / 60.0;
        
        -- VALIDATION 1: Minimum Duration (15 minutes)
        IF v_duration_minutes < 15 THEN
            SET p_result = CONCAT('ERROR: Too fast! Only ', FLOOR(v_duration_minutes), ' min. Need 15 min minimum.');
            SET p_student_name = v_student_name;
            SET p_student_number = v_student_number;
            SET p_scan_type = 'TIME_OUT_BLOCKED';
            SET p_timestamp = NOW();
            SET p_time_in = v_existing_time_in;
            SET p_time_out = NULL;
            LEAVE sp_label;
        END IF;
        
        -- VALIDATION 2: Maximum Duration (18 hours)
        IF v_duration_hours > 18 THEN
            SET p_result = CONCAT('ERROR: Duration too long (', ROUND(v_duration_hours, 1), 'h). Max 18h.');
            SET p_student_name = v_student_name;
            SET p_student_number = v_student_number;
            SET p_scan_type = 'TIME_OUT_BLOCKED';
            SET p_timestamp = NOW();
            SET p_time_in = v_existing_time_in;
            SET p_time_out = NULL;
            LEAVE sp_label;
        END IF;
        
        -- Duration validation passed - proceed with Time-Out
        SET v_notes = '';
        SET v_status = 'completed';
        
        -- Check if mode changed (online -> offline = suspicious)
        IF v_existing_validation_mode = 'online' AND v_current_validation_mode = 'offline' THEN
            SET v_notes = CONCAT(v_notes, 'WARNING: Time-in was ONLINE but time-out is OFFLINE. ');
            SET v_status = 'for_review';
        END IF;
        
        -- Calculate offline duration if available
        IF v_existing_time_in_tick IS NOT NULL AND p_tick_count IS NOT NULL AND p_tick_frequency > 0 THEN
            SET v_offline_minutes = ((p_tick_count - v_existing_time_in_tick) / p_tick_frequency) / 60.0;
        ELSE
            SET v_offline_minutes = NULL;
        END IF;
        
        -- Update existing record with Time-Out
        UPDATE scan_history
        SET time_out = NOW(),
            time_out_validation_mode = v_current_validation_mode,
            time_out_tick_count = p_tick_count,
            connection_drop_count = COALESCE(connection_drop_count, 0) + p_connection_drop_count,
            offline_duration_minutes = v_offline_minutes,
            status = v_status,
            notes = CONCAT(COALESCE(notes, ''), v_notes)
        WHERE scan_id = v_existing_scan_id;
        
        SET p_result = CONCAT('SUCCESS: Time-Out recorded (Duration: ', 
                             FLOOR(v_duration_minutes / 60), 'h ', 
                             MOD(FLOOR(v_duration_minutes), 60), 'm)');
        SET p_student_name = v_student_name;
        SET p_student_number = v_student_number;
        SET p_scan_type = 'TIME_OUT';
        SET p_timestamp = NOW();
        SET p_time_in = v_existing_time_in;
        SET p_time_out = NOW();
        
    ELSE
        -- ====================================================================
        -- TIME-IN SCAN
        -- ====================================================================
        
        -- Set status based on validation mode
        SET v_status = CASE
            WHEN v_current_validation_mode = 'offline' THEN 'for_review'
            ELSE 'pending'
        END;
        
        -- Insert new Time-In record
        INSERT INTO scan_history (
            student_id,
            device_id,
            scan_type,
            scan_data,
            scan_datetime,
            time_out,
            scan_purpose,
            location,
            status,
            notes,
            validation_status,
            requires_review,
            client_time,
            server_time,
            time_drift_seconds,
            time_in_validation_mode,
            time_in_tick_count
        ) VALUES (
            v_student_id,
            p_device_id,
            'QR',
            p_scan_data,
            NOW(),
            NULL,
            'attendance',
            p_location,
            v_status,
            CASE WHEN v_current_validation_mode = 'offline' THEN 'Offline mode - flagged for review' ELSE NULL END,
            p_validation_status,
            p_requires_review,
            p_client_time,
            p_server_time,
            p_time_drift_seconds,
            v_current_validation_mode,
            p_tick_count
        );
        
        SET p_result = 'SUCCESS: Time-In recorded';
        SET p_student_name = v_student_name;
        SET p_student_number = v_student_number;
        SET p_scan_type = 'TIME_IN';
        SET p_timestamp = NOW();
        SET p_time_in = NOW();
        SET p_time_out = NULL;
    END IF;
    
END //

DELIMITER ;

-- Verify migration
SELECT 
    'Migration 011 completed: Duration validation added to stored procedure' AS Status,
    'Minimum duration: 15 minutes' AS Min_Duration,
    'Maximum duration: 18 hours' AS Max_Duration;
