-- ============================================
-- Migration 006: Update vw_recent_scans to Include Validation Columns
-- ============================================
-- Purpose: Add validation-related columns to vw_recent_scans view
--          to prevent IndexOutOfRangeException when MapScanHistory reads from this view
-- Date: December 1, 2025
-- ============================================

USE student_attendance_db;

-- Drop and recreate the view with validation columns
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
    sh.time_out,
    sh.scan_purpose,
    sh.location,
    sh.status,
    sh.notes,
    sh.created_at,
    sh.validation_status,
    sh.requires_review,
    sh.client_time,
    sh.server_time,
    sh.time_drift_seconds,
    sh.time_in_validation_mode,
    sh.time_out_validation_mode,
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

-- ============================================
-- Migration completed successfully
-- ============================================
SELECT 'Migration 006 completed: Updated vw_recent_scans with validation columns' AS Status;
