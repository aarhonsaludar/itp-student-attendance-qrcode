-- ============================================
-- STUDENT ATTENDANCE SYSTEM - FRESH INSTALL SETUP
-- Complete Database Installation Script
-- Version: 1.0.0
-- Date: December 5, 2025
-- ============================================
-- This script performs a complete fresh installation of the
-- Student Attendance System database including:
-- 1. Database creation
-- 2. All tables with proper structure
-- 3. All indexes and foreign keys
-- 4. All stored procedures and triggers
-- 5. All views for reporting
-- 6. Time validation and anti-tampering features
-- 7. Default data (users, devices, settings)
-- ============================================

-- ============================================
-- STEP 1: Create Database
-- ============================================

DROP DATABASE IF EXISTS student_attendance_db;

CREATE DATABASE student_attendance_db
CHARACTER SET utf8mb4 
COLLATE utf8mb4_unicode_ci;

USE student_attendance_db;

SELECT 'Step 1/10: Database created successfully' AS Status;

-- ============================================
-- STEP 2: Create Tables
-- ============================================

-- Table: users (System Users/Administrators)
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

-- Table: students (Student Information)
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
    home_address VARCHAR(255),
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
    INDEX idx_status (status),
    INDEX idx_student_program_year (program, year_level, status),
    INDEX idx_home_address (home_address)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Table: devices (Scanning Devices)
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

-- Table: scan_history (QR Code Scan Records with Anti-Tampering)
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
    status ENUM('success', 'failed', 'duplicate', 'for_review') DEFAULT 'success',
    notes TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    
    -- Time Validation Fields (Migration 005)
    validation_status VARCHAR(30) DEFAULT 'verified',
    time_in_validation_mode VARCHAR(20) DEFAULT NULL,
    time_out_validation_mode VARCHAR(20) DEFAULT NULL,
    requires_review BOOLEAN DEFAULT FALSE,
    client_time DATETIME NULL,
    server_time DATETIME NULL,
    time_drift_seconds INT NULL,
    
    -- TickCount Anti-Tampering Fields (Migration 007)
    time_in_tick_count BIGINT NULL COMMENT 'Stopwatch.GetTimestamp() at Time In - tamper-proof',
    time_out_tick_count BIGINT NULL COMMENT 'Stopwatch.GetTimestamp() at Time Out - tamper-proof',
    connection_drop_count INT NULL DEFAULT 0 COMMENT 'Number of connection drops during session',
    offline_duration_minutes DOUBLE NULL COMMENT 'Total offline duration in minutes',
    
    -- Foreign Keys
    FOREIGN KEY (student_id) REFERENCES students(student_id) ON DELETE CASCADE,
    FOREIGN KEY (device_id) REFERENCES devices(device_id) ON DELETE SET NULL,
    
    -- Indexes for Performance
    INDEX idx_student_scan (student_id, scan_datetime),
    INDEX idx_device_scan (device_id, scan_datetime),
    INDEX idx_scan_date (scan_datetime),
    INDEX idx_scan_type (scan_type),
    INDEX idx_status (status),
    INDEX idx_validation_modes (time_in_validation_mode, time_out_validation_mode, status),
    INDEX idx_tickcount_validation (time_in_tick_count, time_out_tick_count),
    INDEX idx_scan_student_date (student_id, scan_datetime, status)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Table: system_settings (Configuration)
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

-- Table: system_logs (Audit Trail)
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

SELECT 'Step 2/10: All tables created successfully' AS Status;

-- ============================================
-- STEP 3: Create Stored Procedures
-- ============================================

DELIMITER //

-- Procedure: Record Attendance Scan (Secure with Time Validation)
CREATE PROCEDURE sp_record_attendance_scan_secure(
    IN p_student_id INT,
    IN p_device_id INT,
    IN p_scan_type VARCHAR(10),
    IN p_scan_data TEXT,
    IN p_location VARCHAR(100),
    IN p_validation_status VARCHAR(30),
    IN p_validation_mode VARCHAR(20),
    IN p_client_time DATETIME,
    IN p_server_time DATETIME,
    IN p_time_drift_seconds INT,
    IN p_tick_count BIGINT,
    OUT p_scan_id INT,
    OUT p_action VARCHAR(20),
    OUT p_message VARCHAR(255)
)
BEGIN
    DECLARE v_existing_scan_id INT;
    DECLARE v_existing_time_in DATETIME;
    DECLARE v_duration_minutes INT;
    DECLARE v_min_duration_minutes INT DEFAULT 15;
    DECLARE v_max_duration_hours INT DEFAULT 18;
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        SET p_message = 'Database error occurred';
        SET p_action = 'ERROR';
    END;
    
    START TRANSACTION;
    
    -- Check for existing Time-In today without Time-Out
    SELECT scan_id, scan_datetime INTO v_existing_scan_id, v_existing_time_in
    FROM scan_history
    WHERE student_id = p_student_id
      AND DATE(scan_datetime) = CURDATE()
      AND time_out IS NULL
      AND status != 'failed'
    LIMIT 1;
    
    IF v_existing_scan_id IS NOT NULL THEN
        -- This is a TIME-OUT
        
        -- Calculate duration
        SET v_duration_minutes = TIMESTAMPDIFF(MINUTE, v_existing_time_in, NOW());
        
        -- Validate minimum duration
        IF v_duration_minutes < v_min_duration_minutes THEN
            SET p_message = CONCAT('Minimum duration not met. Please wait ', 
                                  (v_min_duration_minutes - v_duration_minutes), 
                                  ' more minutes.');
            SET p_action = 'BLOCKED';
            SET p_scan_id = NULL;
            ROLLBACK;
        -- Validate maximum duration
        ELSEIF v_duration_minutes > (v_max_duration_hours * 60) THEN
            SET p_message = 'Duration exceeded maximum allowed time. Please contact admin.';
            SET p_action = 'BLOCKED';
            SET p_scan_id = NULL;
            ROLLBACK;
        ELSE
            -- Update existing record with Time-Out
            UPDATE scan_history
            SET time_out = NOW(),
                time_out_validation_mode = p_validation_mode,
                time_out_tick_count = p_tick_count,
                status = 'success'
            WHERE scan_id = v_existing_scan_id;
            
            SET p_scan_id = v_existing_scan_id;
            SET p_action = 'TIME_OUT';
            SET p_message = CONCAT('Time-Out recorded successfully. Duration: ', v_duration_minutes, ' minutes');
            COMMIT;
        END IF;
    ELSE
        -- This is a TIME-IN
        
        -- Insert new Time-In record
        INSERT INTO scan_history (
            student_id,
            device_id,
            scan_type,
            scan_data,
            scan_datetime,
            location,
            status,
            validation_status,
            time_in_validation_mode,
            client_time,
            server_time,
            time_drift_seconds,
            time_in_tick_count,
            requires_review
        ) VALUES (
            p_student_id,
            p_device_id,
            p_scan_type,
            p_scan_data,
            NOW(),
            p_location,
            'success',
            p_validation_status,
            p_validation_mode,
            p_client_time,
            p_server_time,
            p_time_drift_seconds,
            p_tick_count,
            CASE WHEN p_validation_mode = 'offline' THEN TRUE ELSE FALSE END
        );
        
        SET p_scan_id = LAST_INSERT_ID();
        SET p_action = 'TIME_IN';
        SET p_message = 'Time-In recorded successfully';
        COMMIT;
    END IF;
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
        SUM(CASE WHEN scan_type = 'MANUAL' THEN 1 ELSE 0 END) AS manual_scans,
        SUM(CASE WHEN status = 'success' THEN 1 ELSE 0 END) AS successful_scans,
        SUM(CASE WHEN status = 'duplicate' THEN 1 ELSE 0 END) AS duplicate_scans,
        SUM(CASE WHEN status = 'failed' THEN 1 ELSE 0 END) AS failed_scans,
        SUM(CASE WHEN status = 'for_review' THEN 1 ELSE 0 END) AS scans_for_review,
        SUM(CASE WHEN time_out IS NOT NULL THEN 1 ELSE 0 END) AS completed_attendances,
        SUM(CASE WHEN time_out IS NULL AND status = 'success' THEN 1 ELSE 0 END) AS pending_timeouts,
        SUM(CASE WHEN validation_status = 'offline_mode' THEN 1 ELSE 0 END) AS offline_scans,
        SUM(CASE WHEN requires_review = TRUE THEN 1 ELSE 0 END) AS flagged_for_review
    FROM scan_history
    WHERE DATE(scan_datetime) = v_target_date;
END //

-- Procedure: Get Student Attendance History
CREATE PROCEDURE sp_get_student_attendance(
    IN p_student_id INT,
    IN p_start_date DATE,
    IN p_end_date DATE
)
BEGIN
    SELECT 
        sh.scan_id,
        sh.scan_datetime AS time_in,
        sh.time_out,
        sh.location,
        sh.status,
        sh.validation_status,
        sh.time_in_validation_mode,
        sh.time_out_validation_mode,
        sh.requires_review,
        sh.time_drift_seconds,
        d.device_name,
        d.location AS device_location,
        CASE 
            WHEN sh.time_out IS NOT NULL 
            THEN TIMESTAMPDIFF(MINUTE, sh.scan_datetime, sh.time_out)
            ELSE NULL 
        END AS duration_minutes,
        CASE 
            WHEN sh.time_out IS NOT NULL THEN 'Completed'
            WHEN sh.time_out IS NULL AND sh.status = 'success' THEN 'Pending Time-Out'
            ELSE 'Incomplete'
        END AS attendance_status
    FROM scan_history sh
    LEFT JOIN devices d ON sh.device_id = d.device_id
    WHERE sh.student_id = p_student_id
      AND DATE(sh.scan_datetime) BETWEEN IFNULL(p_start_date, '2000-01-01') 
                                     AND IFNULL(p_end_date, CURDATE())
    ORDER BY sh.scan_datetime DESC;
END //

DELIMITER ;

SELECT 'Step 3/10: Stored procedures created successfully' AS Status;

-- ============================================
-- STEP 4: Create Triggers
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

SELECT 'Step 4/10: Triggers created successfully' AS Status;

-- ============================================
-- STEP 5: Create Views
-- ============================================

-- View: Recent Scans (Last 24 Hours) - Excludes for_review status
CREATE VIEW vw_recent_scans AS
SELECT 
    sh.scan_id,
    sh.student_id,
    sh.device_id,
    s.student_number,
    CONCAT(s.first_name, ' ', s.last_name) AS student_name,
    s.program,
    s.year_level,
    s.section,
    sh.scan_type,
    sh.scan_data,
    sh.scan_datetime,
    sh.scan_datetime AS time_in,
    sh.time_out,
    sh.scan_purpose,
    sh.location,
    sh.status,
    sh.notes,
    sh.created_at,
    sh.validation_status,
    sh.time_in_validation_mode,
    sh.time_out_validation_mode,
    sh.requires_review,
    sh.client_time,
    sh.server_time,
    sh.time_drift_seconds,
    d.device_name,
    d.location AS device_location,
    CASE 
        WHEN sh.time_out IS NOT NULL THEN 'completed'
        WHEN sh.time_out IS NULL AND sh.scan_datetime >= CURDATE() THEN 'pending_out'
        ELSE 'incomplete'
    END AS attendance_status,
    CASE 
        WHEN sh.time_out IS NOT NULL 
        THEN TIMESTAMPDIFF(MINUTE, sh.scan_datetime, sh.time_out)
        ELSE NULL 
    END AS duration_minutes
FROM scan_history sh
JOIN students s ON sh.student_id = s.student_id
LEFT JOIN devices d ON sh.device_id = d.device_id
WHERE sh.scan_datetime >= DATE_SUB(NOW(), INTERVAL 24 HOUR)
    AND sh.status != 'for_review'
ORDER BY sh.scan_datetime DESC;

-- View: Student Scan Statistics
CREATE VIEW vw_student_scan_stats AS
SELECT 
    s.student_id,
    s.student_number,
    CONCAT(s.first_name, ' ', s.last_name) AS student_name,
    s.program,
    s.year_level,
    s.status AS student_status,
    COUNT(sh.scan_id) AS total_scans,
    MAX(sh.scan_datetime) AS last_scan,
    MIN(sh.scan_datetime) AS first_scan,
    SUM(CASE WHEN DATE(sh.scan_datetime) = CURDATE() THEN 1 ELSE 0 END) AS scans_today,
    SUM(CASE WHEN sh.time_out IS NOT NULL THEN 1 ELSE 0 END) AS completed_attendances,
    SUM(CASE WHEN sh.time_out IS NULL AND sh.status = 'success' THEN 1 ELSE 0 END) AS pending_timeouts
FROM students s
LEFT JOIN scan_history sh ON s.student_id = sh.student_id
WHERE s.status = 'Active'
GROUP BY s.student_id;

-- View: Scans Pending Review (Offline/Flagged)
CREATE VIEW vw_scans_pending_review AS
SELECT 
    sh.scan_id,
    sh.student_id,
    s.student_number,
    CONCAT(s.first_name, ' ', s.last_name) AS student_name,
    sh.scan_datetime AS time_in,
    sh.time_out,
    sh.validation_status,
    sh.time_in_validation_mode,
    sh.time_out_validation_mode,
    sh.client_time,
    sh.server_time,
    sh.time_drift_seconds,
    sh.time_in_tick_count,
    sh.time_out_tick_count,
    sh.connection_drop_count,
    sh.offline_duration_minutes,
    sh.status,
    sh.notes,
    d.device_name,
    CASE 
        WHEN sh.time_out IS NOT NULL 
        THEN TIMESTAMPDIFF(MINUTE, sh.scan_datetime, sh.time_out)
        ELSE NULL 
    END AS duration_minutes,
    CASE
        WHEN sh.validation_status = 'offline_mode' THEN 'Offline Mode'
        WHEN sh.time_in_validation_mode = 'online' AND sh.time_out_validation_mode = 'offline' THEN 'WiFi Disconnect Detected'
        WHEN sh.connection_drop_count > 3 THEN 'Suspicious Connection Drops'
        WHEN sh.offline_duration_minutes > 60 THEN 'Excessive Offline Duration'
        ELSE 'Manual Review Required'
    END AS review_reason
FROM scan_history sh
JOIN students s ON sh.student_id = s.student_id
LEFT JOIN devices d ON sh.device_id = d.device_id
WHERE sh.requires_review = TRUE
   OR sh.validation_status = 'offline_mode'
   OR (sh.time_in_validation_mode = 'online' AND sh.time_out_validation_mode = 'offline')
ORDER BY sh.scan_datetime DESC;

-- View: Daily Offline Scans Summary
CREATE VIEW vw_daily_offline_scans AS
SELECT 
    DATE(sh.scan_datetime) AS scan_date,
    COUNT(*) AS total_offline_scans,
    COUNT(DISTINCT sh.student_id) AS unique_students,
    SUM(CASE WHEN sh.time_out IS NOT NULL THEN 1 ELSE 0 END) AS completed_offline,
    SUM(CASE WHEN sh.time_out IS NULL THEN 1 ELSE 0 END) AS pending_offline,
    SUM(CASE WHEN sh.requires_review = TRUE THEN 1 ELSE 0 END) AS flagged_offline,
    AVG(sh.time_drift_seconds) AS avg_time_drift_seconds
FROM scan_history sh
WHERE sh.validation_status = 'offline_mode'
   OR sh.time_in_validation_mode = 'offline'
   OR sh.time_out_validation_mode = 'offline'
GROUP BY DATE(sh.scan_datetime)
ORDER BY scan_date DESC;

SELECT 'Step 5/10: Views created successfully' AS Status;

-- ============================================
-- STEP 6: Insert Default Users
-- ============================================

-- Note: Password hashes will be replaced by C# application on first run
-- Default password for both users: admin123

INSERT INTO users (username, password_hash, full_name, email, role) VALUES
('admin', 'TEMP_HASH_REPLACE_ON_FIRST_RUN', 'System Administrator', 'admin@school.edu', 'admin'),
('staff1', 'TEMP_HASH_REPLACE_ON_FIRST_RUN', 'John Staff', 'staff@school.edu', 'staff');

SELECT 'Step 6/10: Default users created successfully' AS Status;

-- ============================================
-- STEP 7: Insert Default Devices
-- ============================================

INSERT INTO devices (device_name, device_type, location, status) VALUES
('QR Scanner 01', 'QR_SCANNER', 'Main Building - Entrance', 'active'),
('QR Scanner 02', 'QR_SCANNER', 'Library - Front Desk', 'active'),
('QR Scanner 03', 'QR_SCANNER', 'Computer Lab - Room 301', 'active');

SELECT 'Step 7/10: Default devices created successfully' AS Status;

-- ============================================
-- STEP 8: Insert Default System Settings
-- ============================================

INSERT INTO system_settings (setting_key, setting_value, setting_category, description) VALUES
-- Scanner Settings
('qr_scanner_enabled', 'true', 'Scanner', 'Enable QR code scanning'),
('connection_timeout', '30', 'Scanner', 'Scanner connection timeout in seconds'),
('beep_on_scan', 'true', 'Scanner', 'Play beep sound on successful scan'),
('scan_cooldown_seconds', '5', 'Scanner', 'Cooldown time between scans in seconds'),

-- System Settings
('auto_logout_timer', '15', 'System', 'Auto logout timer in minutes'),
('language', 'English', 'System', 'System language'),
('enable_audit_logging', 'true', 'System', 'Enable comprehensive audit logging'),

-- Time Validation Settings
('min_attendance_duration_minutes', '15', 'System', 'Minimum duration between Time-In and Time-Out'),
('max_attendance_duration_hours', '18', 'System', 'Maximum attendance session duration'),
('time_drift_tolerance_seconds', '300', 'System', 'Maximum allowed time drift (5 minutes)'),
('tickcount_tolerance_minutes', '3', 'System', 'TickCount comparison tolerance'),
('enable_online_validation', 'true', 'System', 'Enable online time validation'),
('enable_offline_detection', 'true', 'System', 'Enable offline tampering detection'),

-- Email/OTP Settings
('otp_expiration_minutes', '5', 'System', 'OTP code expiration time'),
('otp_resend_limit', '3', 'System', 'Maximum OTP resend attempts'),

-- UI Settings
('theme', 'Light', 'UI', 'UI theme (Light/Dark)'),
('font_size', 'Medium', 'UI', 'Default font size'),

-- Database Settings
('database_version', '1.0.0', 'Database', 'Current database schema version'),
('last_backup', NULL, 'Database', 'Last database backup timestamp');

SELECT 'Step 8/10: Default system settings created successfully' AS Status;

-- ============================================
-- STEP 9: Insert Sample Student (Optional - for testing)
-- ============================================

-- Sample student for testing purposes
INSERT INTO students (
    student_number,
    first_name,
    middle_name,
    last_name,
    email,
    phone,
    sex,
    year_level,
    program,
    section,
    home_address,
    qr_code_data,
    status,
    enrollment_date
) VALUES (
    '2024-00001',
    'Juan',
    'Santos',
    'Dela Cruz',
    'juan.delacruz@students.plc.edu.ph',
    '09123456789',
    'Male',
    '4',
    'BS Information Technology',
    'IT-4A',
    'Cabuyao, Laguna',
    'QR_2024-00001_JUAN_DELACRUZ',
    'Active',
    '2024-08-15'
);

SELECT 'Step 9/10: Sample student created successfully (for testing)' AS Status;

-- ============================================
-- STEP 10: Verification and Summary
-- ============================================

-- Display installation summary
SELECT 
    '============================================' AS '',
    'FRESH INSTALLATION COMPLETED SUCCESSFULLY' AS Status,
    '============================================' AS ' ';

SELECT 
    'Database Information' AS Category,
    DATABASE() AS current_database,
    (SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = DATABASE()) AS total_tables,
    (SELECT COUNT(*) FROM information_schema.routines WHERE routine_schema = DATABASE() AND routine_type = 'PROCEDURE') AS total_procedures,
    (SELECT COUNT(*) FROM information_schema.views WHERE table_schema = DATABASE()) AS total_views,
    (SELECT COUNT(*) FROM information_schema.triggers WHERE trigger_schema = DATABASE()) AS total_triggers;

SELECT 
    'Default Data Summary' AS Category,
    (SELECT COUNT(*) FROM users) AS total_users,
    (SELECT COUNT(*) FROM devices) AS total_devices,
    (SELECT COUNT(*) FROM system_settings) AS total_settings,
    (SELECT COUNT(*) FROM students) AS total_students;

SELECT 
    'Security Features' AS Category,
    'Online Time Validation' AS feature_1,
    'Offline TickCount Detection' AS feature_2,
    'Duration Enforcement (15 min - 18 hours)' AS feature_3,
    'OTP Email Verification' AS feature_4,
    'Comprehensive Audit Trail' AS feature_5;

SELECT 
    '============================================' AS '',
    'NEXT STEPS' AS Instructions,
    '============================================' AS ' ';

SELECT 
    '1. Update admin password in C# application (default: admin123)' AS step_1,
    '2. Configure SMTP email settings in App.config' AS step_2,
    '3. Update database connection string in App.config' AS step_3,
    '4. Add students via Student Management screen' AS step_4,
    '5. Configure additional devices if needed' AS step_5,
    '6. Test QR scanner functionality' AS step_6,
    '7. Review system settings and adjust as needed' AS step_7;

SELECT 
    '============================================' AS '',
    'DEFAULT CREDENTIALS' AS Login,
    '============================================' AS ' ';

SELECT 
    'Username: admin' AS admin_account,
    'Password: admin123' AS default_password,
    'IMPORTANT: Change password immediately after first login!' AS security_warning;

SELECT 
    '============================================' AS '',
    'DATABASE READY FOR USE' AS Final_Status,
    NOW() AS installation_completed_at,
    '============================================' AS ' ';

-- End of Fresh Install Setup Script
