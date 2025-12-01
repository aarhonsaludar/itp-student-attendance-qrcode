-- Test the stored procedure directly
USE student_attendance_db;

SET @qr_data = 'QR|ID:2300401|Name:Jaycee Aguilan|Program:Information Technology';
SET @device_id = 1;
SET @location = 'Test Location';
SET @validation_status = 'verified';
SET @requires_review = FALSE;
SET @client_time = NOW();
SET @server_time = NOW();
SET @time_drift_seconds = 0;
SET @tick_count = 12345678901234;
SET @tick_frequency = 10000000;
SET @connection_drop_count = 0;

CALL sp_record_attendance_scan_secure(
    @qr_data,
    @device_id,
    @location,
    @validation_status,
    @requires_review,
    @client_time,
    @server_time,
    @time_drift_seconds,
    @tick_count,
    @tick_frequency,
    @connection_drop_count,
    @result,
    @student_name,
    @student_number,
    @scan_type,
    @timestamp,
    @time_in,
    @time_out
);

SELECT 
    @result AS result,
    @student_name AS student_name,
    @student_number AS student_number,
    @scan_type AS scan_type,
    @timestamp AS timestamp,
    @time_in AS time_in,
    @time_out AS time_out;

-- Show the last scan record
SELECT * FROM scan_history ORDER BY scan_id DESC LIMIT 1;
