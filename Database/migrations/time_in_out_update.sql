-- ============================================
-- Time In/Time Out Attendance System Update
-- ============================================

USE student_attendance_db;

-- Add time_out column to scan_history table (check if exists first)
SET @col_exists = 0;
SELECT COUNT(*) INTO @col_exists 
FROM information_schema.COLUMNS 
WHERE TABLE_SCHEMA = 'student_attendance_db' 
  AND TABLE_NAME = 'scan_history' 
  AND COLUMN_NAME = 'time_out';

SET @sql = IF(@col_exists = 0, 
    'ALTER TABLE scan_history ADD COLUMN time_out DATETIME NULL AFTER scan_datetime;',
    'SELECT "Column time_out already exists" as Info;');
    
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- Drop old procedure if exists
DROP PROCEDURE IF EXISTS sp_record_attendance_scan;

DELIMITER //

-- New Procedure: Record Attendance with Time In/Time Out Logic
CREATE PROCEDURE sp_record_attendance_scan(
    IN p_scan_data TEXT,
    IN p_device_id INT,
    IN p_location VARCHAR(100),
    OUT p_result VARCHAR(200),
    OUT p_student_name VARCHAR(200),
    OUT p_student_number VARCHAR(50),
    OUT p_scan_type VARCHAR(20)
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
    DECLARE v_time_in DATETIME;
    DECLARE v_time_out DATETIME;
    DECLARE v_recent_scan DATETIME;
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        SET p_result = 'ERROR: Database error occurred';
        SET p_student_name = NULL;
        SET p_student_number = NULL;
        SET p_scan_type = 'ERROR';
        ROLLBACK;
    END;
    
    START TRANSACTION;
    
    -- Set today's date range
    SET v_today_start = DATE(NOW());
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
        ROLLBACK;
    -- Check if student is active
    ELSEIF v_student_status != 'Active' THEN
        SET p_result = CONCAT('ERROR: Student is ', v_student_status);
        SET p_student_name = CONCAT(v_student_first, ' ', v_student_last);
        SET p_student_number = v_student_num;
        SET p_scan_type = 'ERROR';
        ROLLBACK;
    ELSE
        SET p_student_name = CONCAT(v_student_first, ' ', v_student_last);
        SET p_student_number = v_student_num;
        
        -- Check for duplicate scan within last 10 seconds (prevent double-scanning)
        SELECT scan_datetime INTO v_recent_scan
        FROM scan_history
        WHERE student_id = v_student_id
          AND scan_datetime > DATE_SUB(NOW(), INTERVAL 10 SECOND)
        ORDER BY scan_datetime DESC
        LIMIT 1;
        
        IF v_recent_scan IS NOT NULL THEN
            SET p_result = 'ERROR: Please wait before scanning again (10 second cooldown)';
            SET p_scan_type = 'DUPLICATE';
            ROLLBACK;
        ELSE
            -- Check for existing Time In/Time Out for today
            SELECT scan_id, scan_datetime, time_out
            INTO v_existing_scan_id, v_time_in, v_time_out
            FROM scan_history
            WHERE student_id = v_student_id
              AND scan_datetime >= v_today_start
              AND scan_datetime < v_today_end
              AND status = 'success'
            ORDER BY scan_datetime DESC
            LIMIT 1;
            
            -- CASE 1: No record for today - Record TIME IN
            IF v_existing_scan_id IS NULL THEN
                INSERT INTO scan_history (
                    student_id, device_id, scan_type, scan_data, 
                    scan_datetime, time_out, location, status, scan_purpose
                ) VALUES (
                    v_student_id, p_device_id, 'QR', p_scan_data,
                    NOW(), NULL, p_location, 'success', 'attendance'
                );
                
                SET p_result = CONCAT('SUCCESS: Time In recorded at ', DATE_FORMAT(NOW(), '%h:%i %p'));
                SET p_scan_type = 'TIME_IN';
                COMMIT;
                
            -- CASE 2: Time In exists, but no Time Out - Record TIME OUT
            ELSEIF v_time_out IS NULL THEN
                UPDATE scan_history
                SET time_out = NOW()
                WHERE scan_id = v_existing_scan_id;
                
                SET p_result = CONCAT('SUCCESS: Time Out recorded at ', DATE_FORMAT(NOW(), '%h:%i %p'));
                SET p_scan_type = 'TIME_OUT';
                COMMIT;
                
            -- CASE 3: Both Time In and Time Out exist - REJECT
            ELSE
                SET p_result = CONCAT('ERROR: Attendance already completed for today (Time In: ', 
                                     DATE_FORMAT(v_time_in, '%h:%i %p'), 
                                     ', Time Out: ', 
                                     DATE_FORMAT(v_time_out, '%h:%i %p'), ')');
                SET p_scan_type = 'COMPLETED';
                ROLLBACK;
            END IF;
        END IF;
    END IF;
END //

DELIMITER ;

-- Update the view to show both time in and time out
DROP VIEW IF EXISTS vw_recent_scans;

CREATE VIEW vw_recent_scans AS
SELECT 
    sh.scan_id,
    s.student_number,
    CONCAT(s.first_name, ' ', s.last_name) as student_name,
    sh.scan_type,
    sh.scan_datetime as time_in,
    sh.time_out,
    sh.location,
    sh.status,
    d.device_name,
    CASE 
        WHEN sh.time_out IS NOT NULL THEN 'completed'
        WHEN sh.time_out IS NULL AND sh.scan_datetime >= CURDATE() THEN 'pending_out'
        ELSE 'incomplete'
    END as attendance_status
FROM scan_history sh
INNER JOIN students s ON sh.student_id = s.student_id
INNER JOIN devices d ON sh.device_id = d.device_id
ORDER BY sh.scan_datetime DESC;

-- Test the new procedure
-- CALL sp_record_attendance_scan('STUDENT-2300401', 1, 'Pamantasan ng Cabuyao Building', @result, @name, @number, @type);
-- SELECT @result as Result, @name as StudentName, @number as StudentNumber, @type as ScanType;

SELECT 'Time In/Time Out system update completed successfully!' as Status;
