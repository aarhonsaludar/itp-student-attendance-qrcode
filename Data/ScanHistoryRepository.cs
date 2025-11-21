using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using MySqlConnector;
using ITP104_FINAL_PROJECT.Models;
using ITP104_FINAL_PROJECT.Services;

namespace ITP104_FINAL_PROJECT.Data
{
    public class ScanHistoryRepository
    {
        /// <summary>
        /// Record a QR scan with Time In/Time Out logic using new stored procedure
        /// </summary>
        public async Task<(bool success, string message, string scanType)> RecordAttendanceScanAsync(
            string qrData,
            int deviceId,
            string location = null)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(qrData))
                {
                    return (false, "QR code data cannot be empty", "ERROR");
                }

                if (deviceId <= 0)
                {
                    return (false, "Invalid device ID", "ERROR");
                }

                using (var connection = await DatabaseHelper.GetConnectionWithRetryAsync())
                {
                    using (var command = new MySqlCommand("sp_record_attendance_scan", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = 60; // Increase timeout for stored procedure

                        // Input parameters
                        command.Parameters.AddWithValue("@p_scan_data", qrData);
                        command.Parameters.AddWithValue("@p_device_id", deviceId);
                        command.Parameters.AddWithValue("@p_location", location ?? (object)DBNull.Value);

                        // Output parameters
                        var resultParam = new MySqlParameter("@p_result", MySqlDbType.VarChar, 200) { Direction = ParameterDirection.Output };
                        var studentNameParam = new MySqlParameter("@p_student_name", MySqlDbType.VarChar, 200) { Direction = ParameterDirection.Output };
                        var studentNumberParam = new MySqlParameter("@p_student_number", MySqlDbType.VarChar, 50) { Direction = ParameterDirection.Output };
                        var scanTypeParam = new MySqlParameter("@p_scan_type", MySqlDbType.VarChar, 20) { Direction = ParameterDirection.Output };

                        command.Parameters.Add(resultParam);
                        command.Parameters.Add(studentNameParam);
                        command.Parameters.Add(studentNumberParam);
                        command.Parameters.Add(scanTypeParam);

                        await command.ExecuteNonQueryAsync();

                        string result = resultParam.Value?.ToString() ?? "Unknown error";
                        string studentName = studentNameParam.Value?.ToString();
                        string studentNumber = studentNumberParam.Value?.ToString();
                        string scanType = scanTypeParam.Value?.ToString() ?? "ERROR";

                        // Determine success based on result message
                        bool success = result.StartsWith("SUCCESS");

                        // Format message to include student info if available
                        string message = result;
                        if (!string.IsNullOrEmpty(studentName) && !string.IsNullOrEmpty(studentNumber))
                        {
                            message = $"{studentName} ({studentNumber})\n{result}";
                        }

                        // Log the scan attempt
                        if (success)
                        {
                            await ErrorLoggingService.LogInfoAsync(
                                $"Attendance Scan - {scanType}",
                                $"Student: {studentNumber} - {studentName}",
                                "scan_history");
                        }
                        else
                        {
                            await ErrorLoggingService.LogInfoAsync(
                                "Attendance Scan - Failed",
                                $"QR Data: {qrData.Substring(0, Math.Min(50, qrData.Length))}... Result: {result}",
                                "scan_history");
                        }

                        return (success, message, scanType);
                    }
                }
            }
            catch (MySqlException ex)
            {
                await ErrorLoggingService.LogErrorAsync(
                    "Record Attendance Scan - Database Error",
                    ex,
                    "scan_history");
                return (false, ErrorLoggingService.GetUserFriendlyMessage(ex), "ERROR");
            }
            catch (Exception ex)
            {
                await ErrorLoggingService.LogErrorAsync(
                    "Record Attendance Scan - Error",
                    ex,
                    "scan_history");
                return (false, $"An error occurred while recording scan: {ex.Message}", "ERROR");
            }
        }

        /// <summary>
        /// Record a QR scan using stored procedure with duplicate detection (OLD METHOD - kept for compatibility)
        /// </summary>
        public async Task<(bool success, string message, int scanId)> RecordScanAsync(int studentId, int deviceId, string scanData, string scanPurpose = "attendance", string location = null, string notes = null)
        {
            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    await connection.OpenAsync();
                    using (var command = new MySqlCommand("sp_record_scan", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Input parameters - matching stored procedure signature
                        command.Parameters.AddWithValue("@p_scan_data", scanData);
                        command.Parameters.AddWithValue("@p_device_id", deviceId);
                        command.Parameters.AddWithValue("@p_location", location ?? (object)DBNull.Value);

                        // Output parameters - matching stored procedure signature
                        var resultParam = new MySqlParameter("@p_result", MySqlDbType.VarChar, 100) { Direction = ParameterDirection.Output };
                        var studentNameParam = new MySqlParameter("@p_student_name", MySqlDbType.VarChar, 200) { Direction = ParameterDirection.Output };
                        var studentNumberParam = new MySqlParameter("@p_student_number", MySqlDbType.VarChar, 50) { Direction = ParameterDirection.Output };

                        command.Parameters.Add(resultParam);
                        command.Parameters.Add(studentNameParam);
                        command.Parameters.Add(studentNumberParam);

                        await command.ExecuteNonQueryAsync();

                        string result = resultParam.Value?.ToString() ?? "Unknown error";
                        string studentName = studentNameParam.Value?.ToString();
                        string studentNumber = studentNumberParam.Value?.ToString();

                        // Determine success based on result message
                        bool success = result.Contains("SUCCESS") || result.Contains("WARNING");
                        string message = result;

                        if (success && !string.IsNullOrEmpty(studentName))
                        {
                            message = $"{result} - {studentName} ({studentNumber})";
                        }

                        return (success, message, 0); // scanId not returned by this stored procedure
                    }
                }
            }
            catch (Exception ex)
            {
                return (false, $"Database error: {ex.Message}", 0);
            }
        }

        /// <summary>
        /// Get scan history for a specific student
        /// </summary>
        public async Task<List<ScanHistory>> GetStudentScansAsync(int studentId)
        {
            return await GetHistoryAsync(studentId: studentId);
        }

        /// <summary>
        /// Get scan history with filters using stored procedure
        /// </summary>
        public async Task<List<ScanHistory>> GetHistoryAsync(DateTime? startDate = null, DateTime? endDate = null, int? studentId = null, string scanType = null)
        {
            var scans = new List<ScanHistory>();

            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    await connection.OpenAsync();

                    // Use direct query instead of stored procedure to ensure correct filtering and avoid duplicates
                    var sqlBuilder = new System.Text.StringBuilder();
                    sqlBuilder.Append(@"SELECT sh.*, s.student_number, 
                                    CONCAT(s.first_name, ' ', s.last_name) as student_name,
                                    d.device_name
                                    FROM scan_history sh
                                    LEFT JOIN students s ON sh.student_id = s.student_id
                                    LEFT JOIN devices d ON sh.device_id = d.device_id
                                    WHERE 1=1");

                    if (startDate.HasValue)
                        sqlBuilder.Append(" AND sh.scan_datetime >= @startDate");

                    if (endDate.HasValue)
                        sqlBuilder.Append(" AND sh.scan_datetime <= @endDate");

                    if (studentId.HasValue)
                        sqlBuilder.Append(" AND sh.student_id = @studentId");

                    if (!string.IsNullOrEmpty(scanType))
                        sqlBuilder.Append(" AND sh.scan_type = @scanType");

                    sqlBuilder.Append(" ORDER BY sh.scan_datetime DESC LIMIT 1000");

                    using (var command = new MySqlCommand(sqlBuilder.ToString(), connection))
                    {
                        if (startDate.HasValue)
                            command.Parameters.AddWithValue("@startDate", startDate.Value);

                        if (endDate.HasValue)
                            command.Parameters.AddWithValue("@endDate", endDate.Value);

                        if (studentId.HasValue)
                            command.Parameters.AddWithValue("@studentId", studentId.Value);

                        if (!string.IsNullOrEmpty(scanType))
                            command.Parameters.AddWithValue("@scanType", scanType);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                scans.Add(MapScanHistory(reader));
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                await ErrorLoggingService.LogErrorAsync(
                    "Get Scan History - Database Error",
                    ex,
                    "scan_history");
                throw new Exception(ErrorLoggingService.GetUserFriendlyMessage(ex), ex);
            }
            catch (Exception ex)
            {
                await ErrorLoggingService.LogErrorAsync(
                    "Get Scan History - Error",
                    ex,
                    "scan_history");
                throw new Exception($"Error retrieving scan history: {ex.Message}", ex);
            }

            return scans;
        }

        /// <summary>
        /// Get daily attendance summary using stored procedure
        /// </summary>
        public async Task<DataTable> GetDailySummaryAsync(DateTime? targetDate = null)
        {
            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    await connection.OpenAsync();
                    using (var command = new MySqlCommand("sp_get_daily_summary", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        // Fixed: Parameter name must match stored procedure definition (p_date, not p_target_date)
                        command.Parameters.AddWithValue("@p_date", targetDate ?? DateTime.Today);

                        using (var adapter = new MySqlDataAdapter(command))
                        {
                            var dataTable = new DataTable();
                            adapter.Fill(dataTable);
                            return dataTable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving daily summary: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Get recent scans using view
        /// </summary>
        public async Task<List<ScanHistory>> GetRecentScansAsync(int limit = 50)
        {
            var scans = new List<ScanHistory>();

            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    await connection.OpenAsync();
                    string query = $"SELECT * FROM vw_recent_scans LIMIT {limit}";

                    using (var command = new MySqlCommand(query, connection))
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            scans.Add(MapScanHistory(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving recent scans: {ex.Message}", ex);
            }

            return scans;
        }

        /// <summary>
        /// Get student scan statistics
        /// </summary>
        public async Task<DataTable> GetStudentStatsAsync()
        {
            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    await connection.OpenAsync();
                    string query = "SELECT * FROM vw_student_scan_stats ORDER BY total_scans DESC";

                    using (var command = new MySqlCommand(query, connection))
                    using (var adapter = new MySqlDataAdapter(command))
                    {
                        var dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        return dataTable;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving student statistics: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Get scan by ID
        /// </summary>
        public async Task<ScanHistory> GetByIdAsync(int scanId)
        {
            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    await connection.OpenAsync();
                    string query = @"SELECT sh.*, s.student_number, 
                                    CONCAT(s.first_name, ' ', s.last_name) as student_name,
                                    d.device_name
                                    FROM scan_history sh
                                    INNER JOIN students s ON sh.student_id = s.student_id
                                    INNER JOIN devices d ON sh.device_id = d.device_id
                                    WHERE sh.scan_id = @scanId";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@scanId", scanId);
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return MapScanHistory(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving scan: {ex.Message}", ex);
            }

            return null;
        }

        /// <summary>
        /// Get scans by student
        /// </summary>
        public async Task<List<ScanHistory>> GetByStudentAsync(int studentId, int limit = 100)
        {
            var scans = new List<ScanHistory>();

            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    await connection.OpenAsync();
                    string query = @"SELECT sh.*, s.student_number, 
                                    CONCAT(s.first_name, ' ', s.last_name) as student_name,
                                    d.device_name
                                    FROM scan_history sh
                                    INNER JOIN students s ON sh.student_id = s.student_id
                                    INNER JOIN devices d ON sh.device_id = d.device_id
                                    WHERE sh.student_id = @studentId
                                    ORDER BY sh.scan_datetime DESC
                                    LIMIT @limit";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@studentId", studentId);
                        command.Parameters.AddWithValue("@limit", limit);
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                scans.Add(MapScanHistory(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving student scans: {ex.Message}", ex);
            }

            return scans;
        }

        private ScanHistory MapScanHistory(MySqlDataReader reader)
        {
            // Check if time_out column exists in the result set
            DateTime? timeOut = null;
            try
            {
                int timeOutOrdinal = reader.GetOrdinal("time_out");
                if (!reader.IsDBNull(timeOutOrdinal))
                {
                    timeOut = reader.GetDateTime(timeOutOrdinal);
                }
            }
            catch
            {
                // Column doesn't exist, leave as null
            }

            // Handle scan_datetime vs time_in (view uses time_in, stored procedure uses scan_datetime)
            DateTime scanDateTime;
            try
            {
                scanDateTime = reader.GetDateTime("scan_datetime");
            }
            catch
            {
                try
                {
                    scanDateTime = reader.GetDateTime("time_in");
                }
                catch
                {
                    scanDateTime = DateTime.MinValue;
                }
            }

            return new ScanHistory
            {
                ScanId = reader.GetInt32("scan_id"),
                StudentId = reader.FieldCount > 9 && !reader.IsDBNull(reader.GetOrdinal("student_id")) ? reader.GetInt32("student_id") : 0,
                DeviceId = reader.FieldCount > 9 && !reader.IsDBNull(reader.GetOrdinal("device_id")) ? reader.GetInt32("device_id") : 0,
                ScanType = reader.GetString("scan_type"),
                ScanData = reader.FieldCount > 9 && !reader.IsDBNull(reader.GetOrdinal("scan_data")) ? reader.GetString("scan_data") : null,
                ScanDateTime = scanDateTime,
                TimeOut = timeOut,
                ScanPurpose = reader.FieldCount > 9 && !reader.IsDBNull(reader.GetOrdinal("scan_purpose")) ? reader.GetString("scan_purpose") : null,
                Location = reader.IsDBNull(reader.GetOrdinal("location")) ? null : reader.GetString("location"),
                Status = reader.GetString("status"),
                Notes = reader.FieldCount > 9 && !reader.IsDBNull(reader.GetOrdinal("notes")) ? reader.GetString("notes") : null,
                CreatedAt = reader.FieldCount > 9 && !reader.IsDBNull(reader.GetOrdinal("created_at")) ? reader.GetDateTime("created_at") : DateTime.MinValue,
                StudentNumber = reader.IsDBNull(reader.GetOrdinal("student_number")) ? null : reader.GetString("student_number"),
                StudentName = reader.IsDBNull(reader.GetOrdinal("student_name")) ? null : reader.GetString("student_name"),
                DeviceName = reader.IsDBNull(reader.GetOrdinal("device_name")) ? null : reader.GetString("device_name")
            };
        }
    }
}
