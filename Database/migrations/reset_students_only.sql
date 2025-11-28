-- ============================================
-- Reset Students ONLY (Keep Scan History)
-- This script will:
-- 1. Delete all student records
-- 2. Reset student auto-increment IDs to start from 1
-- 3. KEEPS scan history for audit purposes
-- ============================================

USE student_attendance_db;

-- Step 1: Show current counts (before deletion)
SELECT 
    'BEFORE RESET' as Status,
    (SELECT COUNT(*) FROM students) as Total_Students,
    (SELECT COUNT(*) FROM scan_history) as Total_Scans;

-- Step 2: Backup scan history to a temporary table (optional)
-- You can skip this if you don't need backup
DROP TABLE IF EXISTS scan_history_backup;
CREATE TABLE scan_history_backup AS SELECT * FROM scan_history;

-- Step 3: Disable foreign key checks temporarily
SET FOREIGN_KEY_CHECKS = 0;

-- Step 4: Delete all student records ONLY
DELETE FROM students;

-- Step 5: Delete all tokens linked to students
DELETE FROM tokens;

-- Step 6: Reset auto-increment counter for students table
ALTER TABLE students AUTO_INCREMENT = 1;
ALTER TABLE tokens AUTO_INCREMENT = 1;

-- Step 7: Re-enable foreign key checks
SET FOREIGN_KEY_CHECKS = 1;

-- Step 8: Verify deletion
SELECT 
    'AFTER RESET' as Status,
    (SELECT COUNT(*) FROM students) as Total_Students,
    (SELECT COUNT(*) FROM scan_history) as Total_Scans,
    'Scan history preserved' as Note;

-- Step 9: Show next auto-increment value
SELECT 
    'students' as Table_Name,
    AUTO_INCREMENT as Next_ID
FROM information_schema.tables
WHERE table_schema = 'student_attendance_db'
  AND table_name = 'students';

SELECT '✅ Students deleted, IDs reset, scan history preserved!' as Result;
SELECT 'ℹ️ Backup created in table: scan_history_backup' as Backup_Info;
