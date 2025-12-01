-- ============================================
-- Test Anti-Tampering TickCount System
-- ============================================

USE student_attendance_db;

-- Verify new columns exist
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE,
    COLUMN_COMMENT
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_SCHEMA = 'student_attendance_db'
  AND TABLE_NAME = 'scan_history'
  AND COLUMN_NAME IN (
      'time_in_tick_count',
      'time_out_tick_count',
      'connection_drop_count',
      'offline_duration_minutes'
  )
ORDER BY ORDINAL_POSITION;

-- Show stored procedure signature
SHOW CREATE PROCEDURE sp_record_attendance_scan_secure;

-- Test data: Simulate tampering scenario
-- Scenario: Student tries to fake 6 hours (360 min) but only 2 minutes passed

-- Step 1: Time In (normal)
SET @student_qr = 'QR|ID:2021-00123|Name:Test Student|Program:BSIT';
SET @tick_in = 10000000; -- Simulated TickCount at Time In

CALL sp_record_attendance_scan_secure(
    @student_qr,
    1,
    'Test Location',
    'verified',
    FALSE,
    NOW(),
    NOW(),
    0,
    @tick_in,
    0,
    @result,
    @name,
    @number,
    @type,
    @timestamp,
    @time_in,
    @time_out
);

SELECT @result AS Result, @name AS Name, @type AS Type;

-- Get the scan ID
SET @scan_id = LAST_INSERT_ID();

-- Step 2: Time Out with TAMPERING (2 minutes real time, but clock shows 6 hours later)
-- Real TickCount: only 120,000 ms passed (2 minutes)
-- System Clock: Shows 6 hours later due to tampering
SET @tick_out = 10120000; -- Only 120,000 ms = 2 minutes passed (REAL TIME)
SET @fake_time_out = DATE_ADD(NOW(), INTERVAL 6 HOUR); -- FAKE: 6 hours later

-- Manually update to simulate tampering
UPDATE scan_history
SET 
    time_out = @fake_time_out,
    time_out_tick_count = @tick_out,
    time_out_validation_mode = 'offline',
    connection_drop_count = 1,
    offline_duration_minutes = ((@tick_out - @tick_in) / 60000.0)
WHERE scan_id = @scan_id;

-- Verify the detection
SELECT 
    scan_id,
    student_number,
    scan_datetime AS time_in,
    time_out,
    TIMESTAMPDIFF(MINUTE, scan_datetime, time_out) AS claimed_duration_minutes,
    ((time_out_tick_count - time_in_tick_count) / 60000.0) AS actual_duration_minutes,
    ABS(TIMESTAMPDIFF(MINUTE, scan_datetime, time_out) - ((time_out_tick_count - time_in_tick_count) / 60000.0)) AS difference_minutes,
    CASE
        WHEN ABS(TIMESTAMPDIFF(MINUTE, scan_datetime, time_out) - ((time_out_tick_count - time_in_tick_count) / 60000.0)) > 3
        THEN '🚨 TIME TAMPERING DETECTED!'
        ELSE '✅ Valid'
    END AS detection_result,
    time_in_tick_count,
    time_out_tick_count,
    connection_drop_count,
    offline_duration_minutes
FROM scan_history
WHERE scan_id = @scan_id;

-- Show all recent scans with tampering analysis
SELECT 
    scan_id,
    student_number,
    DATE_FORMAT(scan_datetime, '%Y-%m-%d %H:%i:%s') AS time_in,
    DATE_FORMAT(time_out, '%Y-%m-%d %H:%i:%s') AS time_out,
    TIMESTAMPDIFF(MINUTE, scan_datetime, time_out) AS claimed_minutes,
    ROUND((time_out_tick_count - time_in_tick_count) / 60000.0, 1) AS actual_minutes,
    CASE
        WHEN time_out_tick_count IS NULL OR time_in_tick_count IS NULL THEN 'N/A (Pending Time Out)'
        WHEN ABS(TIMESTAMPDIFF(MINUTE, scan_datetime, time_out) - ((time_out_tick_count - time_in_tick_count) / 60000.0)) > 3
        THEN '🚨 TAMPERING DETECTED'
        ELSE '✅ Valid'
    END AS status,
    connection_drop_count AS drops,
    ROUND(offline_duration_minutes, 1) AS offline_min
FROM scan_history
WHERE DATE(scan_datetime) = CURDATE()
ORDER BY scan_datetime DESC
LIMIT 10;

-- Cleanup test data
-- DELETE FROM scan_history WHERE student_number = '2021-00123' AND DATE(scan_datetime) = CURDATE();
