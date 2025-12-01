-- Migration 004: Exclude for_review scans from recent scans view
-- This ensures that offline/for_review scans don't appear in Recent Scan Activity
-- until they are approved through the ScanDetailsDialog

-- Drop and recreate the view with the filter
DROP VIEW IF EXISTS vw_recent_scans;

CREATE VIEW vw_recent_scans AS
SELECT 
    sh.scan_id,
    sh.student_id,
    sh.device_id,
    s.student_number,
    CONCAT(s.first_name, ' ', s.last_name) AS student_name,
    s.program,
    sh.scan_type,
    sh.scan_data,
    sh.scan_datetime,
    sh.scan_datetime AS time_in,
    sh.time_out,
    sh.scan_purpose,
    sh.location,
    sh.status,
    sh.notes,
    sh.created_at,
    d.device_name,
    CASE 
        WHEN sh.time_out IS NOT NULL THEN 'completed'
        WHEN sh.time_out IS NULL AND sh.scan_datetime >= CURDATE() THEN 'pending_out'
        ELSE 'incomplete'
    END AS attendance_status
FROM scan_history sh
JOIN students s ON sh.student_id = s.student_id
LEFT JOIN devices d ON sh.device_id = d.device_id
WHERE sh.scan_datetime >= DATE_SUB(NOW(), INTERVAL 24 HOUR)
    AND sh.status != 'for_review'
ORDER BY sh.scan_datetime DESC;
