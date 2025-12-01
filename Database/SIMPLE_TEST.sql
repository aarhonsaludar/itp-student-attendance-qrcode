-- ============================================
-- EASIEST TEST - Just run this in MySQL Workbench
-- Copy everything and paste in SQL editor, then Execute
-- ============================================

USE student_attendance_db;

-- Show current students
SELECT '=== YOUR STUDENTS ===' AS info;
SELECT student_id, student_number, CONCAT(first_name, ' ', last_name) AS name 
FROM students LIMIT 5;

-- Clean today's test data
DELETE FROM scan_history WHERE DATE(scan_datetime) = CURDATE();

-- Get any active student
SET @sid = (SELECT student_id FROM students WHERE status = 'Active' LIMIT 1);
SET @snum = (SELECT student_number FROM students WHERE status = 'Active' LIMIT 1);
SET @sname = (SELECT CONCAT(first_name, ' ', last_name) FROM students WHERE status = 'Active' LIMIT 1);

SELECT CONCAT('Testing with: ', @sname, ' (', @snum, ')') AS 'Using Student';

-- Manual insert: TIME-IN ONLINE
INSERT INTO scan_history (
    student_id, device_id, scan_type, scan_data, scan_datetime, 
    time_out, scan_purpose, location, status, notes,
    validation_status, time_in_validation_mode, time_out_validation_mode,
    requires_review, created_at
) VALUES (
    @sid, 1, 'QR', 'TEST', NOW(),
    NULL, 'attendance', 'Test', 'success', NULL,
    'verified', 'online', NULL,
    FALSE, NOW()
);

SELECT '✅ TIME-IN created (ONLINE)' AS step;

-- Get the scan_id
SET @scan_id = LAST_INSERT_ID();

-- Manual update: TIME-OUT OFFLINE (ATTACK!)
UPDATE scan_history
SET 
    time_out = NOW(),
    time_out_validation_mode = 'offline',
    status = 'for_review',
    notes = '🚨 CRITICAL: Time-in was ONLINE but time-out is OFFLINE - Possible WiFi disconnect + time tampering',
    requires_review = TRUE
WHERE scan_id = @scan_id;

SELECT '✅ TIME-OUT updated (OFFLINE - ATTACK!)' AS step;

-- CHECK RESULT
SELECT 
    '=== 🔍 ATTACK DETECTION RESULT ===' AS result_title,
    scan_id,
    DATE_FORMAT(scan_datetime, '%H:%i:%s') AS time_in,
    DATE_FORMAT(time_out, '%H:%i:%s') AS time_out,
    time_in_validation_mode,
    time_out_validation_mode,
    status,
    requires_review,
    CASE 
        WHEN time_in_validation_mode = 'online' 
         AND time_out_validation_mode = 'offline' 
        THEN '✅ ATTACK DETECTED!'
        ELSE '❌ NOT DETECTED'
    END AS detection_status
FROM scan_history
WHERE scan_id = @scan_id;

-- SHOW WARNING
SELECT 
    '=== ⚠️ WARNING MESSAGE ===' AS warning_title,
    notes AS full_warning_message
FROM scan_history
WHERE scan_id = @scan_id;

SELECT '=== ✅ TEST COMPLETE ===' AS final_message;
