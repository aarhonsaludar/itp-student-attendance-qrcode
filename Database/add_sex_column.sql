-- ============================================
-- Migration Script: Add Sex Column to Students Table
-- Description: Adds sex/gender field to existing student records
-- Date: 2025-11-21
-- ============================================

USE student_attendance_db;

-- Check if column already exists before adding
SET @dbname = DATABASE();
SET @tablename = 'students';
SET @columnname = 'sex';
SET @preparedStatement = (SELECT IF(
  (
    SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE
      (TABLE_SCHEMA = @dbname)
      AND (TABLE_NAME = @tablename)
      AND (COLUMN_NAME = @columnname)
  ) > 0,
  'SELECT ''Column already exists'' AS message;',
  'ALTER TABLE students ADD COLUMN sex ENUM(''Male'', ''Female'') DEFAULT NULL AFTER phone;'
));

PREPARE alterIfNotExists FROM @preparedStatement;
EXECUTE alterIfNotExists;
DEALLOCATE PREPARE alterIfNotExists;

-- Display confirmation message
SELECT 
    'Sex column migration completed successfully' AS status,
    COUNT(*) AS total_students,
    SUM(CASE WHEN sex IS NULL THEN 1 ELSE 0 END) AS students_without_sex,
    SUM(CASE WHEN sex = 'Male' THEN 1 ELSE 0 END) AS male_students,
    SUM(CASE WHEN sex = 'Female' THEN 1 ELSE 0 END) AS female_students
FROM students;

-- ============================================
-- End of Migration Script
-- ============================================
