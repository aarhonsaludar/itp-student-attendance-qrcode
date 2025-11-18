using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using QRCoder;

namespace ITP104_FINAL_PROJECT
{
    public partial class MainDashboard : Form
    {
        private string currentUser = "Admin";
        private Timer statusUpdateTimer;
        private Random random = new Random();

        // Panel references for navigation
        private Panel pnlDashboardContent;
        private Panel pnlRegisterStudentContent;
        private Panel pnlScanContent;
        private Panel pnlStudentRecordsContent;
        private Panel pnlScanHistoryContent;
        private Panel pnlSettingsContent;

        // Current active panel
        private Panel currentPanel;

        // Page titles for breadcrumb navigation - CORRECTED ORDER
        private Dictionary<int, string> pageTitles = new Dictionary<int, string>
        {
            { 0, "Dashboard" },
            { 1, "Register Student" },
            { 2, "Scan QR" },
            { 3, "Student Records" },
            { 4, "Scan History" },
            { 5, "Settings" }
        };

        // Page descriptions for enhanced UX - CORRECTED ORDER
        private Dictionary<int, string> pageDescriptions = new Dictionary<int, string>
        {
            { 0, "View system overview and recent activity" },
            { 1, "Register new students into the system" },
            { 2, "Scan student QR codes" },
            { 3, "Browse and manage student records" },
            { 4, "View complete history of scanned cards" },
            { 5, "Configure system settings" }
        };

        public MainDashboard()
        {
            InitializeComponent();
            InitializeDashboard();
        }

        private void InitializeDashboard()
        {
            // Set user information
            lblUserName.Text = $"User: {currentUser}";

            // Initialize panels for navigation
            InitializePanels();

            // Initialize status indicators
            UpdateSystemStatus();

            // Load dashboard statistics
            LoadDashboardStats();

            // Load recent scans
            LoadRecentScans();

            // Setup button click events
            SetupEventHandlers();

            // Start status update timer
            InitializeStatusTimer();

            // Show dashboard by default
            ShowPanel(pnlDashboardContent);
            UpdateNavIndicator(0);
        }

        private void InitializePanels()
        {
            // Hide the TabControl - we'll use panels instead
            tabControlMain.Visible = false;

            // Initialize Dashboard Panel (already exists in tabDashboard)
            pnlDashboardContent = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.FromArgb(240, 242, 245)
            };

            // Move existing dashboard controls to the new panel
            MoveDashboardControlsToPanel();

            // Initialize Register Student Panel
            pnlRegisterStudentContent = CreateRegisterStudentPanel();

            // Initialize Scan Panel
            pnlScanContent = CreateScanPanel();

            // Initialize Student Records Panel
            pnlStudentRecordsContent = CreateStudentRecordsPanel();

            // Initialize Scan History Panel
            pnlScanHistoryContent = CreateScanHistoryPanel();

            // Initialize Settings Panel
            pnlSettingsContent = CreateSettingsPanel();

            // Add all panels to main content
            pnlMainContent.Controls.Add(pnlDashboardContent);
            pnlMainContent.Controls.Add(pnlRegisterStudentContent);
            pnlMainContent.Controls.Add(pnlScanContent);
            pnlMainContent.Controls.Add(pnlStudentRecordsContent);
            pnlMainContent.Controls.Add(pnlScanHistoryContent);
            pnlMainContent.Controls.Add(pnlSettingsContent);

            // Hide all panels initially
            HideAllPanels();
        }

        private void MoveDashboardControlsToPanel()
        {
            // Move controls from tabDashboard to pnlDashboardContent
            var controlsToMove = tabDashboard.Controls.Cast<Control>().ToList();
            foreach (var control in controlsToMove)
            {
                tabDashboard.Controls.Remove(control);
                pnlDashboardContent.Controls.Add(control);
            }
        }

        private Panel CreateRegisterStudentPanel()
        {
            Panel panel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = false,
                BackColor = Color.FromArgb(240, 242, 245),
                Padding = new Padding(0)
            };

            // Create an instance of StudentRegistration form
            StudentRegistration registrationForm = new StudentRegistration
            {
                TopLevel = false,
                FormBorderStyle = FormBorderStyle.None,
                Dock = DockStyle.Fill,
                AutoScroll = true
            };

            // Add the form to the panel first
            panel.Controls.Add(registrationForm);

            // Show the form
            registrationForm.Show();

            // Hide the header panel (guna2Panel1) after the form is shown
            foreach (Control ctrl in registrationForm.Controls)
            {
                if (ctrl is Guna.UI2.WinForms.Guna2Panel && ctrl.BackColor == Color.LightSeaGreen)
                {
                    ctrl.Visible = false;
                    break;
                }
            }

            return panel;
        }

        private Panel CreateScanPanel()
        {
            Panel panel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.FromArgb(240, 242, 245),
                Padding = new Padding(30)
            };

            // Title
            Label lblTitle = new Label
            {
                Text = "📱 Scan QR Code",
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64),
                Location = new Point(30, 20),
                AutoSize = true
            };

            Label lblDescription = new Label
            {
                Text = "Scan student QR codes for attendance or identification",
                Font = new Font("Segoe UI", 11F),
                ForeColor = Color.FromArgb(100, 100, 100),
                Location = new Point(30, 55),
                AutoSize = true
            };

            // Button to open full scanner
            Guna.UI2.WinForms.Guna2Button btnOpenScanner = new Guna.UI2.WinForms.Guna2Button
            {
                Text = "🔍 Open QR Scanner",
                Location = new Point(30, 110),
                Size = new Size(250, 60),
                BorderRadius = 10,
                FillColor = Color.FromArgb(76, 175, 80),
                Font = new Font("Segoe UI Semibold", 12F),
                ForeColor = Color.White
            };
            btnOpenScanner.Click += (s, e) =>
            {
                // Open the camera scanner form
                CameraScannerForm scannerForm = new CameraScannerForm();
                scannerForm.ShowDialog();
            };

            panel.Controls.Add(lblTitle);
            panel.Controls.Add(lblDescription);
            panel.Controls.Add(btnOpenScanner);

            return panel;
        }

        private Panel CreateStudentRecordsPanel()
        {
            Panel panel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.FromArgb(240, 242, 245),
                Padding = new Padding(30)
            };

            // Create a container panel to center content
            Panel centerContainer = new Panel
            {
                AutoSize = true,
                BackColor = Color.Transparent
            };

            Label lblTitle = new Label
            {
                Text = "👥 Student Records",
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64),
                Location = new Point(0, 0),
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label lblDescription = new Label
            {
                Text = "Browse and manage student records",
                Font = new Font("Segoe UI", 11F),
                ForeColor = Color.FromArgb(100, 100, 100),
                Location = new Point(0, 40),
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label lblComingSoon = new Label
            {
                Text = "📋 This section is under development.\nStudent records management will be available here.",
                Font = new Font("Segoe UI", 10F, FontStyle.Italic),
                ForeColor = Color.FromArgb(150, 150, 150),
                Location = new Point(0, 85),
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Add labels to center container
            centerContainer.Controls.Add(lblTitle);
            centerContainer.Controls.Add(lblDescription);
            centerContainer.Controls.Add(lblComingSoon);

            // Calculate center position after adding controls
            panel.Resize += (s, e) =>
            {
                centerContainer.Left = (panel.Width - centerContainer.Width) / 2;
                centerContainer.Top = (panel.Height - centerContainer.Height) / 2;
            };

            panel.Controls.Add(centerContainer);

            // Trigger initial centering
            centerContainer.SizeChanged += (s, e) =>
            {
                centerContainer.Left = (panel.Width - centerContainer.Width) / 2;
                centerContainer.Top = (panel.Height - centerContainer.Height) / 2;
            };

            return panel;
        }

        private Panel CreateScanHistoryPanel()
        {
            Panel panel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.FromArgb(240, 242, 245),
                Padding = new Padding(30)
            };

            Label lblTitle = new Label
            {
                Text = "📜 Scan History",
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64),
                Location = new Point(30, 20),
                AutoSize = true
            };

            Label lblDescription = new Label
            {
                Text = "View complete history of scanned cards",
                Font = new Font("Segoe UI", 11F),
                ForeColor = Color.FromArgb(100, 100, 100),
                Location = new Point(30, 55),
                AutoSize = true
            };

            Label lblComingSoon = new Label
            {
                Text = "📊 This section is under development.\nComplete scan history will be available here.",
                Font = new Font("Segoe UI", 10F, FontStyle.Italic),
                ForeColor = Color.FromArgb(150, 150, 150),
                Location = new Point(30, 100),
                AutoSize = true
            };

            panel.Controls.Add(lblTitle);
            panel.Controls.Add(lblDescription);
            panel.Controls.Add(lblComingSoon);

            return panel;
        }

        private Panel CreateSettingsPanel()
        {
            Panel panel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.FromArgb(240, 242, 245),
                Padding = new Padding(30)
            };

            Label lblTitle = new Label
            {
                Text = "⚙️ Settings",
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64),
                Location = new Point(30, 20),
                AutoSize = true
            };

            Label lblDescription = new Label
            {
                Text = "Configure system settings and preferences",
                Font = new Font("Segoe UI", 11F),
                ForeColor = Color.FromArgb(100, 100, 100),
                Location = new Point(30, 55),
                AutoSize = true
            };

            Label lblComingSoon = new Label
            {
                Text = "🔧 This section is under development.\nSystem configuration options will be available here.",
                Font = new Font("Segoe UI", 10F, FontStyle.Italic),
                ForeColor = Color.FromArgb(150, 150, 150),
                Location = new Point(30, 100),
                AutoSize = true
            };

            panel.Controls.Add(lblTitle);
            panel.Controls.Add(lblDescription);
            panel.Controls.Add(lblComingSoon);

            return panel;
        }

        private void HideAllPanels()
        {
            pnlDashboardContent.Visible = false;
            pnlRegisterStudentContent.Visible = false;
            pnlScanContent.Visible = false;
            pnlStudentRecordsContent.Visible = false;
            pnlScanHistoryContent.Visible = false;
            pnlSettingsContent.Visible = false;
        }

        private void ShowPanel(Panel panel)
        {
            // Hide current panel
            if (currentPanel != null && currentPanel != panel)
            {
                currentPanel.Visible = false;
            }

            // Show new panel
            panel.Visible = true;
            panel.BringToFront();
            currentPanel = panel;

            // Refresh the panel
            panel.Refresh();
        }

        private void SetupEventHandlers()
        {
            // Quick action buttons
            btnStartScan.Click += BtnStartScan_Click;
            btnStudentRecords.Click += BtnStudentRecords_Click;
            btnReports.Click += BtnReports_Click;

            // Logout buttons
            btnLogout.Click += BtnLogout_Click;
            btnNavLogout.Click += BtnLogout_Click;

            // Navigation buttons with correct mapping
            btnNavDashboard.Click += (s, e) => { NavigateToPage(0); };
            // btnNavRegisterStudent.Click is handled by its own event handler
            btnNavScanQr.Click += (s, e) => { NavigateToPage(2); };
            btnNavStudentRecords.Click += (s, e) => { NavigateToPage(3); };
            btnNavScanHistory.Click += (s, e) => { NavigateToPage(4); };
            btnNavSettings.Click += (s, e) => { NavigateToPage(5); };

            // Button hover effects
            SetupButtonHoverEffects();

            // Navigation button styles
            SetupNavigationButtonStyles();
        }

        /// <summary>
        /// Navigate to a specific page with smooth transitions and breadcrumb updates
        /// </summary>
        private void NavigateToPage(int pageIndex)
        {
            try
            {
                // Determine which panel to show - CORRECTED MAPPING
                Panel targetPanel = null;
                switch (pageIndex)
                {
                    case 0: // Dashboard
                        targetPanel = pnlDashboardContent;
                        break;
                    case 1: // Register Student
                        targetPanel = pnlRegisterStudentContent;
                        break;
                    case 2: // Scan QR
                        targetPanel = pnlScanContent;
                        break;
                    case 3: // Student Records
                        targetPanel = pnlStudentRecordsContent;
                        break;
                    case 4: // Scan History
                        targetPanel = pnlScanHistoryContent;
                        break;
                    case 5: // Settings
                        targetPanel = pnlSettingsContent;
                        break;
                    default:
                        targetPanel = pnlDashboardContent;
                        break;
                }

                // Show the panel
                if (targetPanel != null)
                {
                    ShowPanel(targetPanel);
                    UpdateNavIndicator(pageIndex);
                }

                // Update breadcrumb information
                UpdateBreadcrumb(pageIndex);

                // Log navigation
                System.Diagnostics.Debug.WriteLine($"Navigated to: {pageTitles[pageIndex]}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Navigation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Update breadcrumb information (can be extended to show in UI)
        /// </summary>
        private void UpdateBreadcrumb(int pageIndex)
        {
            try
            {
                string pageTitle = pageTitles.ContainsKey(pageIndex) ? pageTitles[pageIndex] : "Unknown";
                string pageDescription = pageDescriptions.ContainsKey(pageIndex) ? pageDescriptions[pageIndex] : "";

                System.Diagnostics.Debug.WriteLine($"Page: {pageTitle} - {pageDescription}");
            }
            catch { /* Prevent errors during breadcrumb updates */ }
        }

        private void SetupNavigationButtonStyles()
        {
            // Add hover effects to navigation buttons with enhanced visual feedback
            var navButtons = new[] { btnNavDashboard, btnNavRegisterStudent, btnNavScanQr, btnNavStudentRecords, btnNavScanHistory, btnNavSettings };

            foreach (var btn in navButtons)
            {
                // Store original colors
                Color originalColor = Color.FromArgb(33, 42, 57);
                Color hoverColor = Color.FromArgb(50, 65, 85);

                btn.MouseEnter += (s, e) =>
                {
                    btn.FillColor = hoverColor;
                    btn.Cursor = Cursors.Hand;
                };

                btn.MouseLeave += (s, e) =>
                {
                    btn.FillColor = originalColor;
                };
            }

            // Special styling for logout button
            btnNavLogout.MouseEnter += (s, e) =>
            {
                btnNavLogout.FillColor = Color.FromArgb(240, 73, 89); // Lighter red on hover
                btnNavLogout.Cursor = Cursors.Hand;
            };

            btnNavLogout.MouseLeave += (s, e) =>
            {
                btnNavLogout.FillColor = Color.FromArgb(220, 53, 69); // Keep red
            };
        }

        private void UpdateNavIndicator(int navIndex)
        {
            // Navigation indicator removed - no visual indicator needed
            // Buttons still change color on hover for feedback
        }

        private void SetupButtonHoverEffects()
        {
            // Start Scan button
            btnStartScan.MouseEnter += (s, e) =>
            {
                btnStartScan.FillColor = Color.FromArgb(100, 200, 100);
            };
            btnStartScan.MouseLeave += (s, e) =>
            {
                btnStartScan.FillColor = Color.FromArgb(76, 175, 80);
            };

            // Student Records button
            btnStudentRecords.MouseEnter += (s, e) =>
            {
                btnStudentRecords.FillColor = Color.FromArgb(130, 170, 220);
            };
            btnStudentRecords.MouseLeave += (s, e) =>
            {
                btnStudentRecords.FillColor = Color.FromArgb(100, 150, 200);
            };

            // Reports button
            btnReports.MouseEnter += (s, e) =>
            {
                btnReports.FillColor = Color.FromArgb(255, 170, 30);
            };
            btnReports.MouseLeave += (s, e) =>
            {
                btnReports.FillColor = Color.FromArgb(255, 152, 0);
            };

            // Card hover effects
            SetupCardHoverEffects();
        }

        private void SetupCardHoverEffects()
        {
            // Add hover effects to stat cards for visual feedback
            var statCards = new[] { pnlTotalStudents, pnlScansToday, pnlMostUsedScan, pnlScannerModes };

            foreach (var card in statCards)
            {
                card.MouseEnter += (s, e) =>
                {
                    card.ShadowDecoration.Depth = 15;
                    card.Cursor = Cursors.Hand;
                };

                card.MouseLeave += (s, e) =>
                {
                    card.ShadowDecoration.Depth = 8;
                };
            }
        }

        private void InitializeStatusTimer()
        {
            statusUpdateTimer = new Timer();
            statusUpdateTimer.Interval = 5000; // Update every 5 seconds
            statusUpdateTimer.Tick += StatusUpdateTimer_Tick;
            statusUpdateTimer.Start();
        }

        private void StatusUpdateTimer_Tick(object sender, EventArgs e)
        {
            // Simulate real-time updates
            UpdateSystemStatus();
        }

        private void UpdateSystemStatus()
        {
            try
            {
                // Scanner status
                bool scannerActive = random.Next(0, 10) > 2; // 80% chance active
                lblScannerStatus.Text = scannerActive ? "● Scanner: Ready" : "● Scanner: Idle";
                lblScannerStatus.ForeColor = scannerActive ? Color.Lime : Color.Gray;

                // Database status
                lblDatabaseStatus.Text = "● Database: Connected";
                lblDatabaseStatus.ForeColor = Color.Lime;

                // Scanner type status
                lblQRStatus.Text = "● QR Code: Active";
                lblQRStatus.ForeColor = Color.Green;

            }
            catch { /* Prevent errors during UI updates */ }
        }

        private void LoadDashboardStats()
        {
            try
            {
                // Simulate loading statistics from database
                lblTotalStudentsValue.Text = "1,247";
                lblScansTodayValue.Text = "89";
                lblMostUsedScanValue.Text = "QR Code";
            }
            catch { /* Prevent errors during UI updates */ }
        }

        private void LoadRecentScans()
        {
            try
            {
                // Create columns
                dgvRecentScans.Columns.Clear();
                dgvRecentScans.Columns.Add("StudentID", "Student ID");
                dgvRecentScans.Columns.Add("Name", "Name");
                dgvRecentScans.Columns.Add("ScanType", "Scan Type");
                dgvRecentScans.Columns.Add("Time", "Time");

                // Set column widths
                dgvRecentScans.Columns["StudentID"].Width = 200;
                dgvRecentScans.Columns["Name"].Width = 300;
                dgvRecentScans.Columns["ScanType"].Width = 200;
                dgvRecentScans.Columns["Time"].Width = 250;

                // Add sample data
                dgvRecentScans.Rows.Add("2024-STU-0089", "John Smith", "QR Code", DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt"));
                dgvRecentScans.Rows.Add("2024-STU-0088", "Emily Johnson", "Barcode", DateTime.Now.AddMinutes(-5).ToString("MM/dd/yyyy hh:mm:ss tt"));
                dgvRecentScans.Rows.Add("2024-STU-0087", "Michael Brown", "QR Code", DateTime.Now.AddMinutes(-12).ToString("MM/dd/yyyy hh:mm:ss tt"));
                dgvRecentScans.Rows.Add("2024-STU-0086", "Sarah Davis", "Barcode", DateTime.Now.AddMinutes(-18).ToString("MM/dd/yyyy hh:mm:ss tt"));
                dgvRecentScans.Rows.Add("2024-STU-0085", "David Wilson", "QR Code", DateTime.Now.AddMinutes(-25).ToString("MM/dd/yyyy hh:mm:ss tt"));
            }
            catch { /* Prevent errors during UI updates */ }
        }

        private void BtnStartScan_Click(object sender, EventArgs e)
        {
            // Navigate to Scan panel
            NavigateToPage(2); // Index 2 = Scan QR
        }

        private void BtnStudentRecords_Click(object sender, EventArgs e)
        {
            // Navigate to Student Records panel
            NavigateToPage(3); // Index 3 = Student Records
        }

        private void BtnReports_Click(object sender, EventArgs e)
        {
            // Navigate to Scan History panel
            NavigateToPage(4); // Index 4 = Scan History
        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            HandleLogout();
        }

        private void HandleLogout()
        {
            var result = MessageBox.Show(
                "Are you sure you want to logout?",
                "Confirm Logout",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                // Stop timer
                if (statusUpdateTimer != null)
                {
                    statusUpdateTimer.Stop();
                    statusUpdateTimer.Dispose();
                }

                // Show login screen
                this.Hide();
                LoginScreen loginScreen = new LoginScreen();
                loginScreen.FormClosed += (s, args) => this.Close();
                loginScreen.Show();
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            // Cleanup
            if (statusUpdateTimer != null)
            {
                statusUpdateTimer.Stop();
                statusUpdateTimer.Dispose();
            }
        }

        // Public method to set user
        public void SetUser(string username)
        {
            currentUser = username;
            lblUserName.Text = $"User: {currentUser}";
        }

        // Method to refresh dashboard data
        public void RefreshDashboard()
        {
            LoadDashboardStats();
            LoadRecentScans();
            UpdateSystemStatus();
        }

        // Method to add new scan to recent scans
        public void AddScanToHistory(string studentId, string name, string scanType)
        {
            try
            {
                if (dgvRecentScans.Rows.Count >= 10)
                {
                    dgvRecentScans.Rows.RemoveAt(dgvRecentScans.Rows.Count - 1);
                }

                dgvRecentScans.Rows.Insert(0, studentId, name, scanType, DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt"));

                // Update scans today count
                if (int.TryParse(lblScansTodayValue.Text, out int currentScans))
                {
                    lblScansTodayValue.Text = (currentScans + 1).ToString();
                }
            }
            catch { /* Prevent errors during UI updates */ }
        }

        /// <summary>
        /// Show loading state indicator on a button
        /// </summary>
        public void SetButtonLoading(Guna.UI2.WinForms.Guna2Button button, bool isLoading)
        {
            try
            {
                if (isLoading)
                {
                    button.Text = "⏳ Loading...";
                    button.Enabled = false;
                    button.FillColor = Color.FromArgb(200, 200, 200);
                }
                else
                {
                    button.Enabled = true;
                    // Text and color will be set by caller or event handler
                }
            }
            catch { /* Prevent errors */ }
        }

        /// <summary>
        /// Get page title for current panel
        /// </summary>
        public string GetCurrentPageTitle()
        {
            if (currentPanel == pnlDashboardContent) return "Dashboard";
            if (currentPanel == pnlRegisterStudentContent) return "Register Student";
            if (currentPanel == pnlScanContent) return "Scan QR";
            if (currentPanel == pnlStudentRecordsContent) return "Student Records";
            if (currentPanel == pnlScanHistoryContent) return "Scan History";
            if (currentPanel == pnlSettingsContent) return "Settings";
            return "Unknown";
        }

        /// <summary>
        /// Get page description for current panel
        /// </summary>
        public string GetCurrentPageDescription()
        {
            string title = GetCurrentPageTitle();
            foreach (var kvp in pageTitles)
            {
                if (kvp.Value == title && pageDescriptions.ContainsKey(kvp.Key))
                {
                    return pageDescriptions[kvp.Key];
                }
            }
            return "";
        }

        // Designer-generated event handlers
        private void btnNavScanID_Click(object sender, EventArgs e)
        {

        }

        private void btnNavDashboard_Click(object sender, EventArgs e)
        {
            // Handled by lambda in SetupEventHandlers
        }

        private void tabReports_Click(object sender, EventArgs e)
        {
            // Handled by lambda in SetupEventHandlers
        }

        private void btnNavReports_Click(object sender, EventArgs e)
        {
            // Open Scan History Screen
            ScanHistoryScreen historyScreen = new ScanHistoryScreen();
            historyScreen.Show();

            // Refresh data
            historyScreen.RefreshData();
        }

        private void btnNavSettings_Click(object sender, EventArgs e)
        {
            // Handled by lambda in SetupEventHandlers
        }

        private void btnNavLogout_Click(object sender, EventArgs e)
        {
            // Handled by lambda in SetupEventHandlers
        }

        private void btnNavRegisterStudent_Click(object sender, EventArgs e)
        {
            // Hide the MainDashboard form
            this.Hide();

            // Create and show the StudentRegistration form
            StudentRegistration registrationForm = new StudentRegistration();

            // When the registration form closes, show the MainDashboard again
            registrationForm.FormClosed += (s, args) => this.Show();

            registrationForm.Show();
        }

        private void btnNavStudentRecords_Click(object sender, EventArgs e)
        {
            StudentRecordScreen studentRecordScreen = new StudentRecordScreen();
            studentRecordScreen.Show();
        }

        private void btnLogout_Click_1(object sender, EventArgs e)
        {

        }

        private void pictureBoxLogo_Click(object sender, EventArgs e)
        {

        }

        private void lblScannerStatus_Click(object sender, EventArgs e)
        {

        }

        private void lblTotalStudentsTitle_Click(object sender, EventArgs e)
        {

        }

        private void lblRecentScansTitle_Click(object sender, EventArgs e)
        {

        }
    }
}
