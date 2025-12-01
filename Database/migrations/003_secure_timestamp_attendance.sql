-- ============================================
-- Migration 003: Secure Timestamp Attendance System
-- Creates stored procedure for Time-In/Time-Out logic
-- with validation mode tracking for Solution 1
-- ============================================
-- Date: December 1, 2025
-- ============================================

USE student_attendance_db;

-- Drop existing procedure if it exists
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
    DECLARE v_current_validation_mode VARCHAR(20);
    DECLARE v_status VARCHAR(20);
    DECLARE v_notes TEXT;
    
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
    SELECT scan_id, scan_datetime, time_out, time_in_validation_mode
    INTO v_existing_scan_id, v_existing_time_in, v_existing_time_out, v_existing_validation_mode
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
        -- ===================================================
        -- SOLUTION 1: Check for validation mode mismatch
        -- ===================================================
        SET v_notes = '';
        SET v_status = 'success';
        
        -- Critical check: Online → Offline mismatch
        IF v_existing_validation_mode = 'online' AND v_current_validation_mode = 'offline' THEN
            SET v_status = 'for_review';
            SET v_notes = '🚨 CRITICAL: Time-in was ONLINE but time-out is OFFLINE - Possible WiFi disconnect + time tampering';
        ELSEIF v_current_validation_mode = 'offline' THEN
            SET v_status = 'for_review';
            SET v_notes = 'Time-out recorded in offline mode - Requires review';
        END IF;
        
        -- Check for very short duration (< 10 minutes)
        IF TIMESTAMPDIFF(MINUTE, v_existing_time_in, NOW()) < 10 THEN
            SET v_status = 'for_review';
            IF v_notes != '' THEN
                SET v_notes = CONCAT(v_notes, '\n');
            END IF;
            SET v_notes = CONCAT(v_notes, '🟠 WARNING: Very short duration (', 
                TIMESTAMPDIFF(MINUTE, v_existing_time_in, NOW()), ' minutes)');
        END IF;
        
        -- Update existing record with TIME-OUT
        UPDATE scan_history
        SET time_out = NOW(),
            time_out_validation_mode = v_current_validation_mode,
            status = v_status,
            notes = CASE 
                WHEN notes IS NULL OR notes = '' THEN v_notes
                ELSE CONCAT(notes, '\n', v_notes)
            END,
            requires_review = CASE 
                WHEN v_status = 'for_review' THEN TRUE
                ELSE requires_review
            END,
            validation_status = p_validation_status,
            client_time = p_client_time,
            server_time = p_server_time,
            time_drift_seconds = p_time_drift_seconds
        WHERE scan_id = v_existing_scan_id;
        
        -- Set output parameters
        SET p_result = CASE
            WHEN v_status = 'for_review' THEN 'SUCCESS: Time-Out recorded - FLAGGED FOR REVIEW'
            ELSE 'SUCCESS: Time-Out recorded'
        END;
        SET p_student_name = v_student_name;
        SET p_student_number = v_student_number;
        SET p_scan_type = 'TIME_OUT';
        SET p_timestamp = NOW();
        SET p_time_in = v_existing_time_in;
        SET p_time_out = NOW();
        
    ELSE
        -- This is a TIME-IN scan
        -- ===================================================
        -- Check for duplicate Time-In today
        -- ===================================================
        IF EXISTS (
            SELECT 1 FROM scan_history
            WHERE student_id = v_student_id
              AND DATE(scan_datetime) = CURDATE()
              AND time_out IS NOT NULL
        ) THEN
            SET p_result = 'DUPLICATE: Student already has a completed attendance today';
            SET p_student_name = v_student_name;
            SET p_student_number = v_student_number;
            SET p_scan_type = 'DUPLICATE';
            SET p_timestamp = NOW();
            SET p_time_in = NULL;
            SET p_time_out = NULL;
            LEAVE sp_label;
        END IF;
        
        -- Determine status and notes for Time-In
        SET v_status = CASE
            WHEN v_current_validation_mode = 'offline' THEN 'for_review'
            WHEN p_requires_review = TRUE THEN 'for_review'
            ELSE 'success'
        END;
        
        SET v_notes = CASE
            WHEN v_current_validation_mode = 'offline' THEN 'Time-in recorded in offline mode - Timestamp cannot be verified'
            ELSE NULL
        END;
        
        -- Insert new TIME-IN record
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
            time_in_validation_mode,
            time_out_validation_mode,
            requires_review,
            client_time,
            server_time,
            time_drift_seconds,
            created_at
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
            v_notes,
            p_validation_status,
            v_current_validation_mode,  -- Set Time-In validation mode
            NULL,                         -- Time-Out validation mode is NULL initially
            p_requires_review,
            p_client_time,
            p_server_time,
            p_time_drift_seconds,
            NOW()
        );
        
        -- Set output parameters
        SET p_result = CASE
            WHEN v_status = 'for_review' THEN 'SUCCESS: Time-In recorded - FLAGGED FOR REVIEW'
            ELSE 'SUCCESS: Time-In recorded'
        END;
        SET p_student_name = v_student_name;
        SET p_student_number = v_student_number;
        SET p_scan_type = 'TIME_IN';
        SET p_timestamp = NOW();
        SET p_time_in = NOW();
        SET p_time_out = NULL;
    END IF;
    
END //

DELIMITER ;

-- ============================================
-- Verify procedure was created
-- ============================================
SELECT 
    'Migration 003 completed: Stored procedure sp_record_attendance_scan_secure created' AS Status,
    'Procedure now tracks time_in_validation_mode and time_out_validation_mode' AS Feature,
    'Solution 1: WiFi disconnect + time tampering detection enabled' AS Security;

