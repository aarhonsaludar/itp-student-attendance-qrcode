-- ============================================
-- Migration 008: Update Stored Procedure with TickCount Support
-- Updates sp_record_attendance_scan_secure to handle TickCount parameters
-- ============================================
-- Date: December 1, 2025
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
        -- This is a TIME-OUT scan
        SET v_notes = '';
        SET v_status = 'success';
        
        -- Calculate offline duration if TickCount available
        IF v_existing_time_in_tick IS NOT NULL AND p_tick_count IS NOT NULL THEN
            SET v_offline_minutes = (p_tick_count - v_existing_time_in_tick) / 60000.0;
        END IF;
        
        -- Check for validation mode mismatch (Online → Offline)
        IF v_existing_validation_mode = 'online' AND v_current_validation_mode = 'offline' THEN
            SET v_status = 'for_review';
            SET v_notes = CONCAT(v_notes, '⚠️ Validation Mode Mismatch: Time-In was ONLINE, Time-Out is OFFLINE. ');
        END IF;
        
        -- Check if requires review
        IF p_requires_review THEN
            SET v_status = 'for_review';
        END IF;
        
        -- Update existing record with TIME-OUT
        UPDATE scan_history
        SET time_out = NOW(),
            time_out_validation_mode = v_current_validation_mode,
            time_out_tick_count = p_tick_count,
            connection_drop_count = p_connection_drop_count,
            offline_duration_minutes = v_offline_minutes,
            status = v_status,
            notes = CONCAT(IFNULL(notes, ''), v_notes),
            requires_review = (v_status = 'for_review')
        WHERE scan_id = v_existing_scan_id;
        
        SET p_result = 'SUCCESS: Time Out recorded';
        SET p_student_name = v_student_name;
        SET p_student_number = v_student_number;
        SET p_scan_type = 'TIME_OUT';
        SET p_timestamp = NOW();
        SET p_time_in = v_existing_time_in;
        SET p_time_out = NOW();
        
    ELSE
        -- This is a TIME-IN scan
        -- Check for duplicate Time-In
        IF EXISTS (
            SELECT 1 FROM scan_history
            WHERE student_id = v_student_id
              AND DATE(scan_datetime) = CURDATE()
              AND time_out IS NOT NULL
        ) THEN
            SET v_status = 'duplicate';
            SET v_notes = '⚠️ Already have a completed attendance for today.';
        ELSE
            SET v_status = 'success';
            IF p_requires_review THEN
                SET v_status = 'for_review';
            END IF;
        END IF;
        
        -- Insert new TIME-IN record
        INSERT INTO scan_history (
            student_id,
            device_id,
            scan_type,
            scan_data,
            scan_datetime,
            scan_purpose,
            location,
            status,
            notes,
            validation_status,
            time_in_validation_mode,
            requires_review,
            client_time,
            server_time,
            time_drift_seconds,
            time_in_tick_count,
            connection_drop_count
        ) VALUES (
            v_student_id,
            p_device_id,
            'QR',
            p_scan_data,
            NOW(),
            'attendance',
            p_location,
            v_status,
            v_notes,
            p_validation_status,
            v_current_validation_mode,
            p_requires_review,
            p_client_time,
            p_server_time,
            p_time_drift_seconds,
            p_tick_count,
            p_connection_drop_count
        );
        
        SET p_result = 'SUCCESS: Time In recorded';
        SET p_student_name = v_student_name;
        SET p_student_number = v_student_number;
        SET p_scan_type = 'TIME_IN';
        SET p_timestamp = NOW();
        SET p_time_in = NOW();
        SET p_time_out = NULL;
    END IF;
    
END //

DELIMITER ;

SELECT 
    'Migration 008 completed: Stored procedure updated with TickCount support' AS Status,
    NOW() AS CompletedAt;
