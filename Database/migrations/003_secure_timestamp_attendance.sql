-- ============================================
-- Migration: Secure Timestamp Attendance System
-- Version: 003
-- Date: 2025-11-28
-- Description: Enhanced stored procedure that returns database-generated timestamps
--              to prevent client-side time manipulation
-- ============================================

USE student_attendance_db;

DELIMITER //

-- Drop existing procedure if exists
DROP PROCEDURE IF EXISTS sp_record_attendance_scan_secure //

-- Create secure attendance procedure with timestamp output
CREATE PROCEDURE sp_record_attendance_scan_secure(
    IN p_scan_data TEXT,
    IN p_device_id INT,
    IN p_location VARCHAR(100),
    OUT p_result VARCHAR(200),
    OUT p_student_name VARCHAR(200),
    OUT p_student_number VARCHAR(50),
    OUT p_scan_type VARCHAR(20),
    OUT p_timestamp DATETIME,
    OUT p_time_in DATETIME,
    OUT p_time_out DATETIME
)
BEGIN
    DECLARE v_student_id INT;
    DECLARE v_student_first VARCHAR(50);
    DECLARE v_student_last VARCHAR(50);
    DECLARE v_student_num VARCHAR(50);
    DECLARE v_student_status VARCHAR(20);
    DECLARE v_today_start DATETIME;
    DECLARE v_today_end DATETIME;
    DECLARE v_existing_scan_id INT;
    DECLARE v_existing_time_in DATETIME;
    DECLARE v_existing_time_out DATETIME;
    DECLARE v_recent_scan DATETIME;
    DECLARE v_current_timestamp DATETIME;
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        SET p_result = 'ERROR: Database error occurred';
        SET p_student_name = NULL;
        SET p_student_number = NULL;
        SET p_scan_type = 'ERROR';
        SET p_timestamp = NULL;
        SET p_time_in = NULL;
        SET p_time_out = NULL;
        ROLLBACK;
    END;
    
    START TRANSACTION;
    
    -- ===================================================
    -- CRITICAL: All timestamps use NOW() - database time
    -- Client time is NEVER trusted or used
    -- ===================================================
    
    -- Capture current database server time
    SET v_current_timestamp = NOW();
    
    -- Set today's date range using database time
    SET v_today_start = DATE(v_current_timestamp);
    SET v_today_end = DATE_ADD(v_today_start, INTERVAL 1 DAY);
    
    -- Find student by QR code data
    SELECT student_id, first_name, last_name, student_number, status
    INTO v_student_id, v_student_first, v_student_last, v_student_num, v_student_status
    FROM students
    WHERE qr_code_data = p_scan_data
    LIMIT 1;
    
    -- Check if student exists
    IF v_student_id IS NULL THEN
        SET p_result = 'ERROR: Student not found';
        SET p_student_name = NULL;
        SET p_student_number = NULL;
        SET p_scan_type = 'ERROR';
        SET p_timestamp = v_current_timestamp;
        SET p_time_in = NULL;
        SET p_time_out = NULL;
        ROLLBACK;
        
    -- Check if student is active
    ELSEIF v_student_status != 'Active' THEN
        SET p_result = CONCAT('ERROR: Student is ', v_student_status);
        SET p_student_name = CONCAT(v_student_first, ' ', v_student_last);
        SET p_student_number = v_student_num;
        SET p_scan_type = 'ERROR';
        SET p_timestamp = v_current_timestamp;
        SET p_time_in = NULL;
        SET p_time_out = NULL;
        ROLLBACK;
        
    ELSE
        SET p_student_name = CONCAT(v_student_first, ' ', v_student_last);
        SET p_student_number = v_student_num;
        
        -- Check for duplicate scan within last 10 seconds using database time
        SELECT scan_datetime INTO v_recent_scan
        FROM scan_history
        WHERE student_id = v_student_id
          AND scan_datetime > DATE_SUB(v_current_timestamp, INTERVAL 10 SECOND)
        ORDER BY scan_datetime DESC
        LIMIT 1;
        
        IF v_recent_scan IS NOT NULL THEN
            SET p_result = 'ERROR: Please wait before scanning again (10 second cooldown)';
            SET p_scan_type = 'DUPLICATE';
            SET p_timestamp = v_current_timestamp;
            SET p_time_in = v_recent_scan;
            SET p_time_out = NULL;
            ROLLBACK;
            
        ELSE
            -- Check for existing Time In/Time Out for today
            SELECT scan_id, scan_datetime, time_out
            INTO v_existing_scan_id, v_existing_time_in, v_existing_time_out
            FROM scan_history
            WHERE student_id = v_student_id
              AND scan_datetime >= v_today_start
              AND scan_datetime < v_today_end
              AND status = 'success'
            ORDER BY scan_datetime DESC
            LIMIT 1;
            
            -- ===================================================
            -- CASE 1: No record for today - Record TIME IN
            -- Timestamp is ONLY from database NOW()
            -- ===================================================
            IF v_existing_scan_id IS NULL THEN
                INSERT INTO scan_history (
                    student_id, device_id, scan_type, scan_data, 
                    scan_datetime, time_out, location, status, scan_purpose
                ) VALUES (
                    v_student_id, p_device_id, 'QR', p_scan_data,
                    v_current_timestamp, NULL, p_location, 'success', 'attendance'
                );
                
                SET p_result = CONCAT('SUCCESS: Time In recorded at ', DATE_FORMAT(v_current_timestamp, '%h:%i %p'));
                SET p_scan_type = 'TIME_IN';
                SET p_timestamp = v_current_timestamp;
                SET p_time_in = v_current_timestamp;
                SET p_time_out = NULL;
                COMMIT;
                
            -- ===================================================
            -- CASE 2: Time In exists, but no Time Out - Record TIME OUT
            -- Timestamp is ONLY from database NOW()
            -- ===================================================
            ELSEIF v_existing_time_out IS NULL THEN
                UPDATE scan_history
                SET time_out = v_current_timestamp
                WHERE scan_id = v_existing_scan_id;
                
                SET p_result = CONCAT('SUCCESS: Time Out recorded at ', DATE_FORMAT(v_current_timestamp, '%h:%i %p'));
                SET p_scan_type = 'TIME_OUT';
                SET p_timestamp = v_current_timestamp;
                SET p_time_in = v_existing_time_in;
                SET p_time_out = v_current_timestamp;
                COMMIT;
                
            -- ===================================================
            -- CASE 3: Both Time In and Time Out exist - REJECT
            -- ===================================================
            ELSE
                SET p_result = CONCAT('ERROR: Attendance already completed for today (Time In: ', 
                                     DATE_FORMAT(v_existing_time_in, '%h:%i %p'), 
                                     ', Time Out: ', 
                                     DATE_FORMAT(v_existing_time_out, '%h:%i %p'), ')');
                SET p_scan_type = 'COMPLETED';
                SET p_timestamp = v_current_timestamp;
                SET p_time_in = v_existing_time_in;
                SET p_time_out = v_existing_time_out;
                ROLLBACK;
            END IF;
        END IF;
    END IF;
END //

DELIMITER ;

-- ============================================
-- Verification Query
-- ============================================
-- Run this to verify the procedure was created successfully:
-- SHOW PROCEDURE STATUS WHERE Db = 'student_attendance_db' AND Name = 'sp_record_attendance_scan_secure';
