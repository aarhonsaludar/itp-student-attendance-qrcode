using System;
using System.Threading.Tasks;
using MySqlConnector;
using BCrypt.Net;
using ITP104_FINAL_PROJECT.Models;
using ITP104_FINAL_PROJECT.Services;

namespace ITP104_FINAL_PROJECT.Data
{
    public class UserRepository
    {
        /// <summary>
        /// Authenticate user with username and password
        /// </summary>
        public async Task<User> AuthenticateAsync(string username, string password)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(username))
                {
                    await ErrorLoggingService.LogInfoAsync(
                        "Authentication - Empty Username",
                        "Authentication attempt with empty username",
                        "users");
                    return null;
                }

                if (string.IsNullOrWhiteSpace(password))
                {
                    await ErrorLoggingService.LogInfoAsync(
                        "Authentication - Empty Password",
                        $"Authentication attempt with empty password for user: {username}",
                        "users");
                    return null;
                }

                using (var connection = await DatabaseHelper.GetConnectionWithRetryAsync())
                {
                    // First, get user by username only (don't check password in SQL)
                    string query = "SELECT * FROM users WHERE username = @username AND is_active = 1";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@username", username);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                var user = MapUser(reader);
                                reader.Close();

                                // Trim password hash just in case
                                string hashToVerify = user.PasswordHash.Trim();

                                // Check if it's a BCrypt hash or plaintext
                                bool passwordValid = false;

                                if (hashToVerify.StartsWith("$2a$") || hashToVerify.StartsWith("$2b$") || hashToVerify.StartsWith("$2x$"))
                                {
                                    // It's a BCrypt hash - verify using BCrypt
                                    try
                                    {
                                        passwordValid = BCrypt.Net.BCrypt.Verify(password, hashToVerify);
                                    }
                                    catch (Exception bcryptEx)
                                    {
                                        await ErrorLoggingService.LogErrorAsync(
                                            "Authentication - BCrypt Error",
                                            bcryptEx,
                                            "users");
                                        passwordValid = false;
                                    }
                                }
                                else
                                {
                                    // Fallback: plaintext comparison (for testing/recovery)
                                    passwordValid = (password == hashToVerify);
                                }

                                await ErrorLoggingService.LogInfoAsync(
                                    "Authentication - Verification Attempt",
                                    $"User: {username}, Hash type: {(hashToVerify.StartsWith("$") ? "BCrypt" : "Plaintext")}, Hash length: {hashToVerify.Length}, Match: {passwordValid}",
                                    "users");

                                if (passwordValid)
                                {
                                    await UpdateLastLoginAsync(connection, user.UserId);

                                    await ErrorLoggingService.LogInfoAsync(
                                        "Authentication - Success",
                                        $"User logged in: {username}",
                                        "users",
                                        user.UserId);

                                    return user;
                                }
                                else
                                {
                                    await ErrorLoggingService.LogInfoAsync(
                                        "Authentication - Failed",
                                        $"Failed login attempt for username: {username} - Invalid password",
                                        "users");
                                }
                            }
                            else
                            {
                                await ErrorLoggingService.LogInfoAsync(
                                    "Authentication - Failed",
                                    $"Failed login attempt for username: {username} - User not found",
                                    "users");
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                await ErrorLoggingService.LogErrorAsync(
                    "Authentication - Database Error",
                    ex,
                    "users");
                throw new Exception(ErrorLoggingService.GetUserFriendlyMessage(ex), ex);
            }
            catch (Exception ex)
            {
                await ErrorLoggingService.LogErrorAsync(
                    "Authentication - Error",
                    ex,
                    "users");
                throw new Exception($"Authentication error: {ex.Message}", ex);
            }

            return null;
        }

        /// <summary>
        /// Get user by username
        /// </summary>
        public async Task<User> GetByUsernameAsync(string username)
        {
            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    await connection.OpenAsync();
                    string query = "SELECT * FROM users WHERE username = @username";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@username", username);
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return MapUser(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving user: {ex.Message}", ex);
            }

            return null;
        }

        /// <summary>
        /// Update user's last login timestamp
        /// </summary>
        private async Task UpdateLastLoginAsync(MySqlConnection connection, int userId)
        {
            string query = "UPDATE users SET last_login = CURRENT_TIMESTAMP WHERE user_id = @userId";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@userId", userId);
                await command.ExecuteNonQueryAsync();
            }
        }

        /// <summary>
        /// Change user password
        /// </summary>
        public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    await connection.OpenAsync();

                    // Verify current password
                    string checkQuery = "SELECT COUNT(*) FROM users WHERE user_id = @userId AND password_hash = @currentPassword";
                    using (var command = new MySqlCommand(checkQuery, connection))
                    {
                        command.Parameters.AddWithValue("@userId", userId);
                        command.Parameters.AddWithValue("@currentPassword", currentPassword);
                        long count = (long)await command.ExecuteScalarAsync();
                        if (count == 0)
                        {
                            return false;
                        }
                    }

                    // Update new password
                    string updateQuery = "UPDATE users SET password_hash = @newPassword WHERE user_id = @userId";
                    using (var command = new MySqlCommand(updateQuery, connection))
                    {
                        command.Parameters.AddWithValue("@userId", userId);
                        command.Parameters.AddWithValue("@newPassword", newPassword);
                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error changing password: {ex.Message}", ex);
            }
        }
        private User MapUser(MySqlDataReader reader)
        {
            return new User
            {
                UserId = reader.GetInt32("user_id"),
                Username = reader.GetString("username"),
                PasswordHash = reader.GetString("password_hash").Trim(),  // Trim any whitespace
                FullName = reader.GetString("full_name"),
                Email = reader.IsDBNull(reader.GetOrdinal("email")) ? null : reader.GetString("email"),
                Role = reader.GetString("role"),
                IsActive = reader.GetBoolean("is_active"),
                CreatedAt = reader.GetDateTime("created_at"),
                LastLogin = reader.IsDBNull(reader.GetOrdinal("last_login")) ? (DateTime?)null : reader.GetDateTime("last_login")
            };
        }
    }
}
