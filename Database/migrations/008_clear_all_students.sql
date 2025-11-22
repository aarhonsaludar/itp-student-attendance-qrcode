-- ============================================
-- Script: Clear All Student Records
-- Date: November 22, 2025
-- Description: Removes all student data while preserving database structure
-- ============================================

USE student_attendance_db;

-- IMPORTANT: This will delete ALL student records!
-- Uncomment the lines below to execute

-- Step 1: Delete all scan history records (dependent on students)
DELETE FROM scan_history;

-- Step 2: Delete all tokens (dependent on students)
DELETE FROM tokens;

-- Step 3: Delete all students
DELETE FROM students;

-- Step 4: Reset auto-increment counters
ALTER TABLE scan_history AUTO_INCREMENT = 1;
ALTER TABLE tokens AUTO_INCREMENT = 1;
ALTER TABLE students AUTO_INCREMENT = 1;

-- Step 5: Verify deletion
SELECT 'Verification Results:' as status;
SELECT COUNT(*) as total_students FROM students;
SELECT COUNT(*) as total_scans FROM scan_history;
SELECT COUNT(*) as total_tokens FROM tokens;

-- Log this action
INSERT INTO system_logs (action, table_name, old_value, new_value)
VALUES ('DELETE', 'students', 'All student records deleted', 'Database reset for testing');

-- ============================================
-- DONE - All student records cleared
-- ============================================
