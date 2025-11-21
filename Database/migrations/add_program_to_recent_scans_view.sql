-- Update the vw_recent_scans view to include student's program
-- This script should be run in your MySQL database

USE student_attendance_db;

-- Drop existing view
DROP VIEW IF EXISTS vw_recent_scans;

-- Recreate view with student's program
CREATE VIEW vw_recent_scans AS
SELECT 
    sh.scan_id,
    s.student_number,
    CONCAT(s.first_name, ' ', s.last_name) AS student_name,
    s.program,  -- Added: Student's course/program
    sh.scan_type,
    sh.scan_datetime AS time_in,
    sh.time_out,
    sh.location,
    sh.status,
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
ORDER BY sh.scan_datetime DESC;
