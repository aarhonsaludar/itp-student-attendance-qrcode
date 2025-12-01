-- ============================================
-- Direct Database Test: WiFi Disconnect Attack Detection
-- Test Solution 1 without running the app
-- ============================================

USE student_attendance_db;

-- Step 1: Clean up any test data for today
SET @test_student_id = (SELECT student_id FROM students WHERE status = 'Active' LIMIT 1);
DELETE FROM scan_history 
WHERE student_id = @test_student_id
  AND DATE(scan_datetime) = CURDATE();

-- Step 2: Get a real student's QR code
SELECT 
    student_id,
    student_number,
    CONCAT(first_name, ' ', last_name) AS student_name,
    qr_code_data
FROM students 
WHERE status = 'Active'
LIMIT 1;

-- Step 3: Simulate TIME-IN with ONLINE validation (WiFi connected)
SET @student_qr = (SELECT qr_code_data FROM students WHERE status = 'Active' LIMIT 1);

CALL sp_record_attendance_scan_secure(
    @student_qr,           -- QR data
    1,                      -- device_id
    'Test Location',        -- location
    'verified',             -- ONLINE validation status
    FALSE,                  -- requires_review
    NOW(),                  -- client_time
    NOW(),                  -- server_time
    0,                      -- time_drift_seconds
    @result,                -- OUT: result message
    @student_name,          -- OUT: student name
    @student_number,        -- OUT: student number
    @scan_type,             -- OUT: scan type
    @timestamp,             -- OUT: timestamp
    @time_in,               -- OUT: time in
    @time_out               -- OUT: time out
);

-- Show Time-In result
SELECT '=== STEP 1: TIME-IN (ONLINE) ===' AS step;
SELECT 
    @result AS result_message,
    @scan_type AS scan_type,
    @student_number AS student,
    @time_in AS time_in_recorded;

-- Step 4: Simulate ATTACK - Student disconnects WiFi and tries TIME-OUT offline
-- Wait a moment (in real scenario, only 2-5 minutes pass)
CALL sp_record_attendance_scan_secure(
    @student_qr,           -- Same student
    1,                      -- Same device
    'Test Location',        -- Same location
    'offline_mode',         -- NOW OFFLINE (WiFi disconnected!)
    TRUE,                   -- requires_review (offline mode)
    NOW(),                  -- client_time (could be tampered)
    NULL,                   -- NO server_time (offline)
    NULL,                   -- NO time_drift (offline)
    @result2,               -- OUT: result message
    @student_name2,         -- OUT: student name
    @student_number2,       -- OUT: student number
    @scan_type2,            -- OUT: scan type
    @timestamp2,            -- OUT: timestamp
    @time_in2,              -- OUT: time in
    @time_out2              -- OUT: time out
);

-- Show Time-Out result
SELECT '=== STEP 2: TIME-OUT (OFFLINE - ATTACK!) ===' AS step;
SELECT 
    @result2 AS result_message,
    @scan_type2 AS scan_type,
    @time_out2 AS time_out_recorded;

-- Step 5: Check the final database record
SELECT '=== STEP 3: ATTACK DETECTION RESULT ===' AS step;
SELECT 
    scan_id,
    student_id,
    DATE_FORMAT(scan_datetime, '%Y-%m-%d %H:%i:%s') AS time_in,
    DATE_FORMAT(time_out, '%Y-%m-%d %H:%i:%s') AS time_out,
    TIMESTAMPDIFF(MINUTE, scan_datetime, time_out) AS duration_minutes,
    time_in_validation_mode,
    time_out_validation_mode,
    status,
    requires_review,
    validation_status,
    notes
FROM scan_history
WHERE student_id = @test_student_id
  AND DATE(scan_datetime) = CURDATE()
ORDER BY scan_id DESC
LIMIT 1;

-- Step 6: Verify if attack was detected
SELECT '=== STEP 4: VERIFICATION ===' AS step;
SELECT 
    CASE 
        WHEN time_in_validation_mode = 'online' 
         AND time_out_validation_mode = 'offline' 
         AND notes LIKE '%CRITICAL%' THEN 
            '✅ SUCCESS: Attack detected! Warning message present in notes.'
        WHEN time_in_validation_mode = 'online' 
         AND time_out_validation_mode = 'offline' THEN 
            '⚠️ PARTIAL: Validation modes tracked but warning may be missing.'
        ELSE 
            '❌ FAILED: Attack not detected!'
    END AS detection_result,
    time_in_validation_mode,
    time_out_validation_mode,
    status,
    CASE 
        WHEN status = 'for_review' THEN '✅ Flagged for review'
        ELSE '❌ NOT flagged'
    END AS review_status
FROM scan_history
WHERE student_id = @test_student_id
  AND DATE(scan_datetime) = CURDATE()
ORDER BY scan_id DESC
LIMIT 1;

-- Show the critical warning message
SELECT '=== STEP 5: WARNING MESSAGE ===' AS step;
SELECT 
    SUBSTRING(notes, 1, 500) AS warning_message
FROM scan_history
WHERE student_id = @test_student_id
  AND DATE(scan_datetime) = CURDATE()
ORDER BY scan_id DESC
LIMIT 1;
