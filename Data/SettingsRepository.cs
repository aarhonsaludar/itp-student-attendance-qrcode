using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MySqlConnector;
using ITP104_FINAL_PROJECT.Models;
using ITP104_FINAL_PROJECT.Services;

namespace ITP104_FINAL_PROJECT.Data
{
    public class SettingsRepository
    {
        /// <summary>
        /// Get all system settings
        /// </summary>
        public async Task<List<SystemSetting>> GetAllSettingsAsync()
        {
            var settings = new List<SystemSetting>();

            try
            {
                using (var connection = await DatabaseHelper.GetConnectionWithRetryAsync())
                {
                    string query = "SELECT * FROM system_settings ORDER BY setting_category, setting_key";

                    using (var command = new MySqlCommand(query, connection))
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            settings.Add(MapSetting(reader));
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                await ErrorLoggingService.LogErrorAsync(
                    "Get All Settings - Database Error",
                    ex,
                    "system_settings");
                throw new Exception(ErrorLoggingService.GetUserFriendlyMessage(ex), ex);
            }
            catch (Exception ex)
            {
                await ErrorLoggingService.LogErrorAsync(
                    "Get All Settings - Error",
                    ex,
                    "system_settings");
                throw new Exception($"Error retrieving settings: {ex.Message}", ex);
            }

            return settings;
        }

        /// <summary>
        /// Get setting by key
        /// </summary>
        public async Task<SystemSetting> GetByKeyAsync(string settingKey)
        {
            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    await connection.OpenAsync();
                    string query = "SELECT * FROM system_settings WHERE setting_key = @key";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@key", settingKey);
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return MapSetting(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving setting: {ex.Message}", ex);
            }

            return null;
        }

        /// <summary>
        /// Get settings by category
        /// </summary>
        public async Task<List<SystemSetting>> GetByCategoryAsync(string category)
        {
            var settings = new List<SystemSetting>();

            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    await connection.OpenAsync();
                    string query = "SELECT * FROM system_settings WHERE setting_category = @category ORDER BY setting_key";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@category", category);
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                settings.Add(MapSetting(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving settings by category: {ex.Message}", ex);
            }

            return settings;
        }

        /// <summary>
        /// Update a setting value
        /// </summary>
        public async Task<bool> UpdateSettingAsync(string settingKey, string newValue, int? updatedBy = null)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(settingKey))
                {
                    throw new ArgumentException("Setting key cannot be empty", nameof(settingKey));
                }

                if (newValue == null)
                {
                    throw new ArgumentException("Setting value cannot be null", nameof(newValue));
                }

                using (var connection = await DatabaseHelper.GetConnectionWithRetryAsync())
                {
                    string query = @"UPDATE system_settings 
                                    SET setting_value = @value, 
                                        updated_by = @updatedBy, 
                                        updated_at = CURRENT_TIMESTAMP 
                                    WHERE setting_key = @key";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@key", settingKey);
                        command.Parameters.AddWithValue("@value", newValue);
                        command.Parameters.AddWithValue("@updatedBy", updatedBy ?? (object)DBNull.Value);

                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        
                        if (rowsAffected > 0)
                        {
                            await ErrorLoggingService.LogInfoAsync(
                                "Update Setting - Success",
                                $"Updated setting: {settingKey} = {newValue}",
                                "system_settings");
                        }
                        
                        return rowsAffected > 0;
                    }
                }
            }
            catch (MySqlException ex)
            {
                await ErrorLoggingService.LogErrorAsync(
                    "Update Setting - Database Error",
                    ex,
                    "system_settings");
                throw new Exception(ErrorLoggingService.GetUserFriendlyMessage(ex), ex);
            }
            catch (Exception ex)
            {
                await ErrorLoggingService.LogErrorAsync(
                    "Update Setting - Error",
                    ex,
                    "system_settings");
                throw new Exception($"Error updating setting: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Save multiple settings at once
        /// </summary>
        public async Task<bool> SaveSettingsAsync(Dictionary<string, string> settings, int? updatedBy = null)
        {
            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    await connection.OpenAsync();
                    using (var transaction = await connection.BeginTransactionAsync())
                    {
                        try
                        {
                            string query = @"UPDATE system_settings 
                                            SET setting_value = @value, 
                                                updated_by = @updatedBy, 
                                                updated_at = CURRENT_TIMESTAMP 
                                            WHERE setting_key = @key";

                            foreach (var kvp in settings)
                            {
                                using (var command = new MySqlCommand(query, connection, transaction))
                                {
                                    command.Parameters.AddWithValue("@key", kvp.Key);
                                    command.Parameters.AddWithValue("@value", kvp.Value);
                                    command.Parameters.AddWithValue("@updatedBy", updatedBy ?? (object)DBNull.Value);
                                    await command.ExecuteNonQueryAsync();
                                }
                            }

                            await transaction.CommitAsync();
                            return true;
                        }
                        catch
                        {
                            await transaction.RollbackAsync();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error saving settings: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Get setting value as string
        /// </summary>
        public async Task<string> GetValueAsync(string settingKey, string defaultValue = null)
        {
            var setting = await GetByKeyAsync(settingKey);
            return setting?.SettingValue ?? defaultValue;
        }

        /// <summary>
        /// Get setting value as integer
        /// </summary>
        public async Task<int> GetIntValueAsync(string settingKey, int defaultValue = 0)
        {
            var value = await GetValueAsync(settingKey);
            return int.TryParse(value, out int result) ? result : defaultValue;
        }

        /// <summary>
        /// Get setting value as boolean
        /// </summary>
        public async Task<bool> GetBoolValueAsync(string settingKey, bool defaultValue = false)
        {
            var value = await GetValueAsync(settingKey);
            if (string.IsNullOrEmpty(value)) return defaultValue;

            value = value.ToLower();
            return value == "true" || value == "1" || value == "yes" || value == "on";
        }

        /// <summary>
        /// Test database connection
        /// </summary>
        public async Task<bool> TestConnectionAsync()
        {
            return await DatabaseHelper.TestConnectionAsync();
        }

        private SystemSetting MapSetting(MySqlDataReader reader)
        {
            return new SystemSetting
            {
                SettingId = reader.GetInt32("setting_id"),
                SettingKey = reader.GetString("setting_key"),
                SettingValue = reader.GetString("setting_value"),
                SettingCategory = reader.GetString("setting_category"),
                Description = reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString("description"),
                UpdatedBy = reader.IsDBNull(reader.GetOrdinal("updated_by")) ? (int?)null : reader.GetInt32("updated_by"),
                UpdatedAt = reader.IsDBNull(reader.GetOrdinal("updated_at")) ? null : (DateTime?)reader.GetDateTime("updated_at")
            };
        }
    }
}
