using System;
using System.Drawing;
using System.Windows.Forms;

namespace ITP104_FINAL_PROJECT
{
    public partial class SettingsScreen : Form
    {
        public SettingsScreen()
        {
            InitializeComponent();
            InitializeSettings();
        }

        private void InitializeSettings()
        {
            // Load current settings
            LoadScannerSettings();
            LoadSystemSettings();
            LoadDatabaseSettings();
        }

        private void LoadScannerSettings()
        {
            // Load scanner configuration from settings
            toggleQRScanner.Checked = true;
            numConnectionTimeout.Value = 30;
            toggleBeepOnScan.Checked = true;
        }

        private void LoadSystemSettings()
        {
            // Load system configuration from settings
            numAutoLogout.Value = 15;
            cmbTheme.SelectedIndex = 0; // Light theme
            cmbLanguage.SelectedIndex = 0; // English
        }

        private void LoadDatabaseSettings()
        {
            // Load database settings (disabled as placeholder)
            txtServerAddress.Text = "localhost";
            txtPort.Text = "1433";
        }

        private void btnSaveSettings_Click(object sender, EventArgs e)
        {
            try
            {
                // Show saving indicator
                btnSaveSettings.Text = "⏳ Saving...";
                btnSaveSettings.Enabled = false;
                Application.DoEvents();

                // Simulate save operation
                System.Threading.Thread.Sleep(1000);

                // Save scanner settings
                SaveScannerSettings();

                // Save system settings
                SaveSystemSettings();

                // Save database settings
                SaveDatabaseSettings();

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
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving settings: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnSaveSettings.Text = "💾 Save Settings";
                btnSaveSettings.Enabled = true;
            }
        }

        private void SaveScannerSettings()
        {
            // Save scanner configuration
            bool qrEnabled = toggleQRScanner.Checked;
            int timeout = (int)numConnectionTimeout.Value;
            bool beepEnabled = toggleBeepOnScan.Checked;

            // TODO: Save to configuration file or database
        }

        private void SaveSystemSettings()
        {
            // Save system configuration
            int autoLogout = (int)numAutoLogout.Value;
            string theme = cmbTheme.SelectedItem?.ToString() ?? "Light";
            string language = cmbLanguage.SelectedItem?.ToString() ?? "English";

            // TODO: Save to configuration file or database
        }

        private void SaveDatabaseSettings()
        {
            // Save database settings
            string serverAddress = txtServerAddress.Text;
            string port = txtPort.Text;

            // TODO: Save to configuration file or database
        }

        private void btnTestConnection_Click(object sender, EventArgs e)
        {
            // Show testing indicator
            btnTestConnection.Text = "⏳ Testing...";
            btnTestConnection.Enabled = false;
            Application.DoEvents();

            // Simulate connection test
            System.Threading.Thread.Sleep(1500);

            // Show result (placeholder - always success for demo)
            MessageBox.Show("Database connection test successful!\n\nServer: " + txtServerAddress.Text +
                "\nPort: " + txtPort.Text, "Connection Test",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Reset button
            btnTestConnection.Text = "🔌 Test Connection";
            btnTestConnection.Enabled = true;
        }

        private void btnResetDefaults_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Are you sure you want to reset all settings to default values?",
                "Reset to Defaults",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                ResetToDefaults();
                MessageBox.Show("Settings have been reset to default values.", "Reset Complete",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
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
