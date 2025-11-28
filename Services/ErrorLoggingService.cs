using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySqlConnector;
using ITP104_FINAL_PROJECT.Data;

namespace ITP104_FINAL_PROJECT.Services
{
    /// <summary>
    /// Centralized error logging service that logs errors to system_logs table
    /// </summary>
    public static class ErrorLoggingService
    {
        /// <summary>
        /// Log an error to the system_logs table
        /// </summary>
        public static async Task LogErrorAsync(
            string action,
            Exception exception,
            string tableName = null,
            int? recordId = null,
            int? userId = null)
        {
            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    await connection.OpenAsync();

                    string query = @"INSERT INTO system_logs 
                                    (user_id, action, table_name, record_id, old_value, new_value, timestamp)
                                    VALUES (@userId, @action, @tableName, @recordId, @errorType, @errorMessage, CURRENT_TIMESTAMP)";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@userId", userId.HasValue ? (object)userId.Value : DBNull.Value);
                        command.Parameters.AddWithValue("@action", $"ERROR: {action}");
                        command.Parameters.AddWithValue("@tableName", tableName ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@recordId", recordId.HasValue ? (object)recordId.Value : DBNull.Value);
                        command.Parameters.AddWithValue("@errorType", exception.GetType().Name);
                        command.Parameters.AddWithValue("@errorMessage",
                            $"{exception.Message}\n\nStack Trace:\n{exception.StackTrace}");

                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception logEx)
            {
                // If logging fails, write to a local file as fallback
                try
                {
                    string logPath = System.IO.Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                        "StudentAttendance",
                        "error_logs.txt");

                    System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(logPath));

                    string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {action}\n" +
                                    $"Error: {exception.Message}\n" +
                                    $"Logging Error: {logEx.Message}\n" +
                                    $"Stack Trace: {exception.StackTrace}\n\n";

                    System.IO.File.AppendAllText(logPath, logEntry);
                }
                catch
                {
                    // Silent fail - nothing we can do if both database and file logging fail
                }
            }
        }

        /// <summary>
        /// Log an informational message to system_logs
        /// </summary>
        public static async Task LogInfoAsync(
            string action,
            string message,
            string tableName = null,
            int? recordId = null,
            int? userId = null)
        {
            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    await connection.OpenAsync();

                    string query = @"INSERT INTO system_logs 
                                    (user_id, action, table_name, record_id, new_value, timestamp)
                                    VALUES (@userId, @action, @tableName, @recordId, @message, CURRENT_TIMESTAMP)";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@userId", userId.HasValue ? (object)userId.Value : DBNull.Value);
                        command.Parameters.AddWithValue("@action", $"INFO: {action}");
                        command.Parameters.AddWithValue("@tableName", tableName ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@recordId", recordId.HasValue ? (object)recordId.Value : DBNull.Value);
                        command.Parameters.AddWithValue("@message", message);

                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch
            {
                // Silent fail for info logging
            }
        }

        /// <summary>
        /// Log a warning message to system_logs
        /// </summary>
        public static async Task LogWarningAsync(
            string action,
            string message,
            string tableName = null,
            int? recordId = null,
            int? userId = null)
        {
            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    await connection.OpenAsync();

                    string query = @"INSERT INTO system_logs 
                                    (user_id, action, table_name, record_id, new_value, timestamp)
                                    VALUES (@userId, @action, @tableName, @recordId, @message, CURRENT_TIMESTAMP)";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@userId", userId.HasValue ? (object)userId.Value : DBNull.Value);
                        command.Parameters.AddWithValue("@action", $"WARNING: {action}");
                        command.Parameters.AddWithValue("@tableName", tableName ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@recordId", recordId.HasValue ? (object)recordId.Value : DBNull.Value);
                        command.Parameters.AddWithValue("@message", message);

                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch
            {
                // Silent fail for warning logging
            }
        }

        /// <summary>
        /// Display a user-friendly error message and log the error
        /// </summary>
        public static async Task ShowAndLogErrorAsync(
            string userMessage,
            Exception exception,
            string action,
            string tableName = null,
            int? recordId = null)
        {
            // Log the error
            await LogErrorAsync(action, exception, tableName, recordId);

            // Show user-friendly message
            MessageBox.Show(
                userMessage,
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        /// <summary>
        /// Get a user-friendly error message based on exception type
        /// </summary>
        public static string GetUserFriendlyMessage(Exception ex)
        {
            if (ex is MySqlException mySqlEx)
            {
                return mySqlEx.Number switch
                {
                    0 => "Unable to connect to the database. Please check your network connection and try again.",
                    1042 => "Database server is not reachable. Please contact your system administrator.",
                    1045 => "Database authentication failed. Please contact your system administrator.",
                    1062 => "This record already exists in the database.",
                    1064 => "A database query error occurred. Please contact your system administrator.",
                    1146 => "A required database table is missing. Please contact your system administrator.",
                    1205 => "The operation timed out due to a database lock. Please try again.",
                    1213 => "A database deadlock occurred. Please try again.",
                    _ => $"A database error occurred (Code: {mySqlEx.Number}). Please try again or contact support."
                };
            }
            else if (ex is TimeoutException)
            {
                return "The operation took too long to complete. Please check your connection and try again.";
            }
            else if (ex is InvalidOperationException)
            {
                return "An invalid operation was attempted. Please verify your input and try again.";
            }
            else if (ex is ArgumentException)
            {
                return "Invalid data was provided. Please check your input and try again.";
            }
            else
            {
                return "An unexpected error occurred. Please try again or contact support if the problem persists.";
            }
        }
    }
}
