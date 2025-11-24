using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using ITP104_FINAL_PROJECT.Data;

namespace ITP104_FINAL_PROJECT
{
    public partial class SettingsScreen : Form
    {
        private readonly SettingsRepository settingsRepository;

        public SettingsScreen()
        {
            InitializeComponent();
            settingsRepository = new SettingsRepository();
            InitializeSettings();
        }

        private async void InitializeSettings()
        {
            try
            {
                // Show loading indicator
                this.Cursor = Cursors.WaitCursor;
                
                // Load current settings from database
                await LoadScannerSettingsAsync();
                await LoadSystemSettingsAsync();
                LoadDatabaseSettings();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading settings: {ex.Message}\n\nDefault values will be used.",
                    "Settings Load Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                
                // Load default values on error
                LoadDefaultSettings();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private async Task LoadScannerSettingsAsync()
        {
            // Load scanner configuration from database
            toggleQRScanner.Checked = await settingsRepository.GetBoolValueAsync("qr_scanner_enabled", true);
            numConnectionTimeout.Value = await settingsRepository.GetIntValueAsync("connection_timeout", 30);
            toggleBeepOnScan.Checked = await settingsRepository.GetBoolValueAsync("beep_on_scan", true);
        }

        private async Task LoadSystemSettingsAsync()
        {
            // Load system configuration from database
            numAutoLogout.Value = await settingsRepository.GetIntValueAsync("auto_logout_timer", 15);
            
            // Load theme setting
            string theme = await settingsRepository.GetValueAsync("theme", "Light");
            int themeIndex = cmbTheme.Items.IndexOf(theme);
            cmbTheme.SelectedIndex = themeIndex >= 0 ? themeIndex : 0;
            
            // Load language setting
            string language = await settingsRepository.GetValueAsync("language", "English");
            int langIndex = cmbLanguage.Items.IndexOf(language);
            cmbLanguage.SelectedIndex = langIndex >= 0 ? langIndex : 0;
        }

        private void LoadDatabaseSettings()
        {
            // Database settings are read-only (system-level configuration)
            // Display current connection information from DatabaseHelper
            txtServerAddress.Text = "localhost";
            txtPort.Text = "3306"; // MySQL default port
        }

        private void LoadDefaultSettings()
        {
            // Load default values when database is unavailable
            toggleQRScanner.Checked = true;
            numConnectionTimeout.Value = 30;
            toggleBeepOnScan.Checked = true;
            numAutoLogout.Value = 15;
            cmbTheme.SelectedIndex = 0;
            cmbLanguage.SelectedIndex = 0;
            txtServerAddress.Text = "localhost";
            txtPort.Text = "3306";
        }

        private async void btnSaveSettings_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate settings before saving
                if (!ValidateSettings())
                {
                    return;
                }

                // Show saving indicator
                btnSaveSettings.Text = "⏳ Saving...";
                btnSaveSettings.Enabled = false;
                this.Cursor = Cursors.WaitCursor;
                Application.DoEvents();

                // Save all settings to database
                bool success = await SaveAllSettingsAsync();

                if (success)
                {
                    // Show success message
                    btnSaveSettings.Text = "✅ Saved!";
                    MessageBox.Show("Settings saved successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Reset button after delay
                    Timer resetTimer = new Timer { Interval = 2000 };
                    resetTimer.Tick += (s, args) =>
                    {
                        btnSaveSettings.Text = "💾 Save Settings";
                        btnSaveSettings.Enabled = true;
                        resetTimer.Stop();
                        resetTimer.Dispose();
                    };
                    resetTimer.Start();
                }
                else
                {
                    btnSaveSettings.Text = "💾 Save Settings";
                    btnSaveSettings.Enabled = true;
                    MessageBox.Show("Some settings could not be saved. Please try again.", "Warning",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving settings: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnSaveSettings.Text = "💾 Save Settings";
                btnSaveSettings.Enabled = true;
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private async Task<bool> SaveAllSettingsAsync()
        {
            try
            {
                // Prepare settings dictionary for batch update
                var settings = new System.Collections.Generic.Dictionary<string, string>();

                // Scanner settings
                settings["qr_scanner_enabled"] = toggleQRScanner.Checked.ToString().ToLower();
                settings["connection_timeout"] = numConnectionTimeout.Value.ToString();
                settings["beep_on_scan"] = toggleBeepOnScan.Checked.ToString().ToLower();

                // System settings
                settings["auto_logout_timer"] = numAutoLogout.Value.ToString();
                settings["theme"] = cmbTheme.SelectedItem?.ToString() ?? "Light";
                settings["language"] = cmbLanguage.SelectedItem?.ToString() ?? "English";

                // Save all settings in a single transaction
                return await settingsRepository.SaveSettingsAsync(settings);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to save settings: {ex.Message}", ex);
            }
        }

        private bool ValidateSettings()
        {
            // Validate connection timeout (5-120 seconds)
            if (numConnectionTimeout.Value < 5 || numConnectionTimeout.Value > 120)
            {
                MessageBox.Show("Connection timeout must be between 5 and 120 seconds.",
                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                numConnectionTimeout.Focus();
                return false;
            }

            // Validate auto-logout timer (5-60 minutes)
            if (numAutoLogout.Value < 5 || numAutoLogout.Value > 60)
            {
                MessageBox.Show("Auto-logout timer must be between 5 and 60 minutes.",
                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                numAutoLogout.Focus();
                return false;
            }

            // Validate theme selection
            if (cmbTheme.SelectedIndex < 0)
            {
                MessageBox.Show("Please select a theme.",
                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbTheme.Focus();
                return false;
            }

            // Validate language selection
            if (cmbLanguage.SelectedIndex < 0)
            {
                MessageBox.Show("Please select a language.",
                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbLanguage.Focus();
                return false;
            }

            return true;
        }

        private async void btnTestConnection_Click(object sender, EventArgs e)
        {
            try
            {
                // Show testing indicator
                btnTestConnection.Text = "⏳ Testing...";
                btnTestConnection.Enabled = false;
                this.Cursor = Cursors.WaitCursor;
                Application.DoEvents();

                // Test actual database connection
                bool connectionSuccess = await settingsRepository.TestConnectionAsync();

                if (connectionSuccess)
                {
                    MessageBox.Show("Database connection test successful!\n\n" +
                        "Server: " + txtServerAddress.Text + "\n" +
                        "Port: " + txtPort.Text + "\n" +
                        "Database: student_attendance_db",
                        "Connection Test", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Database connection test failed.\n\n" +
                        "Please check your database server and try again.",
                        "Connection Test", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Connection test error: {ex.Message}",
                    "Connection Test", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Reset button
                btnTestConnection.Text = "🔌 Test Connection";
                btnTestConnection.Enabled = true;
                this.Cursor = Cursors.Default;
            }
        }

        private void btnResetDefaults_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Are you sure you want to reset all settings to default values?\n\n" +
                "This will restore:\n" +
                "• QR Scanner: Enabled\n" +
                "• Connection Timeout: 30 seconds\n" +
                "• Beep on Scan: Enabled\n" +
                "• Auto-Logout Timer: 15 minutes\n" +
                "• Theme: Light\n" +
                "• Language: English\n\n" +
                "You will need to click 'Save Settings' to apply these changes.",
                "Reset to Defaults",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                ResetToDefaults();
                MessageBox.Show("Settings have been reset to default values.\n\n" +
                    "Click 'Save Settings' to apply these changes.",
                    "Reset Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ResetToDefaults()
        {
            // Scanner settings
            toggleQRScanner.Checked = true;
            numConnectionTimeout.Value = 30;
            toggleBeepOnScan.Checked = true;

            // System settings
            numAutoLogout.Value = 15;
            cmbTheme.SelectedIndex = 0;
            cmbLanguage.SelectedIndex = 0;

            // Database settings
            txtServerAddress.Text = "localhost";
            txtPort.Text = "1433";
        }

        private void cmbTheme_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Preview theme change (optional)
            string selectedTheme = cmbTheme.SelectedItem?.ToString();
            lblThemePreview.Text = $"Theme preview: {selectedTheme}";
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
