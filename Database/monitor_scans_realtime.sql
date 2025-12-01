-- Real-time monitoring query for anti-tampering testing
-- Run this in MySQL Workbench or command line while testing

USE student_attendance_db;

-- Show the most recent 10 scans with tampering analysis
SELECT 
    scan_id,
    student_number,
    DATE_FORMAT(scan_datetime, '%H:%i:%s') AS time_in,
    DATE_FORMAT(time_out, '%H:%i:%s') AS time_out,
    
    -- Claimed duration (what the clock shows)
    CASE 
        WHEN time_out IS NOT NULL 
        THEN TIMESTAMPDIFF(MINUTE, scan_datetime, time_out)
        ELSE NULL
    END AS claimed_min,
    
    -- Actual duration (from TickCount - tamper-proof!)
    CASE 
        WHEN time_in_tick_count IS NOT NULL AND time_out_tick_count IS NOT NULL
        THEN ROUND((time_out_tick_count - time_in_tick_count) / (SELECT @@global.slow_query_log * 0 + 10000000), 1)
        ELSE NULL
    END AS actual_min_approx,
    
    -- Tampering detection
    CASE
        WHEN time_out IS NULL THEN '⏳ Pending Time Out'
        WHEN time_in_tick_count IS NULL OR time_out_tick_count IS NULL THEN 'ℹ️ No TickCount (old record)'
        WHEN ABS(
            TIMESTAMPDIFF(SECOND, scan_datetime, time_out) - 
            ((time_out_tick_count - time_in_tick_count) / (SELECT @@global.slow_query_log * 0 + 10000000))
        ) > 180 -- 3 minutes tolerance in seconds
        THEN '🚨 TAMPERING DETECTED!'
        ELSE '✅ Valid'
    END AS detection_status,
    
    -- Additional info
    status,
    time_in_validation_mode AS in_mode,
    time_out_validation_mode AS out_mode,
    connection_drop_count AS drops,
    
    -- Show TickCount values (for debugging)
    time_in_tick_count,
    time_out_tick_count
    
FROM scan_history
WHERE DATE(scan_datetime) = CURDATE()
ORDER BY scan_datetime DESC
LIMIT 10;

-- Summary statistics
SELECT 
    COUNT(*) AS total_scans_today,
    SUM(CASE WHEN time_out IS NOT NULL THEN 1 ELSE 0 END) AS completed,
    SUM(CASE WHEN time_out IS NULL THEN 1 ELSE 0 END) AS pending,
    SUM(CASE WHEN status = 'for_review' THEN 1 ELSE 0 END) AS flagged_for_review
FROM scan_history
WHERE DATE(scan_datetime) = CURDATE();
