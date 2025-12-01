-- ============================================
-- Test Script: Solution 1 - WiFi Disconnect Detection
-- Tests the online→offline validation mode mismatch
-- ============================================

USE student_attendance_db;

-- Clean up any existing test data for today
DELETE FROM scan_history 
WHERE student_id = 1 
  AND DATE(scan_datetime) = CURDATE();

DELIMITER //

-- ============================================
-- TEST 1: Normal Online Attendance (should pass)
-- ============================================
CREATE PROCEDURE test_normal_online()
BEGIN
    DECLARE v_result VARCHAR(200);
    DECLARE v_student_name VARCHAR(200);
    DECLARE v_student_number VARCHAR(50);
    DECLARE v_scan_type VARCHAR(20);
    DECLARE v_timestamp DATETIME;
    DECLARE v_time_in DATETIME;
    DECLARE v_time_out DATETIME;
    
    -- Get first student's QR code
    SET @qr_data = (SELECT qr_code_data FROM students WHERE student_id = 1 LIMIT 1);
    
    -- Simulate TIME-IN with ONLINE validation
    CALL sp_record_attendance_scan_secure(
        @qr_data,
        1,  -- device_id
        'Test Location',
        'verified',  -- ONLINE validation
        FALSE,  -- requires_review
        NOW(),  -- client_time
        NOW(),  -- server_time
        0,  -- time_drift_seconds
        v_result,
        v_student_name,
        v_student_number,
        v_scan_type,
        v_timestamp,
        v_time_in,
        v_time_out
    );
    
    SELECT 
        '✅ TEST 1: Normal Online Time-In' AS test_name,
        v_result AS result,
        v_scan_type AS scan_type,
        v_student_number AS student;
    
    -- Wait 2 minutes simulation (in real scenario, student waits)
    -- Then simulate TIME-OUT with ONLINE validation
    CALL sp_record_attendance_scan_secure(
        @qr_data,
        1,
        'Test Location',
        'verified',  -- STILL ONLINE
        FALSE,
        NOW(),
        NOW(),
        0,
        v_result,
        v_student_name,
        v_student_number,
        v_scan_type,
        v_timestamp,
        v_time_in,
        v_time_out
    );
    
    SELECT 
        '✅ TEST 1: Normal Online Time-Out' AS test_name,
        v_result AS result,
        v_scan_type AS scan_type;
    
    -- Check the database record
    SELECT 
        'Final Result' AS stage,
        time_in_validation_mode,
        time_out_validation_mode,
        status,
        requires_review,
        SUBSTRING(notes, 1, 100) AS notes_preview
    FROM scan_history
    WHERE student_id = 1
      AND DATE(scan_datetime) = CURDATE()
    ORDER BY scan_id DESC
    LIMIT 1;
    
END //

-- ============================================
-- TEST 2: WiFi Disconnect Attack (should detect!)
-- ============================================
CREATE PROCEDURE test_wifi_disconnect_attack()
BEGIN
    DECLARE v_result VARCHAR(200);
    DECLARE v_student_name VARCHAR(200);
    DECLARE v_student_number VARCHAR(50);
    DECLARE v_scan_type VARCHAR(20);
    DECLARE v_timestamp DATETIME;
    DECLARE v_time_in DATETIME;
    DECLARE v_time_out DATETIME;
    
    -- Clean up
    DELETE FROM scan_history 
    WHERE student_id = 1 
      AND DATE(scan_datetime) = CURDATE();
    
    -- Get first student's QR code
    SET @qr_data = (SELECT qr_code_data FROM students WHERE student_id = 1 LIMIT 1);
    
    -- Simulate TIME-IN with ONLINE validation (WiFi connected)
    CALL sp_record_attendance_scan_secure(
        @qr_data,
        1,
        'Test Location',
        'verified',  -- ONLINE
        FALSE,
        NOW(),
        NOW(),
        0,
        v_result,
        v_student_name,
        v_student_number,
        v_scan_type,
        v_timestamp,
        v_time_in,
        v_time_out
    );
    
    SELECT 
        '🔴 TEST 2: Attack Scenario - Time-In ONLINE' AS test_name,
        v_result AS result,
        v_scan_type AS scan_type;
    
    -- ATTACK: Student disconnects WiFi and changes time
    -- Then tries to TIME-OUT in OFFLINE mode
    CALL sp_record_attendance_scan_secure(
        @qr_data,
        1,
        'Test Location',
        'offline_mode',  -- NOW OFFLINE (WiFi disconnected!)
        TRUE,  -- will be flagged for review
        NOW(),
        NULL,  -- no server time (offline)
        NULL,  -- no drift calculation (offline)
        v_result,
        v_student_name,
        v_student_number,
        v_scan_type,
        v_timestamp,
        v_time_in,
        v_time_out
    );
    
    SELECT 
        '🔴 TEST 2: Attack Scenario - Time-Out OFFLINE' AS test_name,
        v_result AS result,
        v_scan_type AS scan_type;
    
    -- Check if attack was detected
    SELECT 
        '🚨 ATTACK DETECTION RESULT' AS stage,
        time_in_validation_mode AS time_in_mode,
        time_out_validation_mode AS time_out_mode,
        status,
        requires_review,
        notes
    FROM scan_history
    WHERE student_id = 1
      AND DATE(scan_datetime) = CURDATE()
    ORDER BY scan_id DESC
    LIMIT 1;
    
    -- Verify the critical warning is present
    SELECT 
        CASE 
            WHEN notes LIKE '%CRITICAL%online%offline%' THEN '✅ ATTACK DETECTED! Warning message present'
            WHEN time_in_validation_mode = 'online' AND time_out_validation_mode = 'offline' THEN '⚠️ Validation modes tracked but warning missing'
            ELSE '❌ ATTACK NOT DETECTED - System failed!'
        END AS detection_status
    FROM scan_history
    WHERE student_id = 1
      AND DATE(scan_datetime) = CURDATE()
    ORDER BY scan_id DESC
    LIMIT 1;
    
END //

DELIMITER ;

-- ============================================
-- Run the tests
-- ============================================
SELECT '================================================' AS separator;
SELECT 'STARTING SOLUTION 1 DETECTION TESTS' AS test_suite;
SELECT '================================================' AS separator;

-- Test 1: Normal online attendance
CALL test_normal_online();

SELECT '' AS separator;
SELECT '================================================' AS separator;

-- Test 2: WiFi disconnect attack
CALL test_wifi_disconnect_attack();

SELECT '' AS separator;
SELECT '================================================' AS separator;
SELECT 'TESTS COMPLETED' AS status;
SELECT '================================================' AS separator;

-- Cleanup
DROP PROCEDURE IF EXISTS test_normal_online;
DROP PROCEDURE IF EXISTS test_wifi_disconnect_attack;
