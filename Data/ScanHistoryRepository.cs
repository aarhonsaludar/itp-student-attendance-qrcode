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
        /// Record a QR scan with Time In/Time Out logic using secure stored procedure
        /// CRITICAL: Client time is validated against INTERNET time sources (Google.com, TimeAPI.io)
        /// Database timestamps use NOW() for recording, but validation uses internet APIs
        /// This prevents time manipulation by changing device clock
        /// </summary>
        public async Task<(bool success, string message, string scanType, DateTime? timestamp, DateTime? timeIn, DateTime? timeOut)> RecordAttendanceScanAsync(
            string qrData,
            int deviceId,
            string location = null)
        {
            try
            {
                // ===================================================
                // STEP 1: Validate client time against server time
                // Allow offline mode but flag for review
                // ===================================================
                var timeValidation = await TimeValidationService.ValidateClientTimeAsync();

                if (!timeValidation.IsValid && timeValidation.ValidationStatus != TimeValidationStatus.OfflineMode)
                {
                    // Time tampering detected - BLOCK attendance recording
                    await ErrorLoggingService.LogWarningAsync(
                        "Time Tampering Detected - Attendance BLOCKED",
                        $"Client Time: {timeValidation.ClientTime:yyyy-MM-dd HH:mm:ss}\n" +
                        $"Server Time: {timeValidation.ServerTime?.ToString("yyyy-MM-dd HH:mm:ss") ?? "N/A"}\n" +
                        $"Time Drift: {timeValidation.TimeDrift.TotalMinutes:F2} minutes\n" +
                        $"QR Data: {qrData}\n" +
                        $"Status: {timeValidation.ValidationStatus}",
                        "time_tampering");

                    return (false,
                        $"⚠️ TIME TAMPERING DETECTED\n\n{timeValidation.ErrorMessage}\n\nAttendance recording is BLOCKED for security.",
                        "TIME_TAMPERED",
                        null, null, null);
                }

                // Log offline mode scans
                if (timeValidation.RequiresManualReview)
                {
                    await ErrorLoggingService.LogWarningAsync(
                        "⚠️ OFFLINE ATTENDANCE - Flagged for Review",
                        $"Client Time: {timeValidation.ClientTime:yyyy-MM-dd HH:mm:ss}\n" +
                        $"Validation Status: {timeValidation.ValidationStatus}\n" +
                        $"QR Data: {qrData}\n" +
                        $"This attendance will be flagged for manual review.",
                        "offline_attendance");
                }

                // Validate input
                if (string.IsNullOrWhiteSpace(qrData))
                {
                    return (false, "QR code data cannot be empty", "ERROR", null, null, null);
                }

                if (deviceId <= 0)
                {
                    return (false, "Invalid device ID", "ERROR", null, null, null);
                }

                using (var connection = await DatabaseHelper.GetConnectionWithRetryAsync())
                {
                    using (var command = new MySqlCommand("sp_record_attendance_scan_secure", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = 60; // Increase timeout for stored procedure

                        // ===================================================
                        // INPUT PARAMETERS
                        // Include validation parameters for tracking
                        // ===================================================
                        command.Parameters.AddWithValue("@p_scan_data", qrData);
                        command.Parameters.AddWithValue("@p_device_id", deviceId);
                        command.Parameters.AddWithValue("@p_location", location ?? (object)DBNull.Value);

                        // Add validation tracking parameters
                        command.Parameters.AddWithValue("@p_validation_status",
                            timeValidation.ValidationStatus == TimeValidationStatus.OfflineMode ? "offline_mode" : "verified");
                        command.Parameters.AddWithValue("@p_requires_review", timeValidation.RequiresManualReview);
                        command.Parameters.AddWithValue("@p_client_time", timeValidation.ClientTime);
                        command.Parameters.AddWithValue("@p_server_time",
                            timeValidation.ServerTime.HasValue ? (object)timeValidation.ServerTime.Value : DBNull.Value);
                        command.Parameters.AddWithValue("@p_time_drift_seconds",
                            timeValidation.ServerTime.HasValue ? (int)timeValidation.TimeDrift.TotalSeconds : (object)DBNull.Value);

                        // ===================================================
                        // OUTPUT PARAMETERS
                        // Database returns the timestamps it generated
                        // ===================================================
                        var resultParam = new MySqlParameter("@p_result", MySqlDbType.VarChar, 200) { Direction = ParameterDirection.Output };
                        var studentNameParam = new MySqlParameter("@p_student_name", MySqlDbType.VarChar, 200) { Direction = ParameterDirection.Output };
                        var studentNumberParam = new MySqlParameter("@p_student_number", MySqlDbType.VarChar, 50) { Direction = ParameterDirection.Output };
                        var scanTypeParam = new MySqlParameter("@p_scan_type", MySqlDbType.VarChar, 20) { Direction = ParameterDirection.Output };
                        var timestampParam = new MySqlParameter("@p_timestamp", MySqlDbType.DateTime) { Direction = ParameterDirection.Output };
                        var timeInParam = new MySqlParameter("@p_time_in", MySqlDbType.DateTime) { Direction = ParameterDirection.Output };
                        var timeOutParam = new MySqlParameter("@p_time_out", MySqlDbType.DateTime) { Direction = ParameterDirection.Output };

                        command.Parameters.Add(resultParam);
                        command.Parameters.Add(studentNameParam);
                        command.Parameters.Add(studentNumberParam);
                        command.Parameters.Add(scanTypeParam);
                        command.Parameters.Add(timestampParam);
                        command.Parameters.Add(timeInParam);
                        command.Parameters.Add(timeOutParam);

                        await command.ExecuteNonQueryAsync();

                        // Extract output values
                        string result = resultParam.Value?.ToString() ?? "Unknown error";
                        string studentName = studentNameParam.Value?.ToString();
                        string studentNumber = studentNumberParam.Value?.ToString();
                        string scanType = scanTypeParam.Value?.ToString() ?? "ERROR";

                        // ===================================================
                        // CRITICAL: Extract database-generated timestamps
                        // These are the ONLY trusted timestamps in the system
                        // ===================================================
                        DateTime? timestamp = timestampParam.Value != DBNull.Value ? (DateTime?)timestampParam.Value : null;
                        DateTime? timeIn = timeInParam.Value != DBNull.Value ? (DateTime?)timeInParam.Value : null;
                        DateTime? timeOut = timeOutParam.Value != DBNull.Value ? (DateTime?)timeOutParam.Value : null;

                        // Determine success based on result message
                        bool success = result.StartsWith("SUCCESS");

                        // Format message to include student info if available
                        string message = result;
                        if (!string.IsNullOrEmpty(studentName) && !string.IsNullOrEmpty(studentNumber))
                        {
                            message = $"{studentName} ({studentNumber})\n{result}";
                        }

                        // Log the scan attempt with database timestamp
                        if (success)
                        {
                            await ErrorLoggingService.LogInfoAsync(
                                $"Attendance Scan - {scanType}",
                                $"Student: {studentNumber} - {studentName} | DB Time: {timestamp:yyyy-MM-dd HH:mm:ss}",
                                "scan_history");
                        }
                        else
                        {
                            await ErrorLoggingService.LogInfoAsync(
                                "Attendance Scan - Failed",
                                $"QR Data: {qrData.Substring(0, Math.Min(50, qrData.Length))}... Result: {result}",
                                "scan_history");
                        }

                        return (success, message, scanType, timestamp, timeIn, timeOut);
                    }
                }
            }
            catch (MySqlException ex)
            {
                await ErrorLoggingService.LogErrorAsync(
                    "Record Attendance Scan - Database Error",
                    ex,
                    "scan_history");
                return (false, ErrorLoggingService.GetUserFriendlyMessage(ex), "ERROR", null, null, null);
            }
            catch (Exception ex)
            {
                await ErrorLoggingService.LogErrorAsync(
                    "Record Attendance Scan - Error",
                    ex,
                    "scan_history");
                return (false, $"An error occurred while recording scan: {ex.Message}", "ERROR", null, null, null);
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
                                    s.program,
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
                                    s.program,
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
                                    s.program,
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
            // Helper method to safely get column ordinal
            int TryGetOrdinal(string columnName)
            {
                try
                {
                    return reader.GetOrdinal(columnName);
                }
                catch
                {
                    return -1;
                }
            }

            // Helper method to safely get string value
            string GetStringValue(int ordinal)
            {
                if (ordinal < 0 || reader.IsDBNull(ordinal))
                    return null;
                return reader.GetString(ordinal);
            }

            // Helper method to safely get int value
            int GetIntValue(int ordinal)
            {
                if (ordinal < 0 || reader.IsDBNull(ordinal))
                    return 0;
                return reader.GetInt32(ordinal);
            }

            // Helper method to safely get DateTime value
            DateTime GetDateTimeValue(int ordinal)
            {
                if (ordinal < 0 || reader.IsDBNull(ordinal))
                    return DateTime.MinValue;
                return reader.GetDateTime(ordinal);
            }

            // Get ordinals for all columns we might need
            int scanIdOrdinal = TryGetOrdinal("scan_id");
            int studentIdOrdinal = TryGetOrdinal("student_id");
            int deviceIdOrdinal = TryGetOrdinal("device_id");
            int scanTypeOrdinal = TryGetOrdinal("scan_type");
            int scanDataOrdinal = TryGetOrdinal("scan_data");
            int scanDatetimeOrdinal = TryGetOrdinal("scan_datetime");
            int timeOutOrdinal = TryGetOrdinal("time_out");
            int scanPurposeOrdinal = TryGetOrdinal("scan_purpose");
            int locationOrdinal = TryGetOrdinal("location");
            int statusOrdinal = TryGetOrdinal("status");
            int notesOrdinal = TryGetOrdinal("notes");
            int createdAtOrdinal = TryGetOrdinal("created_at");
            int studentNumberOrdinal = TryGetOrdinal("student_number");
            int studentNameOrdinal = TryGetOrdinal("student_name");
            int programOrdinal = TryGetOrdinal("program");
            int deviceNameOrdinal = TryGetOrdinal("device_name");

            // Additional columns from migrations
            int validationStatusOrdinal = TryGetOrdinal("validation_status");
            int requiresReviewOrdinal = TryGetOrdinal("requires_review");
            int clientTimeOrdinal = TryGetOrdinal("client_time");
            int serverTimeOrdinal = TryGetOrdinal("server_time");
            int timeDriftSecondsOrdinal = TryGetOrdinal("time_drift_seconds");
            int timeInValidationModeOrdinal = TryGetOrdinal("time_in_validation_mode");
            int timeOutValidationModeOrdinal = TryGetOrdinal("time_out_validation_mode");

            // Determine scan datetime - use scan_datetime column
            DateTime scanDateTime;
            if (scanDatetimeOrdinal >= 0 && !reader.IsDBNull(scanDatetimeOrdinal))
            {
                scanDateTime = reader.GetDateTime(scanDatetimeOrdinal);
            }
            else
            {
                scanDateTime = DateTime.MinValue;
            }

            return new ScanHistory
            {
                ScanId = GetIntValue(scanIdOrdinal),
                StudentId = GetIntValue(studentIdOrdinal),
                DeviceId = deviceIdOrdinal >= 0 ? (int?)GetIntValue(deviceIdOrdinal) : null,
                ScanType = GetStringValue(scanTypeOrdinal) ?? "UNKNOWN",
                ScanData = GetStringValue(scanDataOrdinal),
                ScanDateTime = scanDateTime,
                TimeOut = timeOutOrdinal >= 0 ? (reader.IsDBNull(timeOutOrdinal) ? (DateTime?)null : reader.GetDateTime(timeOutOrdinal)) : null,
                ScanPurpose = GetStringValue(scanPurposeOrdinal),
                Location = GetStringValue(locationOrdinal),
                Status = GetStringValue(statusOrdinal) ?? "unknown",
                Notes = GetStringValue(notesOrdinal),
                CreatedAt = GetDateTimeValue(createdAtOrdinal),
                RequiresReview = requiresReviewOrdinal >= 0 && !reader.IsDBNull(requiresReviewOrdinal)
                    ? reader.GetBoolean(requiresReviewOrdinal)
                    : (GetStringValue(statusOrdinal)?.ToLower().Contains("review") ?? false),
                StudentNumber = GetStringValue(studentNumberOrdinal),
                StudentName = GetStringValue(studentNameOrdinal),
                Program = GetStringValue(programOrdinal),
                DeviceName = GetStringValue(deviceNameOrdinal),

                // Additional migration columns
                ValidationStatus = GetStringValue(validationStatusOrdinal),
                ClientTime = clientTimeOrdinal >= 0 && !reader.IsDBNull(clientTimeOrdinal) ? (DateTime?)reader.GetDateTime(clientTimeOrdinal) : null,
                ServerTime = serverTimeOrdinal >= 0 && !reader.IsDBNull(serverTimeOrdinal) ? (DateTime?)reader.GetDateTime(serverTimeOrdinal) : null,
                TimeDriftSeconds = timeDriftSecondsOrdinal >= 0 && !reader.IsDBNull(timeDriftSecondsOrdinal) ? (int?)reader.GetInt32(timeDriftSecondsOrdinal) : null,
                TimeInValidationMode = GetStringValue(timeInValidationModeOrdinal),
                TimeOutValidationMode = GetStringValue(timeOutValidationModeOrdinal)
            };
        }

        /// <summary>
        /// Approve a scan that requires manual review (offline scans)
        /// Updates the status from 'for_review' to 'success'
        /// </summary>
        public async Task<bool> ApproveScanAsync(int scanId)
        {
            try
            {
                using (var connection = await DatabaseHelper.GetConnectionWithRetryAsync())
                {
                    string query = @"
                        UPDATE scan_history 
                        SET status = 'success',
                            notes = CONCAT(COALESCE(notes, ''), '\nApproved by admin on ', NOW())
                        WHERE scan_id = @scanId 
                        AND status = 'for_review'";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@scanId", scanId);
                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            await ErrorLoggingService.LogInfoAsync(
                                "Scan Approved",
                                $"Scan ID {scanId} approved - status changed from 'for_review' to 'success'",
                                "scan_review");
                            return true;
                        }
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                await ErrorLoggingService.LogErrorAsync(
                    "Approve Scan Failed",
                    ex,
                    "scan_review");
                return false;
            }
        }

        /// <summary>
        /// Decline a scan that requires manual review (offline scans)
        /// Updates the status from 'for_review' to 'failed'
        /// </summary>
        public async Task<bool> DeclineScanAsync(int scanId)
        {
            try
            {
                using (var connection = await DatabaseHelper.GetConnectionWithRetryAsync())
                {
                    string query = @"
                        UPDATE scan_history 
                        SET status = 'failed',
                            notes = CONCAT(COALESCE(notes, ''), '\nDeclined by admin on ', NOW())
                        WHERE scan_id = @scanId 
                        AND status = 'for_review'";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@scanId", scanId);
                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            await ErrorLoggingService.LogInfoAsync(
                                "Scan Declined",
                                $"Scan ID {scanId} declined - status changed from 'for_review' to 'failed'",
                                "scan_review");
                            return true;
                        }
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                await ErrorLoggingService.LogErrorAsync(
                    "Decline Scan Failed",
                    ex,
                    "scan_review");
                return false;
            }
        }

        /// <summary>
        /// Check if student has an active Time In for today without Time Out
        /// </summary>
        public async Task<bool> HasActiveTodayTimeInAsync(int studentId)
        {
            try
            {
                using (var connection = await DatabaseHelper.GetConnectionWithRetryAsync())
                {
                    string query = @"
                        SELECT COUNT(*) 
                        FROM scan_history 
                        WHERE student_id = @studentId 
                        AND DATE(scan_datetime) = CURDATE()
                        AND time_out IS NULL
                        AND status != 'failed'";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@studentId", studentId);
                        var result = await command.ExecuteScalarAsync();
                        return Convert.ToInt32(result) > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                await ErrorLoggingService.LogErrorAsync(
                    "Check Active Time In Failed",
                    ex,
                    "scan_history");
                return false;
            }
        }
    }
}
