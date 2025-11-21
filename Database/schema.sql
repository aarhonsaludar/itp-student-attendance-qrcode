-- ============================================
-- Student Attendance System Database Schema
-- MySQL Workbench ERD Compatible
-- For use with MySqlConnector in C#
-- ============================================

-- Create Database
CREATE DATABASE IF NOT EXISTS student_attendance_db
CHARACTER SET utf8mb4 
COLLATE utf8mb4_unicode_ci;

USE student_attendance_db;

-- ============================================
-- Table: users (System Users/Administrators)
-- ============================================
CREATE TABLE users (
    user_id INT AUTO_INCREMENT PRIMARY KEY,
    username VARCHAR(50) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    full_name VARCHAR(100) NOT NULL,
    email VARCHAR(100) UNIQUE,
    role ENUM('admin', 'staff', 'teacher') DEFAULT 'staff',
    is_active BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    last_login TIMESTAMP NULL,
    INDEX idx_username (username),
    INDEX idx_email (email),
    INDEX idx_active (is_active)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================
-- Table: students (Student Information)
-- ============================================
CREATE TABLE students (
    student_id INT AUTO_INCREMENT PRIMARY KEY,
    student_number VARCHAR(50) NOT NULL UNIQUE,
    first_name VARCHAR(50) NOT NULL,
    middle_name VARCHAR(50),
    last_name VARCHAR(50) NOT NULL,
    email VARCHAR(100) UNIQUE,
    phone VARCHAR(20),
    sex ENUM('Male', 'Female') DEFAULT NULL,
    year_level ENUM('1', '2', '3', '4', 'Graduate') NOT NULL,
    program VARCHAR(100) NOT NULL,
    section VARCHAR(50),
    qr_code_data TEXT NOT NULL,
    photo_path VARCHAR(255),
    status ENUM('Active', 'Inactive', 'Suspended') DEFAULT 'Active',
    enrollment_date DATE NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    INDEX idx_student_number (student_number),
    INDEX idx_qr_code (qr_code_data(255)),
    INDEX idx_name (last_name, first_name),
    INDEX idx_email (email),
    INDEX idx_status (status)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================
-- Table: devices (Scanning Devices)
-- ============================================
CREATE TABLE devices (
    device_id INT AUTO_INCREMENT PRIMARY KEY,
    device_name VARCHAR(100) NOT NULL,
    device_type ENUM('QR_SCANNER') DEFAULT 'QR_SCANNER' NOT NULL,
    location VARCHAR(100),
    status ENUM('active', 'inactive', 'maintenance') DEFAULT 'active',
    last_active TIMESTAMP NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    INDEX idx_device_type (device_type),
    INDEX idx_status (status)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================
-- Table: scan_history (QR Code Scan Records)
-- ============================================
CREATE TABLE scan_history (
    scan_id INT AUTO_INCREMENT PRIMARY KEY,
    student_id INT NOT NULL,
    device_id INT,
    scan_type ENUM('QR', 'MANUAL') DEFAULT 'QR',
    scan_data TEXT NOT NULL,
    scan_datetime DATETIME DEFAULT CURRENT_TIMESTAMP,
    time_out DATETIME NULL,
    scan_purpose ENUM('attendance', 'identification', 'verification') DEFAULT 'attendance',
    location VARCHAR(100),
    status ENUM('success', 'failed', 'duplicate') DEFAULT 'success',
    notes TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (student_id) REFERENCES students(student_id) ON DELETE CASCADE,
    FOREIGN KEY (device_id) REFERENCES devices(device_id) ON DELETE SET NULL,
    INDEX idx_student_scan (student_id, scan_datetime),
    INDEX idx_device_scan (device_id, scan_datetime),
    INDEX idx_scan_date (scan_datetime),
    INDEX idx_scan_type (scan_type),
    INDEX idx_status (status)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================
-- Table: tokens (QR Code Token Management)
-- ============================================
CREATE TABLE tokens (
    token_id INT AUTO_INCREMENT PRIMARY KEY,
    student_id INT NOT NULL,
    token_type ENUM('QR') DEFAULT 'QR',
    token_value TEXT NOT NULL,
    issue_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    expiry_date DATE NULL,
    is_active BOOLEAN DEFAULT TRUE,
    revocation_reason TEXT NULL,
    revoked_at TIMESTAMP NULL,
    FOREIGN KEY (student_id) REFERENCES students(student_id) ON DELETE CASCADE,
    INDEX idx_token_value (token_value(255)),
    INDEX idx_student_token (student_id),
    INDEX idx_active_tokens (is_active, expiry_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================
-- Table: system_settings (Configuration)
-- ============================================
CREATE TABLE system_settings (
    setting_id INT AUTO_INCREMENT PRIMARY KEY,
    setting_key VARCHAR(100) NOT NULL UNIQUE,
    setting_value TEXT NOT NULL,
    setting_category ENUM('Scanner', 'System', 'Database', 'UI') DEFAULT 'System',
    description VARCHAR(255),
    updated_by INT,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (updated_by) REFERENCES users(user_id) ON DELETE SET NULL,
    INDEX idx_setting_key (setting_key),
    INDEX idx_category (setting_category)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================
-- Table: system_logs (Audit Trail)
-- ============================================
CREATE TABLE system_logs (
    log_id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT,
    action VARCHAR(100) NOT NULL,
    table_name VARCHAR(50),
    record_id INT,
    old_value TEXT,
    new_value TEXT,
    ip_address VARCHAR(45),
    timestamp TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (user_id) REFERENCES users(user_id) ON DELETE SET NULL,
    INDEX idx_user_log (user_id, timestamp),
    INDEX idx_action (action),
    INDEX idx_timestamp (timestamp)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================
-- Insert Default Data
-- ============================================

-- Default Admin User (password: admin123 - must hash in C# with BCrypt)
-- Placeholder hash - will be replaced by C# application on first run
INSERT INTO users (username, password_hash, full_name, email, role) VALUES
('admin', 'TEMP_HASH_REPLACE_ON_FIRST_RUN', 'System Administrator', 'admin@school.edu', 'admin'),
('staff1', 'TEMP_HASH_REPLACE_ON_FIRST_RUN', 'John Staff', 'staff@school.edu', 'staff');

-- Default QR Scanner Device
INSERT INTO devices (device_name, device_type, location, status) VALUES
('QR Scanner 01', 'QR_SCANNER', 'Pamantasan ng Cabuyao Building', 'active'),
('QR Scanner 02', 'QR_SCANNER', 'Library', 'active');

-- Default System Settings
INSERT INTO system_settings (setting_key, setting_value, setting_category, description) VALUES
('qr_scanner_enabled', 'true', 'Scanner', 'Enable QR code scanning'),
('connection_timeout', '30', 'Scanner', 'Scanner connection timeout in seconds'),
('beep_on_scan', 'true', 'Scanner', 'Play beep sound on successful scan'),
('auto_logout_timer', '15', 'System', 'Auto logout timer in minutes'),
('theme', 'Light', 'UI', 'UI theme (Light/Dark)'),
('language', 'English', 'System', 'System language'),
('database_version', '1.0.0', 'Database', 'Current database schema version');

-- Sample Students for Testing
INSERT INTO students (
    student_number, first_name, middle_name, last_name, 
    email, phone, year_level, program, section,
    qr_code_data, enrollment_date, status
) VALUES
('2024-STU-0001', 'John', 'M.', 'Smith', 'john.smith@school.edu', '09171234567', '3', 'Computer Science', 'CS-3A', 
'ID:2024-STU-0001|Name:John M. Smith|Email:john.smith@school.edu|Course:Computer Science|Year:3', '2021-08-15', 'Active'),
('2024-STU-0002', 'Emily', 'R.', 'Johnson', 'emily.johnson@school.edu', '09181234567', '2', 'Information Technology', 'IT-2B', 
'ID:2024-STU-0002|Name:Emily R. Johnson|Email:emily.johnson@school.edu|Course:Information Technology|Year:2', '2022-08-15', 'Active'),
('2024-STU-0003', 'Michael', 'A.', 'Brown', 'michael.brown@school.edu', '09191234567', '4', 'Computer Science', 'CS-4A', 
'ID:2024-STU-0003|Name:Michael A. Brown|Email:michael.brown@school.edu|Course:Computer Science|Year:4', '2020-08-15', 'Active'),
('2024-STU-0004', 'Sarah', 'L.', 'Davis', 'sarah.davis@school.edu', '09201234567', '1', 'Information Technology', 'IT-1A', 
'ID:2024-STU-0004|Name:Sarah L. Davis|Email:sarah.davis@school.edu|Course:Information Technology|Year:1', '2023-08-15', 'Active'),
('2024-STU-0005', 'David', 'K.', 'Wilson', 'david.wilson@school.edu', '09211234567', '3', 'Computer Engineering', 'CE-3B', 
'ID:2024-STU-0005|Name:David K. Wilson|Email:david.wilson@school.edu|Course:Computer Engineering|Year:3', '2021-08-15', 'Active');

-- Sample Scan History
INSERT INTO scan_history (student_id, device_id, scan_type, scan_data, scan_datetime, location, status) VALUES
(1, 1, 'QR', 'ID:2024-STU-0001|Name:John M. Smith|Email:john.smith@school.edu|Course:Computer Science|Year:3', NOW(), 'Pamantasan ng Cabuyao Building', 'success'),
(2, 1, 'QR', 'ID:2024-STU-0002|Name:Emily R. Johnson|Email:emily.johnson@school.edu|Course:Information Technology|Year:2', DATE_SUB(NOW(), INTERVAL 5 MINUTE), 'Pamantasan ng Cabuyao Building', 'success'),
(3, 1, 'QR', 'ID:2024-STU-0003|Name:Michael A. Brown|Email:michael.brown@school.edu|Course:Computer Science|Year:4', DATE_SUB(NOW(), INTERVAL 12 MINUTE), 'Pamantasan ng Cabuyao Building', 'success'),
(4, 2, 'QR', 'ID:2024-STU-0004|Name:Sarah L. Davis|Email:sarah.davis@school.edu|Course:Information Technology|Year:1', DATE_SUB(NOW(), INTERVAL 18 MINUTE), 'Library', 'success'),
(5, 1, 'QR', 'ID:2024-STU-0005|Name:David K. Wilson|Email:david.wilson@school.edu|Course:Computer Engineering|Year:3', DATE_SUB(NOW(), INTERVAL 25 MINUTE), 'Pamantasan ng Cabuyao Building', 'success');

-- ============================================
-- Stored Procedures (QR Code Focused)
-- ============================================

DELIMITER //

-- Procedure: Register New Student
CREATE PROCEDURE sp_register_student(
    IN p_student_number VARCHAR(50),
    IN p_first_name VARCHAR(50),
    IN p_middle_name VARCHAR(50),
    IN p_last_name VARCHAR(50),
    IN p_email VARCHAR(100),
    IN p_phone VARCHAR(20),
    IN p_year_level VARCHAR(10),
    IN p_program VARCHAR(100),
    IN p_section VARCHAR(50),
    IN p_qr_code_data TEXT,
    IN p_enrollment_date DATE,
    OUT p_student_id INT,
    OUT p_result VARCHAR(100)
)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        SET p_student_id = -1;
        SET p_result = 'ERROR: Database error occurred';
    END;
    
    -- Check for duplicate student number
    IF EXISTS (SELECT 1 FROM students WHERE student_number = p_student_number) THEN
        SET p_student_id = -1;
        SET p_result = 'ERROR: Student number already exists';
    ELSEIF EXISTS (SELECT 1 FROM students WHERE email = p_email) THEN
        SET p_student_id = -1;
        SET p_result = 'ERROR: Email already exists';
    ELSE
        START TRANSACTION;
        
        INSERT INTO students (
            student_number, first_name, middle_name, last_name, 
            email, phone, year_level, program, section,
            qr_code_data, enrollment_date
        ) VALUES (
            p_student_number, p_first_name, p_middle_name, p_last_name,
            p_email, p_phone, p_year_level, p_program, p_section,
            p_qr_code_data, p_enrollment_date
        );
        
        SET p_student_id = LAST_INSERT_ID();
        
        -- Create QR token
        INSERT INTO tokens (student_id, token_type, token_value)
        VALUES (p_student_id, 'QR', p_qr_code_data);
        
        COMMIT;
        SET p_result = 'SUCCESS';
    END IF;
END //

-- Procedure: Record QR Code Scan
CREATE PROCEDURE sp_record_scan(
    IN p_scan_data TEXT,
    IN p_device_id INT,
    IN p_location VARCHAR(100),
    OUT p_result VARCHAR(100),
    OUT p_student_name VARCHAR(200),
    OUT p_student_number VARCHAR(50)
)
BEGIN
    DECLARE v_student_id INT;
    DECLARE v_student_first VARCHAR(50);
    DECLARE v_student_last VARCHAR(50);
    DECLARE v_student_num VARCHAR(50);
    DECLARE v_last_scan TIMESTAMP;
    DECLARE v_duplicate BOOLEAN DEFAULT FALSE;
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        SET p_result = 'ERROR: Database error occurred';
        SET p_student_name = NULL;
        SET p_student_number = NULL;
    END;
    
    -- Find student by QR code data
    SELECT student_id, first_name, last_name, student_number 
    INTO v_student_id, v_student_first, v_student_last, v_student_num
    FROM students
    WHERE qr_code_data = p_scan_data
      AND status = 'Active'
    LIMIT 1;
    
    IF v_student_id IS NULL THEN
        SET p_result = 'ERROR: Student not found or inactive';
        SET p_student_name = NULL;
        SET p_student_number = NULL;
    ELSE
        SET p_student_name = CONCAT(v_student_first, ' ', v_student_last);
        SET p_student_number = v_student_num;
        
        -- Check for duplicate scan (within last 5 minutes)
        SELECT scan_datetime INTO v_last_scan
        FROM scan_history
        WHERE student_id = v_student_id
          AND scan_datetime > DATE_SUB(NOW(), INTERVAL 5 MINUTE)
        ORDER BY scan_datetime DESC
        LIMIT 1;
        
        IF v_last_scan IS NOT NULL THEN
            SET v_duplicate = TRUE;
        END IF;
        
        -- Insert scan record
        INSERT INTO scan_history (
            student_id, device_id, scan_type, scan_data, 
            scan_datetime, location, status
        ) VALUES (
            v_student_id, p_device_id, 'QR', p_scan_data,
            NOW(), p_location, IF(v_duplicate, 'duplicate', 'success')
        );
        
        IF v_duplicate THEN
            SET p_result = 'WARNING: Duplicate scan detected';
        ELSE
            SET p_result = 'SUCCESS';
        END IF;
    END IF;
END //

-- Procedure: Get Scan History with Filters
CREATE PROCEDURE sp_get_scan_history(
    IN p_start_date DATE,
    IN p_end_date DATE,
    IN p_student_id INT,
    IN p_limit INT,
    IN p_offset INT
)
BEGIN
    SELECT 
        sh.scan_id,
        s.student_number,
        CONCAT(s.first_name, ' ', s.last_name) AS student_name,
        sh.scan_type,
        sh.scan_datetime,
        sh.time_out,
        sh.location,
        sh.status,
        d.device_name
    FROM scan_history sh
    JOIN students s ON sh.student_id = s.student_id
    LEFT JOIN devices d ON sh.device_id = d.device_id
    WHERE (p_start_date IS NULL OR DATE(sh.scan_datetime) >= p_start_date)
      AND (p_end_date IS NULL OR DATE(sh.scan_datetime) <= p_end_date)
      AND (p_student_id IS NULL OR sh.student_id = p_student_id)
    ORDER BY sh.scan_datetime DESC
    LIMIT p_limit OFFSET p_offset;
END //

-- Procedure: Get Daily Summary Statistics
CREATE PROCEDURE sp_get_daily_summary(
    IN p_date DATE
)
BEGIN
    DECLARE v_target_date DATE;
    SET v_target_date = IFNULL(p_date, CURDATE());
    
    SELECT 
        COUNT(DISTINCT student_id) AS total_students_scanned,
        COUNT(*) AS total_scans,
        SUM(CASE WHEN scan_type = 'QR' THEN 1 ELSE 0 END) AS qr_scans,
        SUM(CASE WHEN status = 'success' THEN 1 ELSE 0 END) AS successful_scans,
        SUM(CASE WHEN status = 'duplicate' THEN 1 ELSE 0 END) AS duplicate_scans,
        SUM(CASE WHEN status = 'failed' THEN 1 ELSE 0 END) AS failed_scans
    FROM scan_history
    WHERE DATE(scan_datetime) = v_target_date;
END //

-- Procedure: Get Student by QR Code
CREATE PROCEDURE sp_get_student_by_qrcode(
    IN p_qr_code TEXT,
    OUT p_student_id INT,
    OUT p_student_number VARCHAR(50),
    OUT p_full_name VARCHAR(200),
    OUT p_email VARCHAR(100),
    OUT p_program VARCHAR(100),
    OUT p_year_level VARCHAR(10),
    OUT p_status VARCHAR(20)
)
BEGIN
    SELECT 
        student_id,
        student_number,
        CONCAT(first_name, ' ', IFNULL(middle_name, ''), ' ', last_name),
        email,
        program,
        year_level,
        status
    INTO 
        p_student_id,
        p_student_number,
        p_full_name,
        p_email,
        p_program,
        p_year_level,
        p_status
    FROM students
    WHERE qr_code_data = p_qr_code
    LIMIT 1;
END //

DELIMITER ;

-- ============================================
-- Views for Reporting
-- ============================================

-- View: Active Students with QR Codes
CREATE VIEW vw_active_students AS
SELECT 
    s.student_id,
    s.student_number,
    CONCAT(s.first_name, ' ', IFNULL(s.middle_name, ''), ' ', s.last_name) AS full_name,
    s.email,
    s.phone,
    s.year_level,
    s.program,
    s.section,
    s.status,
    s.enrollment_date,
    COUNT(t.token_id) AS active_tokens
FROM students s
LEFT JOIN tokens t ON s.student_id = t.student_id AND t.is_active = TRUE
WHERE s.status = 'Active'
GROUP BY s.student_id;

-- View: Recent Scans (Last 24 Hours)
CREATE VIEW vw_recent_scans AS
SELECT 
    sh.scan_id,
    s.student_number,
    CONCAT(s.first_name, ' ', s.last_name) AS student_name,
    sh.scan_type,
    sh.scan_datetime,
    sh.location,
    sh.status,
    d.device_name
FROM scan_history sh
JOIN students s ON sh.student_id = s.student_id
LEFT JOIN devices d ON sh.device_id = d.device_id
WHERE sh.scan_datetime >= DATE_SUB(NOW(), INTERVAL 24 HOUR)
ORDER BY sh.scan_datetime DESC;

-- View: Student Scan Statistics
CREATE VIEW vw_student_scan_stats AS
SELECT 
    s.student_id,
    s.student_number,
    CONCAT(s.first_name, ' ', s.last_name) AS student_name,
    s.program,
    s.year_level,
    COUNT(sh.scan_id) AS total_scans,
    MAX(sh.scan_datetime) AS last_scan,
    MIN(sh.scan_datetime) AS first_scan,
    SUM(CASE WHEN DATE(sh.scan_datetime) = CURDATE() THEN 1 ELSE 0 END) AS scans_today
FROM students s
LEFT JOIN scan_history sh ON s.student_id = sh.student_id
WHERE s.status = 'Active'
GROUP BY s.student_id;

-- View: Device Usage Statistics
CREATE VIEW vw_device_stats AS
SELECT 
    d.device_id,
    d.device_name,
    d.device_type,
    d.location,
    d.status,
    COUNT(sh.scan_id) AS total_scans,
    MAX(sh.scan_datetime) AS last_scan,
    SUM(CASE WHEN DATE(sh.scan_datetime) = CURDATE() THEN 1 ELSE 0 END) AS scans_today
FROM devices d
LEFT JOIN scan_history sh ON d.device_id = sh.device_id
GROUP BY d.device_id;

-- ============================================
-- Triggers for Audit Trail
-- ============================================

DELIMITER //

-- Trigger: Log student updates
CREATE TRIGGER trg_student_update_log
AFTER UPDATE ON students
FOR EACH ROW
BEGIN
    IF OLD.status != NEW.status THEN
        INSERT INTO system_logs (action, table_name, record_id, old_value, new_value)
        VALUES (
            'UPDATE',
            'students',
            NEW.student_id,
            CONCAT('Status: ', OLD.status),
            CONCAT('Status: ', NEW.status)
        );
    END IF;
END //

-- Trigger: Log student deletions
CREATE TRIGGER trg_student_delete_log
BEFORE DELETE ON students
FOR EACH ROW
BEGIN
    INSERT INTO system_logs (action, table_name, record_id, old_value)
    VALUES (
        'DELETE',
        'students',
        OLD.student_id,
        CONCAT('Student: ', OLD.student_number, ' - ', OLD.first_name, ' ', OLD.last_name)
    );
END //

-- Trigger: Update device last_active on scan
CREATE TRIGGER trg_update_device_active
AFTER INSERT ON scan_history
FOR EACH ROW
BEGIN
    UPDATE devices
    SET last_active = NOW()
    WHERE device_id = NEW.device_id;
END //

DELIMITER ;

-- ============================================
-- Performance Indexes
-- ============================================

-- Additional composite indexes for common queries
CREATE INDEX idx_student_program_year ON students(program, year_level, status);
CREATE INDEX idx_scan_student_date ON scan_history(student_id, scan_datetime, status);

-- ============================================
-- Database Schema Information
-- ============================================

SELECT 
    'Database schema created successfully - QR Code Only System' AS message,
    DATABASE() AS current_database,
    (SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = DATABASE()) AS total_tables,
    (SELECT COUNT(*) FROM information_schema.routines WHERE routine_schema = DATABASE()) AS total_procedures,
    (SELECT COUNT(*) FROM information_schema.views WHERE table_schema = DATABASE()) AS total_views;

-- ============================================
-- End of Schema - Ready for MySqlConnector
-- ============================================
