using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using MySqlConnector;
using ITP104_FINAL_PROJECT.Models;

namespace ITP104_FINAL_PROJECT.Data
{
    public class ScanHistoryRepository
    {
        /// <summary>
        /// Record a QR scan using stored procedure with duplicate detection
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
                    using (var command = new MySqlCommand("sp_get_scan_history", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@p_start_date", startDate ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@p_end_date", endDate ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@p_student_id", studentId ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@p_limit", 1000); // Default limit
                        command.Parameters.AddWithValue("@p_offset", 0); // Default offset

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
                        command.Parameters.AddWithValue("@p_target_date", targetDate ?? DateTime.Today);

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
            return new ScanHistory
            {
                ScanId = reader.GetInt32("scan_id"),
                StudentId = reader.GetInt32("student_id"),
                DeviceId = reader.GetInt32("device_id"),
                ScanType = reader.GetString("scan_type"),
                ScanData = reader.GetString("scan_data"),
                ScanDateTime = reader.GetDateTime("scan_datetime"),
                ScanPurpose = reader.GetString("scan_purpose"),
                Location = reader.IsDBNull(reader.GetOrdinal("location")) ? null : reader.GetString("location"),
                Status = reader.GetString("status"),
                Notes = reader.IsDBNull(reader.GetOrdinal("notes")) ? null : reader.GetString("notes"),
                CreatedAt = reader.GetDateTime("created_at"),
                StudentNumber = reader.IsDBNull(reader.GetOrdinal("student_number")) ? null : reader.GetString("student_number"),
                StudentName = reader.IsDBNull(reader.GetOrdinal("student_name")) ? null : reader.GetString("student_name"),
                DeviceName = reader.IsDBNull(reader.GetOrdinal("device_name")) ? null : reader.GetString("device_name")
            };
        }
    }
}
