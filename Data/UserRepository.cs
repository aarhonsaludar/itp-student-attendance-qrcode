using System;
using System.Threading.Tasks;
using MySqlConnector;
using ITP104_FINAL_PROJECT.Models;

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
                using (var connection = DatabaseHelper.GetConnection())
                {
                    await connection.OpenAsync();
                    string query = "SELECT * FROM users WHERE username = @username AND password_hash = @password AND is_active = 1";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@username", username);
                        command.Parameters.AddWithValue("@password", password);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                var user = MapUser(reader);
                                reader.Close();

                                await UpdateLastLoginAsync(connection, user.UserId);
                                return user;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Authentication error: {ex.Message}", ex);
            }

            return null;
        }

        /// <summary>
        /// Create a new user with BCrypt hashed password
        /// </summary>
        public async Task<(bool success, string message, int userId)> CreateUserAsync(User user, string plainPassword)
        {
            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    await connection.OpenAsync();

                    // Check if username already exists
                    string checkQuery = "SELECT COUNT(*) FROM users WHERE username = @username";
                    using (var checkCommand = new MySqlCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@username", user.Username);
                        long count = (long)await checkCommand.ExecuteScalarAsync();
                        if (count > 0)
                        {
                            return (false, "Username already exists", 0);
                        }
                    }

                    // Insert new user with plaintext password
                    string insertQuery = @"INSERT INTO users (username, password_hash, full_name, email, role, is_active)
                                          VALUES (@username, @password, @fullName, @email, @role, @isActive);
                                          SELECT LAST_INSERT_ID();";

                    using (var command = new MySqlCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@username", user.Username);
                        command.Parameters.AddWithValue("@password", plainPassword);
                        command.Parameters.AddWithValue("@fullName", user.FullName);
                        command.Parameters.AddWithValue("@email", user.Email ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@role", user.Role);
                        command.Parameters.AddWithValue("@isActive", user.IsActive);

                        var userId = Convert.ToInt32(await command.ExecuteScalarAsync());
                        return (true, "User created successfully", userId);
                    }
                }
            }
            catch (Exception ex)
            {
                return (false, $"Database error: {ex.Message}", 0);
            }
        }        /// <summary>
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
                PasswordHash = reader.GetString("password_hash"),
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
