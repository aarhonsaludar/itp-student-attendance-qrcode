using System;
using System.Configuration;
using System.Threading.Tasks;
using MySqlConnector;

namespace ITP104_FINAL_PROJECT.Data
{
    /// <summary>
    /// Database connection helper for MySqlConnector
    /// Provides async connection management and utility methods
    /// </summary>
    public static class DatabaseHelper
    {
        private static readonly string connectionString;

        static DatabaseHelper()
        {
            try
            {
                connectionString = ConfigurationManager.ConnectionStrings["StudentAttendanceDB"]?.ConnectionString;
                
                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new ConfigurationErrorsException("Connection string 'StudentAttendanceDB' not found in App.config");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to initialize database connection: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Get a new MySqlConnection instance (async-ready)
        /// </summary>
        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }

        /// <summary>
        /// Test database connection asynchronously
        /// </summary>
        public static async Task<bool> TestConnectionAsync()
        {
            try
            {
                using (var connection = GetConnection())
                {
                    await connection.OpenAsync();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Test database connection synchronously
        /// </summary>
        public static bool TestConnection()
        {
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Execute a scalar query asynchronously
        /// </summary>
        public static async Task<object> ExecuteScalarAsync(string query, params MySqlParameter[] parameters)
        {
            using (var connection = GetConnection())
            {
                await connection.OpenAsync();
                using (var command = new MySqlCommand(query, connection))
                {
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }
                    return await command.ExecuteScalarAsync();
                }
            }
        }

        /// <summary>
        /// Execute a non-query command asynchronously
        /// </summary>
        public static async Task<int> ExecuteNonQueryAsync(string query, params MySqlParameter[] parameters)
        {
            using (var connection = GetConnection())
            {
                await connection.OpenAsync();
                using (var command = new MySqlCommand(query, connection))
                {
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }
                    return await command.ExecuteNonQueryAsync();
                }
            }
        }

        /// <summary>
        /// Get connection string (for diagnostic purposes only)
        /// </summary>
        public static string GetConnectionStringInfo()
        {
            var builder = new MySqlConnectionStringBuilder(connectionString);
            return $"Server={builder.Server}, Database={builder.Database}, User={builder.UserID}";
        }
    }
}
