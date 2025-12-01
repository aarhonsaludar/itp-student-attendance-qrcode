-- Quick test to verify Solution 1 is working

USE student_attendance_db;

-- Clean test data
DELETE FROM scan_history WHERE student_id = 1 AND DATE(scan_datetime) = CURDATE();

-- Get student QR
SET @qr = (SELECT qr_code_data FROM students LIMIT 1);

-- Test 1: Time-In ONLINE
CALL sp_record_attendance_scan_secure(@qr, 1, 'Test', 'verified', FALSE, NOW(), NOW(), 0, 
    @r1, @sn1, @snum1, @st1, @ts1, @ti1, @to1);
SELECT @r1 AS TimeIn_Result, @st1 AS ScanType;

-- Test 2: Time-Out OFFLINE (ATTACK!)
CALL sp_record_attendance_scan_secure(@qr, 1, 'Test', 'offline_mode', TRUE, NOW(), NULL, NULL, 
    @r2, @sn2, @snum2, @st2, @ts2, @ti2, @to2);
SELECT @r2 AS TimeOut_Result, @st2 AS ScanType;

-- Check result
SELECT 
    time_in_validation_mode,
    time_out_validation_mode,
    status,
    notes
FROM scan_history 
WHERE student_id = 1 AND DATE(scan_datetime) = CURDATE()
ORDER BY scan_id DESC LIMIT 1;
