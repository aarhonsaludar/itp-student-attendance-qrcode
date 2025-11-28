-- ============================================
-- Reset Students Database
-- This script will:
-- 1. Delete all student records
-- 2. Delete all scan history related to students
-- 3. Reset auto-increment IDs to start from 1
-- ============================================

USE student_attendance_db;

-- Step 1: Show current counts (before deletion)
SELECT 
    'BEFORE RESET' as Status,
    (SELECT COUNT(*) FROM students) as Total_Students,
    (SELECT COUNT(*) FROM scan_history) as Total_Scans;

-- Step 2: Disable foreign key checks temporarily
SET FOREIGN_KEY_CHECKS = 0;

-- Step 3: Delete all scan history records
-- (This must be done first if there are foreign key constraints)
DELETE FROM scan_history;

-- Step 4: Delete all student records
DELETE FROM students;

-- Step 5: Delete all tokens (if any are linked to students)
DELETE FROM tokens;

-- Step 6: Reset auto-increment counters to start from 1
ALTER TABLE students AUTO_INCREMENT = 1;
ALTER TABLE scan_history AUTO_INCREMENT = 1;
ALTER TABLE tokens AUTO_INCREMENT = 1;

-- Step 7: Re-enable foreign key checks
SET FOREIGN_KEY_CHECKS = 1;

-- Step 8: Verify deletion (should show 0 records)
SELECT 
    'AFTER RESET' as Status,
    (SELECT COUNT(*) FROM students) as Total_Students,
    (SELECT COUNT(*) FROM scan_history) as Total_Scans,
    (SELECT COUNT(*) FROM tokens) as Total_Tokens;

-- Step 9: Show next auto-increment values
SELECT 
    'students' as Table_Name,
    AUTO_INCREMENT as Next_ID
FROM information_schema.tables
WHERE table_schema = 'student_attendance_db'
  AND table_name = 'students'
UNION ALL
SELECT 
    'scan_history' as Table_Name,
    AUTO_INCREMENT as Next_ID
FROM information_schema.tables
WHERE table_schema = 'student_attendance_db'
  AND table_name = 'scan_history';

SELECT '✅ All student data has been deleted and IDs have been reset!' as Result;
