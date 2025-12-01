-- Test Anti-Tampering with Real Student (Tick Count - 2300505)
USE student_attendance_db;

-- Student Info
SET @student_qr = 'QR|ID:2300505|Name:Tick Count|Program:Information Technology';
SET @device_id = 1;
SET @location = 'Test Location - Anti-Tampering Demo';

-- Simulated Stopwatch ticks (for .NET Framework 4.7.2)
-- Stopwatch.Frequency is typically 10,000,000 ticks per second
SET @tick_in = 50000000000; -- Starting tick count
SET @tick_out_normal = 50009000000; -- 15 minutes later (normal)
SET @tick_out_tamper = 50000120000; -- Only 2 minutes later (tampered!)

SELECT '========================================' AS '';
SELECT 'TEST 1: NORMAL ATTENDANCE (15 minutes)' AS '';
SELECT '========================================' AS '';

-- Step 1: Time In
CALL sp_record_attendance_scan_secure(
    @student_qr,
    @device_id,
    @location,
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

SELECT @result AS Result, @name AS Name, @type AS ScanType, @timestamp AS Timestamp;

-- Get the scan ID
SET @scan_id = LAST_INSERT_ID();

-- Wait a moment (simulate time passing)
SELECT SLEEP(1);

-- Step 2: Time Out (NORMAL - 15 minutes later)
UPDATE scan_history
SET 
    time_out = DATE_ADD(scan_datetime, INTERVAL 15 MINUTE),
    time_out_tick_count = @tick_out_normal,
    time_out_validation_mode = 'verified',
    connection_drop_count = 0
WHERE scan_id = @scan_id;

-- Check the result
SELECT 
    '✅ NORMAL ATTENDANCE TEST' AS TestResult,
    s.student_number,
    DATE_FORMAT(sh.scan_datetime, '%H:%i:%s') AS time_in,
    DATE_FORMAT(sh.time_out, '%H:%i:%s') AS time_out,
    TIMESTAMPDIFF(MINUTE, sh.scan_datetime, sh.time_out) AS claimed_minutes,
    ROUND((sh.time_out_tick_count - sh.time_in_tick_count) / 10000000 / 60, 1) AS actual_minutes,
    ABS(TIMESTAMPDIFF(MINUTE, sh.scan_datetime, sh.time_out) - 
        ROUND((sh.time_out_tick_count - sh.time_in_tick_count) / 10000000 / 60, 1)) AS difference,
    CASE
        WHEN ABS(TIMESTAMPDIFF(MINUTE, sh.scan_datetime, sh.time_out) - 
             ROUND((sh.time_out_tick_count - sh.time_in_tick_count) / 10000000 / 60, 1)) > 3
        THEN '🚨 TAMPERING DETECTED'
        ELSE '✅ Valid'
    END AS detection_status
FROM scan_history sh
JOIN students s ON sh.student_id = s.student_id
WHERE sh.scan_id = @scan_id;

-- Clean up for next test
DELETE FROM scan_history WHERE scan_id = @scan_id;

SELECT '' AS '';
SELECT '========================================' AS '';
SELECT 'TEST 2: TIME TAMPERING (Claims 6 hours, Actually 2 minutes)' AS '';
SELECT '========================================' AS '';

-- Step 1: Time In
CALL sp_record_attendance_scan_secure(
    @student_qr,
    @device_id,
    @location,
    'verified',
    FALSE,
    NOW(),
    NOW(),
    0,
    @tick_in,
    0,
    @result2,
    @name2,
    @number2,
    @type2,
    @timestamp2,
    @time_in2,
    @time_out2
);

SELECT @result2 AS Result, @name2 AS Name, @type2 AS ScanType;

-- Get the scan ID
SET @scan_id2 = LAST_INSERT_ID();

-- Step 2: Time Out with TAMPERING
-- User changed system clock to show 6 hours later
-- But only 2 minutes actually passed (TickCount shows this!)
UPDATE scan_history
SET 
    time_out = DATE_ADD(scan_datetime, INTERVAL 6 HOUR),  -- FAKE: Clock shows 6 hours
    time_out_tick_count = @tick_out_tamper,                -- REAL: Only 2 min (12,000 ms)
    time_out_validation_mode = 'offline',
    connection_drop_count = 1,
    offline_duration_minutes = ROUND((@tick_out_tamper - @tick_in) / 10000000 / 60, 1)
WHERE scan_id = @scan_id2;

-- Check tampering detection
SELECT 
    '🚨 TAMPERING TEST' AS TestResult,
    s.student_number,
    DATE_FORMAT(sh.scan_datetime, '%H:%i:%s') AS time_in,
    DATE_FORMAT(sh.time_out, '%H:%i:%s') AS time_out,
    TIMESTAMPDIFF(MINUTE, sh.scan_datetime, sh.time_out) AS claimed_minutes,
    ROUND((sh.time_out_tick_count - sh.time_in_tick_count) / 10000000 / 60, 1) AS actual_minutes,
    ABS(TIMESTAMPDIFF(MINUTE, sh.scan_datetime, sh.time_out) - 
        ROUND((sh.time_out_tick_count - sh.time_in_tick_count) / 10000000 / 60, 1)) AS difference,
    CASE
        WHEN ABS(TIMESTAMPDIFF(MINUTE, sh.scan_datetime, sh.time_out) - 
             ROUND((sh.time_out_tick_count - sh.time_in_tick_count) / 10000000 / 60, 1)) > 3
        THEN '🚨 TAMPERING DETECTED!'
        ELSE '✅ Valid'
    END AS detection_status,
    sh.status,
    sh.notes
FROM scan_history sh
JOIN students s ON sh.student_id = s.student_id
WHERE sh.scan_id = @scan_id2;

SELECT '' AS '';
SELECT '========================================' AS '';
SELECT 'Summary: Anti-Tampering System Test Results' AS '';
SELECT '========================================' AS '';
SELECT 'Test 1 (Normal 15 min): Should show ✅ Valid' AS Result;
SELECT 'Test 2 (Tampered 6hr claim, 2min actual): Should show 🚨 TAMPERING DETECTED!' AS Result;
SELECT '' AS '';
SELECT '✅ If both tests passed, the anti-tampering system is working!' AS Result;

-- Clean up test data
-- DELETE FROM scan_history WHERE scan_id IN (@scan_id, @scan_id2);
