-- ============================================
-- Migration: Add Home Address Column to Students
-- Date: November 22, 2025
-- Description: Adds home_address field to students table
-- ============================================

USE student_attendance_db;

-- Check if column already exists to prevent errors
ALTER TABLE students
ADD COLUMN home_address VARCHAR(255) AFTER section;

-- Add index for address searches (optional, for future use)
CREATE INDEX idx_home_address ON students(home_address);

-- Log migration
INSERT INTO system_logs (action, table_name, old_value, new_value)
VALUES ('ALTER', 'students', 'Added column: home_address VARCHAR(255)', 'Column added successfully');

-- Verify the migration
SELECT 
    COLUMN_NAME, 
    COLUMN_TYPE, 
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'students' 
AND TABLE_SCHEMA = 'student_attendance_db'
AND COLUMN_NAME = 'home_address';

-- ============================================
-- Migration Complete
-- ============================================
