-- ============================================
-- Migration Script: Add time_out Column to scan_history Table
-- Description: Adds time_out field to track when students leave
-- Date: 2025-11-21
-- ============================================

USE student_attendance_db;

-- Check if column already exists before adding
SET @dbname = DATABASE();
SET @tablename = 'scan_history';
SET @columnname = 'time_out';
SET @preparedStatement = (SELECT IF(
  (
    SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE
      (TABLE_SCHEMA = @dbname)
      AND (TABLE_NAME = @tablename)
      AND (COLUMN_NAME = @columnname)
  ) > 0,
  'SELECT ''Column already exists'' AS message;',
  'ALTER TABLE scan_history ADD COLUMN time_out DATETIME NULL AFTER scan_datetime;'
));

PREPARE alterIfNotExists FROM @preparedStatement;
EXECUTE alterIfNotExists;
DEALLOCATE PREPARE alterIfNotExists;

-- Display confirmation message
SELECT 
    'time_out column migration completed successfully' AS status,
    COUNT(*) AS total_scans,
    SUM(CASE WHEN time_out IS NULL THEN 1 ELSE 0 END) AS scans_without_timeout,
    SUM(CASE WHEN time_out IS NOT NULL THEN 1 ELSE 0 END) AS scans_with_timeout
FROM scan_history;

-- ============================================
-- End of Migration Script
-- ============================================
