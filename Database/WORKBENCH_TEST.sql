-- ============================================
-- SIMPLE COPY-PASTE TEST FOR MySQL WORKBENCH
-- Instructions: 
-- 1. Open MySQL Workbench
-- 2. Connect to student_attendance_db
-- 3. Copy and paste this entire script
-- 4. Click Execute (⚡ icon) or press Ctrl+Shift+Enter
-- ============================================

USE student_attendance_db;

-- Clean up test data
DELETE FROM scan_history WHERE DATE(scan_datetime) = CURDATE();

-- Get first student
SET @qr = (SELECT qr_code_data FROM students LIMIT 1);
SET @student_name = (SELECT CONCAT(first_name, ' ', last_name) FROM students LIMIT 1);

SELECT CONCAT('Testing with student: ', @student_name) AS 'TEST START';

-- ====================
-- ATTACK SCENARIO TEST
-- ====================

-- 1. TIME-IN with WiFi ON (ONLINE)
CALL sp_record_attendance_scan_secure(
    @qr, 1, 'Test', 'verified', FALSE, NOW(), NOW(), 0,
    @r1, @sn1, @snum1, @st1, @ts1, @ti1, @to1
);

SELECT 
    '1️⃣ TIME-IN (WiFi ON)' AS step,
    @r1 AS result,
    @st1 AS type;

-- 2. TIME-OUT with WiFi OFF (OFFLINE) - ATTACK!
CALL sp_record_attendance_scan_secure(
    @qr, 1, 'Test', 'offline_mode', TRUE, NOW(), NULL, NULL,
    @r2, @sn2, @snum2, @st2, @ts2, @ti2, @to2
);

SELECT 
    '2️⃣ TIME-OUT (WiFi OFF - ATTACK!)' AS step,
    @r2 AS result,
    @st2 AS type;

-- 3. CHECK RESULT
SELECT 
    '3️⃣ DETECTION RESULT' AS step,
    time_in_validation_mode AS 'Time-In Mode',
    time_out_validation_mode AS 'Time-Out Mode',
    status AS 'Status',
    CASE 
        WHEN time_in_validation_mode = 'online' 
         AND time_out_validation_mode = 'offline' 
         AND notes LIKE '%CRITICAL%' 
        THEN '✅ ATTACK DETECTED!'
        ELSE '❌ NOT DETECTED'
    END AS 'Detection',
    notes AS 'Warning Message'
FROM scan_history
WHERE DATE(scan_datetime) = CURDATE()
ORDER BY scan_id DESC
LIMIT 1;

-- 4. SHOW FULL NOTES
SELECT 
    '4️⃣ FULL WARNING MESSAGE' AS step,
    notes
FROM scan_history
WHERE DATE(scan_datetime) = CURDATE()
ORDER BY scan_id DESC
LIMIT 1;
