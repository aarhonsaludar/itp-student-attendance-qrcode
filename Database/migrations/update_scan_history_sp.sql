-- ============================================
-- Update sp_get_scan_history to include time_out
-- ============================================

USE student_attendance_db;

-- Drop and recreate the stored procedure
DROP PROCEDURE IF EXISTS sp_get_scan_history;

DELIMITER //

CREATE PROCEDURE sp_get_scan_history(
    IN p_start_date DATE,
    IN p_end_date DATE,
    IN p_student_id INT,
    IN p_limit INT,
    IN p_offset INT
)
BEGIN
    SELECT 
        sh.scan_id,
        s.student_number,
        CONCAT(s.first_name, ' ', s.last_name) AS student_name,
        sh.scan_type,
        sh.scan_datetime,
        sh.time_out,
        sh.location,
        sh.status,
        d.device_name
    FROM scan_history sh
    JOIN students s ON sh.student_id = s.student_id
    LEFT JOIN devices d ON sh.device_id = d.device_id
    WHERE (p_start_date IS NULL OR DATE(sh.scan_datetime) >= p_start_date)
      AND (p_end_date IS NULL OR DATE(sh.scan_datetime) <= p_end_date)
      AND (p_student_id IS NULL OR sh.student_id = p_student_id)
    ORDER BY sh.scan_datetime DESC
    LIMIT p_limit OFFSET p_offset;
END //

DELIMITER ;

SELECT 'Stored procedure sp_get_scan_history updated successfully with time_out column!' as Status;
