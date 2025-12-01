-- ============================================
-- Database Cleanup Script
-- Removes unused tables, procedures, and old logs
-- Date: November 30, 2025
-- ============================================

USE student_attendance_db;

-- ============================================
-- BACKUP REMINDER
-- ============================================
SELECT '⚠️ IMPORTANT: Make sure you have a backup before running this cleanup!' as warning;
SELECT 'This script will permanently delete unused database objects.' as notice;
SELECT '' as space;

-- ============================================
-- 1. DROP UNUSED TOKENS TABLE
-- ============================================
SELECT '1. Dropping unused TOKENS table...' as step;

DROP TABLE IF EXISTS tokens;

SELECT '✓ TOKENS table dropped successfully' as result;
SELECT '' as space;

-- ============================================
-- 2. DROP UNUSED STORED PROCEDURES
-- ============================================
SELECT '2. Dropping unused stored procedures...' as step;

DROP PROCEDURE IF EXISTS sp_get_student_by_qrcode;
DROP PROCEDURE IF EXISTS sp_register_student;
DROP PROCEDURE IF EXISTS sp_record_attendance_scan;

SELECT '✓ Unused stored procedures dropped:' as result;
SELECT '  - sp_get_student_by_qrcode' as dropped;
SELECT '  - sp_register_student' as dropped;
SELECT '  - sp_record_attendance_scan (old version)' as dropped;
SELECT '' as space;

-- ============================================
-- 3. ARCHIVE OLD SYSTEM LOGS (Keep last 60 days)
-- ============================================
SELECT '3. Archiving old system logs...' as step;

-- Count logs to be archived
SELECT CONCAT('Old logs to archive: ', COUNT(*)) as count
FROM system_logs
WHERE timestamp < DATE_SUB(NOW(), INTERVAL 60 DAY);

-- Create archive table if it doesn't exist
CREATE TABLE IF NOT EXISTS system_logs_archive (
    log_id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT,
    action VARCHAR(100) NOT NULL,
    table_name VARCHAR(50),
    record_id INT,
    old_value TEXT,
    new_value TEXT,
    ip_address VARCHAR(45),
    timestamp TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    archived_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    INDEX idx_timestamp (timestamp),
    INDEX idx_action (action)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Move old logs to archive
INSERT INTO system_logs_archive 
    (user_id, action, table_name, record_id, old_value, new_value, ip_address, timestamp)
SELECT 
    user_id, action, table_name, record_id, old_value, new_value, ip_address, timestamp
FROM system_logs
WHERE timestamp < DATE_SUB(NOW(), INTERVAL 60 DAY);

-- Delete archived logs from main table
DELETE FROM system_logs
WHERE timestamp < DATE_SUB(NOW(), INTERVAL 60 DAY);

SELECT '✓ Old logs archived to system_logs_archive table' as result;
SELECT '' as space;

-- ============================================
-- 4. OPTIMIZE TABLES
-- ============================================
SELECT '4. Optimizing tables...' as step;

OPTIMIZE TABLE students;
OPTIMIZE TABLE scan_history;
OPTIMIZE TABLE system_logs;
OPTIMIZE TABLE devices;
OPTIMIZE TABLE users;
OPTIMIZE TABLE system_settings;

SELECT '✓ Tables optimized' as result;
SELECT '' as space;

-- ============================================
-- 5. CLEANUP SUMMARY
-- ============================================
SELECT '========================================' as summary;
SELECT 'CLEANUP COMPLETED SUCCESSFULLY!' as summary;
SELECT '========================================' as summary;
SELECT '' as space;

SELECT 'Database objects removed:' as summary;
SELECT '  ✓ tokens table (unused)' as removed;
SELECT '  ✓ 3 unused stored procedures' as removed;
SELECT CONCAT('  ✓ ', 
    (SELECT COUNT(*) FROM system_logs_archive), 
    ' old system logs archived') as removed;
SELECT '' as space;

SELECT 'Current database status:' as summary;
SELECT CONCAT('  • Students: ', COUNT(*)) as status FROM students;
SELECT CONCAT('  • Scan History: ', COUNT(*)) as status FROM scan_history;
SELECT CONCAT('  • Active Logs: ', COUNT(*)) as status FROM system_logs;
SELECT CONCAT('  • Archived Logs: ', COUNT(*)) as status FROM system_logs_archive;
SELECT CONCAT('  • Users: ', COUNT(*)) as status FROM users;
SELECT CONCAT('  • Devices: ', COUNT(*)) as status FROM devices;
SELECT '' as space;

SELECT 'Remaining stored procedures:' as summary;
SHOW PROCEDURE STATUS WHERE Db = 'student_attendance_db';

SELECT '' as space;
SELECT '✓ Database cleanup completed!' as final_status;
