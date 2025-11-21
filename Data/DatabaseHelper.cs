using System;
using System.Configuration;
using System.Threading.Tasks;
using MySqlConnector;

namespace ITP104_FINAL_PROJECT.Data
{
    /// <summary>
    /// Database connection helper for MySqlConnector
    /// Provides async connection management, retry logic, and utility methods
    /// </summary>
    public static class DatabaseHelper
    {
        private static readonly string connectionString;
        private const int MaxRetryAttempts = 3;
        private const int InitialRetryDelayMs = 100;

        static DatabaseHelper()
        {
            try
            {
                connectionString = ConfigurationManager.ConnectionStrings["StudentAttendanceDB"]?.ConnectionString;
                
                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new ConfigurationErrorsException("Connection string 'StudentAttendanceDB' not found in App.config");
                }

                // Configure connection pooling for better performance
                var builder = new MySqlConnectionStringBuilder(connectionString)
                {
                    Pooling = true,
                    MinimumPoolSize = 0,
                    MaximumPoolSize = 100,
                    ConnectionIdleTimeout = 180,
                    ConnectionTimeout = 30,
                    DefaultCommandTimeout = 30
                };
                connectionString = builder.ConnectionString;
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
        /// Open a connection with retry logic and exponential backoff
        /// </summary>
        public static async Task<MySqlConnection> GetConnectionWithRetryAsync()
        {
            Exception lastException = null;
            int retryDelay = InitialRetryDelayMs;

            for (int attempt = 1; attempt <= MaxRetryAttempts; attempt++)
            {
                try
                {
                    var connection = GetConnection();
                    await connection.OpenAsync();
                    return connection;
                }
                catch (MySqlException ex) when (IsTransientError(ex))
                {
                    lastException = ex;

                    if (attempt < MaxRetryAttempts)
                    {
                        // Exponential backoff: wait longer between each retry
                        await Task.Delay(retryDelay);
                        retryDelay *= 2; // Double the delay for next attempt
                    }
                }
                catch (Exception ex)
                {
                    // Non-transient error, don't retry
                    throw new Exception($"Database connection failed: {ex.Message}", ex);
                }
            }

            // All retries exhausted
            throw new Exception(
                $"Failed to connect to database after {MaxRetryAttempts} attempts. " +
                $"Please check your network connection and database server status.",
                lastException);
        }

        /// <summary>
        /// Determine if a MySQL error is transient (worth retrying)
        /// </summary>
        private static bool IsTransientError(MySqlException ex)
        {
            // Common transient error codes
            return ex.Number switch
            {
                0 => true,      // Unable to connect
                1040 => true,   // Too many connections
                1205 => true,   // Lock wait timeout
                1213 => true,   // Deadlock
                2002 => true,   // Connection timeout
                2003 => true,   // Can't connect to server
                2006 => true,   // Server has gone away
                2013 => true,   // Lost connection during query
                _ => false
            };
        }

        /// <summary>
        /// Test database connection asynchronously with retry
        /// </summary>
        public static async Task<bool> TestConnectionAsync()
        {
            try
            {
                using (var connection = await GetConnectionWithRetryAsync())
                {
                    return connection.State == System.Data.ConnectionState.Open;
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
        /// Execute a scalar query asynchronously with retry logic
        /// </summary>
        public static async Task<object> ExecuteScalarAsync(string query, params MySqlParameter[] parameters)
        {
            if (string.IsNullOrWhiteSpace(query))
                throw new ArgumentException("Query cannot be null or empty", nameof(query));

            Exception lastException = null;
            int retryDelay = InitialRetryDelayMs;

            for (int attempt = 1; attempt <= MaxRetryAttempts; attempt++)
            {
                MySqlConnection connection = null;
                try
                {
                    connection = await GetConnectionWithRetryAsync();
                    using (var command = new MySqlCommand(query, connection))
                    {
                        if (parameters != null)
                        {
                            command.Parameters.AddRange(parameters);
                        }
                        return await command.ExecuteScalarAsync();
                    }
                }
                catch (MySqlException ex) when (IsTransientError(ex) && attempt < MaxRetryAttempts)
                {
                    lastException = ex;
                    connection?.Dispose();
                    await Task.Delay(retryDelay);
                    retryDelay *= 2;
                }
                catch (Exception ex)
                {
                    connection?.Dispose();
                    throw new Exception($"Database query failed: {ex.Message}", ex);
                }
                finally
                {
                    connection?.Dispose();
                }
            }

            throw new Exception(
                $"Query failed after {MaxRetryAttempts} attempts.",
                lastException);
        }

        /// <summary>
        /// Execute a non-query command asynchronously with retry logic
        /// </summary>
        public static async Task<int> ExecuteNonQueryAsync(string query, params MySqlParameter[] parameters)
        {
            if (string.IsNullOrWhiteSpace(query))
                throw new ArgumentException("Query cannot be null or empty", nameof(query));

            Exception lastException = null;
            int retryDelay = InitialRetryDelayMs;

            for (int attempt = 1; attempt <= MaxRetryAttempts; attempt++)
            {
                MySqlConnection connection = null;
                try
                {
                    connection = await GetConnectionWithRetryAsync();
                    using (var command = new MySqlCommand(query, connection))
                    {
                        if (parameters != null)
                        {
                            command.Parameters.AddRange(parameters);
                        }
                        return await command.ExecuteNonQueryAsync();
                    }
                }
                catch (MySqlException ex) when (IsTransientError(ex) && attempt < MaxRetryAttempts)
                {
                    lastException = ex;
                    connection?.Dispose();
                    await Task.Delay(retryDelay);
                    retryDelay *= 2;
                }
                catch (Exception ex)
                {
                    connection?.Dispose();
                    throw new Exception($"Database command failed: {ex.Message}", ex);
                }
                finally
                {
                    connection?.Dispose();
                }
            }

            throw new Exception(
                $"Command failed after {MaxRetryAttempts} attempts.",
                lastException);
        }

        /// <summary>
        /// Get connection string (for diagnostic purposes only)
        /// </summary>
        public static string GetConnectionStringInfo()
        {
            try
            {
                var builder = new MySqlConnectionStringBuilder(connectionString);
                return $"Server={builder.Server}, Database={builder.Database}, User={builder.UserID}";
            }
            catch (Exception ex)
            {
                return $"Error reading connection string: {ex.Message}";
            }
        }

        /// <summary>
        /// Get detailed connection health information
        /// </summary>
        public static async Task<(bool IsHealthy, string Message)> GetConnectionHealthAsync()
        {
            try
            {
                using (var connection = await GetConnectionWithRetryAsync())
                {
                    using (var command = new MySqlCommand("SELECT 1", connection))
                    {
                        var result = await command.ExecuteScalarAsync();
                        if (result != null && Convert.ToInt32(result) == 1)
                        {
                            return (true, "Database connection is healthy");
                        }
                    }
                }
                return (false, "Database connection test failed");
            }
            catch (Exception ex)
            {
                return (false, $"Database connection error: {ex.Message}");
            }
        }
    }
}

