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
using ITP104_FINAL_PROJECT.Data;
using ITP104_FINAL_PROJECT.Models;
using System.Windows.Forms.DataVisualization.Charting;

namespace ITP104_FINAL_PROJECT
{
    public partial class MainDashboard : Form
    {
        private string currentUser = "Admin";
        private Timer statusUpdateTimer;
        private Timer dashboardRefreshTimer;
        private Random random = new Random();
        private readonly StudentRepository studentRepository;
        private readonly ScanHistoryRepository scanHistoryRepository;

        // Panel references for navigation
        private Panel pnlDashboardContent;
        private Panel pnlRegisterStudentContent;
        private Panel pnlScanContent;
        private Panel pnlStudentRecordsContent;
        private DataGridView dgvStudentsGrid; // Store reference to the student grid for auto-refresh
        private Button btnStudentRecordsRefresh; // Store reference to the refresh button
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
            studentRepository = new StudentRepository();
            scanHistoryRepository = new ScanHistoryRepository();
            InitializeDashboard();
        }

        private async void InitializeDashboard()
        {
            // Set user information
            lblUserName.Text = $"User: {currentUser}";

            // Initialize panels for navigation
            InitializePanels();

            // Initialize status indicators
            UpdateSystemStatus();

            // Load dashboard statistics asynchronously
            await LoadDashboardStatsAsync();

            // Load recent scans asynchronously
            await LoadRecentScansAsync();

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
                // Open the QR scanner form with centered scan box
                QRScannerForm scannerForm = new QRScannerForm();
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
                Padding = new Padding(20)
            };

            // Header Panel
            Panel headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.White,
                Padding = new Padding(20, 15, 20, 15)
            };

            Label lblTitle = new Label
            {
                Text = "👥 Student Records",
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64),
                Location = new Point(20, 10),
                AutoSize = true
            };

            Label lblDescription = new Label
            {
                Text = "Browse and manage registered students",
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.FromArgb(120, 120, 120),
                Location = new Point(20, 42),
                AutoSize = true
            };

            headerPanel.Controls.AddRange(new Control[] { lblTitle, lblDescription });

            // Search Panel
            Panel searchPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = Color.Transparent,
                Padding = new Padding(0, 10, 0, 10)
            };

            TextBox txtSearch = new TextBox
            {
                Location = new Point(0, 15),
                Width = 300,
                Height = 35,
                Font = new Font("Segoe UI", 11F),
                Text = "Search by name or student number..."
            };

            // Add placeholder text behavior
            txtSearch.ForeColor = Color.Gray;
            txtSearch.GotFocus += (s, e) =>
            {
                if (txtSearch.Text == "Search by name or student number...")
                {
                    txtSearch.Text = "";
                    txtSearch.ForeColor = Color.Black;
                }
            };
            txtSearch.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    txtSearch.Text = "Search by name or student number...";
                    txtSearch.ForeColor = Color.Gray;
                }
            };

            Button btnSearch = new Button
            {
                Text = "🔍 Search",
                Location = new Point(310, 15),
                Width = 100,
                Height = 35,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnSearch.FlatAppearance.BorderSize = 0;

            btnStudentRecordsRefresh = new Button
            {
                Text = "🔄 Refresh",
                Location = new Point(420, 15),
                Width = 100,
                Height = 35,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnStudentRecordsRefresh.FlatAppearance.BorderSize = 0;

            searchPanel.Controls.AddRange(new Control[] { txtSearch, btnSearch, btnStudentRecordsRefresh });

            // DataGridView for students
            dgvStudentsGrid = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                RowHeadersVisible = false,
                EnableHeadersVisualStyles = false
            };

            // Column styling
            dgvStudentsGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
            dgvStudentsGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvStudentsGrid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgvStudentsGrid.ColumnHeadersDefaultCellStyle.Padding = new Padding(10, 5, 10, 5);
            dgvStudentsGrid.ColumnHeadersHeight = 45;

            dgvStudentsGrid.DefaultCellStyle.BackColor = Color.White;
            dgvStudentsGrid.DefaultCellStyle.ForeColor = Color.FromArgb(64, 64, 64);
            dgvStudentsGrid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(189, 195, 199);
            dgvStudentsGrid.DefaultCellStyle.SelectionForeColor = Color.FromArgb(44, 62, 80);
            dgvStudentsGrid.DefaultCellStyle.Font = new Font("Segoe UI", 9.5F);
            dgvStudentsGrid.DefaultCellStyle.Padding = new Padding(10, 5, 10, 5);
            dgvStudentsGrid.RowTemplate.Height = 45;
            dgvStudentsGrid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 250, 250);

            // Define columns
            dgvStudentsGrid.Columns.AddRange(new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn { Name = "StudentId", HeaderText = "ID", Width = 60, DataPropertyName = "StudentId", Visible = false },
                new DataGridViewTextBoxColumn { Name = "StudentNumber", HeaderText = "Student Number", Width = 130, DataPropertyName = "StudentNumber" },
                new DataGridViewTextBoxColumn { Name = "FullName", HeaderText = "Full Name", Width = 200, DataPropertyName = "FullName" },
                new DataGridViewTextBoxColumn { Name = "Email", HeaderText = "Email", Width = 200, DataPropertyName = "Email" },
                new DataGridViewTextBoxColumn { Name = "Program", HeaderText = "Program", Width = 160, DataPropertyName = "Program" },
                new DataGridViewTextBoxColumn { Name = "YearLevel", HeaderText = "Year", Width = 60, DataPropertyName = "YearLevel" },
                new DataGridViewTextBoxColumn { Name = "Section", HeaderText = "Section", Width = 80, DataPropertyName = "Section" },
                new DataGridViewTextBoxColumn { Name = "Status", HeaderText = "Status", Width = 80, DataPropertyName = "Status" }
            });

            // Add View Details button column
            DataGridViewButtonColumn btnViewDetails = new DataGridViewButtonColumn
            {
                Name = "btnViewDetails",
                HeaderText = "Action",
                Text = "View Details",
                UseColumnTextForButtonValue = true,
                Width = 100,
                FlatStyle = FlatStyle.Flat
            };
            dgvStudentsGrid.Columns.Add(btnViewDetails);

            // Container for DataGridView
            Panel dgvContainer = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(0, 10, 0, 0)
            };
            dgvContainer.Controls.Add(dgvStudentsGrid);

            // Add all controls to main panel
            panel.Controls.Add(dgvContainer);
            panel.Controls.Add(searchPanel);
            panel.Controls.Add(headerPanel);

            // Load students asynchronously
            Task.Run(async () =>
            {
                try
                {
                    var students = await studentRepository.GetAllAsync(activeOnly: false);

                    // Create display list with formatted data
                    var displayList = students.Select(s => new
                    {
                        s.StudentId,
                        s.StudentNumber,
                        FullName = $"{s.FirstName} {(string.IsNullOrEmpty(s.MiddleName) ? "" : s.MiddleName + " ")}{s.LastName}",
                        s.Email,
                        s.Program,
                        YearLevel = s.YearLevel + (s.YearLevel == "1" ? "st" : s.YearLevel == "2" ? "nd" : s.YearLevel == "3" ? "rd" : "th"),
                        s.Section,
                        s.Status
                    }).ToList();

                    // Update UI on main thread
                    if (dgvStudentsGrid.InvokeRequired)
                    {
                        dgvStudentsGrid.Invoke(new Action(() =>
                        {
                            dgvStudentsGrid.DataSource = displayList;
                        }));
                    }
                    else
                    {
                        dgvStudentsGrid.DataSource = displayList;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading students: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            });

            // Search button click event
            btnSearch.Click += async (s, e) =>
            {
                string searchTerm = txtSearch.Text.Trim();
                if (string.IsNullOrEmpty(searchTerm) || searchTerm == "Search by name or student number...")
                {
                    btnStudentRecordsRefresh.PerformClick();
                    return;
                }

                try
                {
                    var students = await studentRepository.SearchAsync(searchTerm);
                    var displayList = students.Select(st => new
                    {
                        st.StudentId,
                        st.StudentNumber,
                        FullName = $"{st.FirstName} {(string.IsNullOrEmpty(st.MiddleName) ? "" : st.MiddleName + " ")}{st.LastName}",
                        st.Email,
                        st.Program,
                        YearLevel = st.YearLevel + (st.YearLevel == "1" ? "st" : st.YearLevel == "2" ? "nd" : st.YearLevel == "3" ? "rd" : "th"),
                        st.Section,
                        st.Status
                    }).ToList();

                    dgvStudentsGrid.DataSource = displayList;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error searching students: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            // Refresh button click event
            btnStudentRecordsRefresh.Click += async (s, e) =>
            {
                try
                {
                    txtSearch.Text = "Search by name or student number...";
                    txtSearch.ForeColor = Color.Gray;

                    var students = await studentRepository.GetAllAsync(activeOnly: false);
                    var displayList = students.Select(st => new
                    {
                        st.StudentId,
                        st.StudentNumber,
                        FullName = $"{st.FirstName} {(string.IsNullOrEmpty(st.MiddleName) ? "" : st.MiddleName + " ")}{st.LastName}",
                        st.Email,
                        st.Program,
                        YearLevel = st.YearLevel + (st.YearLevel == "1" ? "st" : st.YearLevel == "2" ? "nd" : st.YearLevel == "3" ? "rd" : "th"),
                        st.Section,
                        st.Status
                    }).ToList();

                    dgvStudentsGrid.DataSource = displayList;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error refreshing students: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            // Handle View Details button click
            dgvStudentsGrid.CellContentClick += (s, e) =>
            {
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                {
                    if (dgvStudentsGrid.Columns[e.ColumnIndex].Name == "btnViewDetails")
                    {
                        var studentId = dgvStudentsGrid.Rows[e.RowIndex].Cells["StudentId"].Value?.ToString();
                        if (!string.IsNullOrEmpty(studentId))
                        {
                            StudentRecordScreen recordScreen = new StudentRecordScreen(studentId);
                            recordScreen.ShowDialog();
                        }
                    }
                }
            };

            // Double-click also opens student details for convenience
            dgvStudentsGrid.CellDoubleClick += (s, e) =>
            {
                if (e.RowIndex >= 0)
                {
                    var studentId = dgvStudentsGrid.Rows[e.RowIndex].Cells["StudentId"].Value?.ToString();
                    if (!string.IsNullOrEmpty(studentId))
                    {
                        StudentRecordScreen recordScreen = new StudentRecordScreen(studentId);
                        recordScreen.ShowDialog();
                    }
                }
            };

            // Enter key search
            txtSearch.KeyPress += (s, e) =>
            {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    btnSearch.PerformClick();
                    e.Handled = true;
                }
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

            panel.Controls.Add(lblTitle);
            panel.Controls.Add(lblDescription);

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

            // Button to open full settings screen
            Guna.UI2.WinForms.Guna2Button btnOpenSettings = new Guna.UI2.WinForms.Guna2Button
            {
                Text = "⚙️ Open Settings",
                Location = new Point(30, 110),
                Size = new Size(250, 60),
                BorderRadius = 10,
                FillColor = Color.FromArgb(52, 73, 94),
                Font = new Font("Segoe UI Semibold", 12F),
                ForeColor = Color.White
            };
            btnOpenSettings.Click += (s, e) =>
            {
                // Open the settings screen
                SettingsScreen settingsForm = new SettingsScreen();
                settingsForm.ShowDialog();
            };

            panel.Controls.Add(lblTitle);
            panel.Controls.Add(lblDescription);
            panel.Controls.Add(btnOpenSettings);

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
            // Navigation to Student Records page - button does not open StudentRecordScreen directly
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

                // show the panel
                if (targetPanel != null)
                {
                    ShowPanel(targetPanel);
                    UpdateNavIndicator(pageIndex);
                }

                // update breadcrumb information
                UpdateBreadcrumb(pageIndex);

                // navigation completed successfully to specified page
            }
            catch (Exception ex)
            {
                // navigation attempt failed, target panel may be null or invalid
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

                // breadcrumb information retrieved and ready for display
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
            // Status update timer
            statusUpdateTimer = new Timer();
            statusUpdateTimer.Interval = 5000; // Update every 5 seconds
            statusUpdateTimer.Tick += StatusUpdateTimer_Tick;
            statusUpdateTimer.Start();

            // Dashboard refresh timer for real-time stats
            dashboardRefreshTimer = new Timer();
            dashboardRefreshTimer.Interval = 5000; // Refresh every 5 seconds
            dashboardRefreshTimer.Tick += DashboardRefreshTimer_Tick;
            dashboardRefreshTimer.Start();
        }

        private void StatusUpdateTimer_Tick(object sender, EventArgs e)
        {
            // Simulate real-time updates
            UpdateSystemStatus();
        }

        private async void DashboardRefreshTimer_Tick(object sender, EventArgs e)
        {
            // Auto-refresh dashboard stats every 5 seconds
            await LoadDashboardStatsAsync();
            await LoadRecentScansAsync();
        }

        private void UpdateSystemStatus()
        {
            try
            {
                // Scanner status - Check actual device state from QRScannerForm
                bool scannerActive = QRScannerForm.IsScannerRunning;
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

        private async Task LoadDashboardStatsAsync()
        {
            try
            {
                // Get total students count from database
                var students = await studentRepository.GetAllAsync(activeOnly: true);
                int totalStudents = students.Count;

                // Get daily summary from stored procedure
                var summaryTable = await scanHistoryRepository.GetDailySummaryAsync(DateTime.Today);

                int scansToday = 0;
                int qrScans = 0;
                int manualScans = 0;

                if (summaryTable != null && summaryTable.Rows.Count > 0)
                {
                    var row = summaryTable.Rows[0];
                    scansToday = row["total_scans"] != DBNull.Value ? Convert.ToInt32(row["total_scans"]) : 0;
                    qrScans = row["qr_scans"] != DBNull.Value ? Convert.ToInt32(row["qr_scans"]) : 0;
                }

                // Calculate most used scan type from actual data
                manualScans = scansToday - qrScans;
                string mostUsedScanType = "N/A";

                if (scansToday > 0)
                {
                    if (qrScans >= manualScans)
                    {
                        mostUsedScanType = $"QR Code ({qrScans})";
                    }
                    else
                    {
                        mostUsedScanType = $"Manual ({manualScans})";
                    }
                }
                else
                {
                    mostUsedScanType = "No scans today";
                }

                // Update UI on main thread
                if (lblTotalStudentsValue.InvokeRequired)
                {
                    lblTotalStudentsValue.Invoke(new Action(() =>
                    {
                        lblTotalStudentsValue.Text = totalStudents.ToString("N0");
                        lblScansTodayValue.Text = scansToday.ToString("N0");
                        lblMostUsedScanValue.Text = mostUsedScanType;
                    }));
                }
                else
                {
                    lblTotalStudentsValue.Text = totalStudents.ToString("N0");
                    lblScansTodayValue.Text = scansToday.ToString("N0");
                    lblMostUsedScanValue.Text = mostUsedScanType;
                }
            }
            catch (Exception ex)
            {
                // Log detailed error information
                string errorDetails = $"Error loading dashboard stats:\n" +
                                    $"Message: {ex.Message}\n" +
                                    $"Type: {ex.GetType().Name}\n" +
                                    $"Stack: {ex.StackTrace}";

                if (ex.InnerException != null)
                {
                    errorDetails += $"\nInner Exception: {ex.InnerException.Message}";
                }

                System.Diagnostics.Debug.WriteLine(errorDetails);

                // show error to user only once, not on every refresh cycle
                if (lblTotalStudentsValue.Text != "Error")
                {
                    MessageBox.Show(
                        $"Failed to load dashboard statistics.\n\n" +
                        $"Error: {ex.Message}\n\n" +
                        $"Please check:\n" +
                        $"1. Database connection is active\n" +
                        $"2. Stored procedure 'sp_get_daily_summary' exists\n" +
                        $"3. Database tables have data",
                        "Dashboard Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }

                // Update UI with error state on main thread
                if (lblTotalStudentsValue.InvokeRequired)
                {
                    lblTotalStudentsValue.Invoke(new Action(() =>
                    {
                        lblTotalStudentsValue.Text = "Error";
                        lblScansTodayValue.Text = "Error";
                        lblMostUsedScanValue.Text = "Error";
                    }));
                }
                else
                {
                    lblTotalStudentsValue.Text = "Error";
                    lblScansTodayValue.Text = "Error";
                    lblMostUsedScanValue.Text = "Error";
                }
            }
        }

        private async Task LoadRecentScansAsync()
        {
            try
            {
                // fetching recent scans from database view

                // get recent scans from view
                var recentScans = await scanHistoryRepository.GetRecentScansAsync(limit: 10);

                // scans successfully retrieved from database query

                // Update UI on main thread
                if (dgvRecentScans.InvokeRequired)
                {
                    dgvRecentScans.Invoke(new Action(() => UpdateRecentScansUI(recentScans)));
                }
                else
                {
                    UpdateRecentScansUI(recentScans);
                }
            }
            catch (Exception ex)
            {
                // Log detailed error information
                string errorDetails = $"Error loading recent scans:\n" +
                                    $"Message: {ex.Message}\n" +
                                    $"Type: {ex.GetType().Name}\n" +
                                    $"StackTrace: {ex.StackTrace}";

                if (ex.InnerException != null)
                {
                    errorDetails += $"\nInner Exception: {ex.InnerException.Message}";
                }

                System.Diagnostics.Debug.WriteLine(errorDetails);

                // show error message to user for debugging
                MessageBox.Show(
                    $"Failed to load recent scans.\n\n" +
                    $"Error: {ex.Message}\n\n" +
                    $"Please check:\n" +
                    $"1. Database connection is active\n" +
                    $"2. View 'vw_recent_scans' exists in database\n" +
                    $"3. Database has scan history data",
                    "Recent Scans Load Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
        }

        private void UpdateRecentScansUI(List<ScanHistory> recentScans)
        {
            // Initialize columns if not already done
            if (dgvRecentScans.Columns.Count == 0)
            {
                dgvRecentScans.Columns.Add("StudentNumber", "Student ID");
                dgvRecentScans.Columns.Add("StudentName", "Name");
                dgvRecentScans.Columns.Add("Program", "Course");
                dgvRecentScans.Columns.Add("ScanType", "Scan Type");
                dgvRecentScans.Columns.Add("ScanDateTime", "Time");

                // Set column widths
                dgvRecentScans.Columns["StudentNumber"].Width = 150;
                dgvRecentScans.Columns["StudentName"].Width = 200;
                dgvRecentScans.Columns["Program"].Width = 180;
                dgvRecentScans.Columns["ScanType"].Width = 100;
                dgvRecentScans.Columns["ScanDateTime"].Width = 180;
            }

            // Clear existing rows
            dgvRecentScans.Rows.Clear();

            // Add recent scans
            if (recentScans != null && recentScans.Count > 0)
            {
                foreach (var scan in recentScans)
                {
                    dgvRecentScans.Rows.Add(
                        scan.StudentNumber ?? "N/A",
                        scan.StudentName ?? "Unknown",
                        scan.Program ?? "N/A",
                        scan.ScanType ?? "QR",
                        scan.ScanDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")
                    );
                }
                // recent scans successfully added to grid display
            }
            else
            {
                // no scan records currently exist in database
            }
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
            // Open ScanHistoryScreen
            ScanHistoryScreen historyScreen = new ScanHistoryScreen();
            historyScreen.ShowDialog();
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

            // Cleanup timers
            if (statusUpdateTimer != null)
            {
                statusUpdateTimer.Stop();
                statusUpdateTimer.Dispose();
            }

            if (dashboardRefreshTimer != null)
            {
                dashboardRefreshTimer.Stop();
                dashboardRefreshTimer.Dispose();
            }
        }

        // Public method to set user
        public void SetUser(string username)
        {
            currentUser = username;
            lblUserName.Text = $"User: {currentUser}";
        }

        // Method to refresh dashboard data
        public async void RefreshDashboard()
        {
            await LoadDashboardStatsAsync();
            await LoadRecentScansAsync();
            UpdateSystemStatus();
        }

        // Method to add new scan to recent scans
        // Method to add new scan to recent scans
        public void AddScanToHistory(string studentId, string name, string program, string scanType)
        {
            try
            {
                if (dgvRecentScans.Rows.Count >= 10)
                {
                    dgvRecentScans.Rows.RemoveAt(dgvRecentScans.Rows.Count - 1);
                }

                // Columns: StudentNumber, StudentName, Program, ScanType, ScanDateTime
                dgvRecentScans.Rows.Insert(0, studentId, name, program, scanType, DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt"));

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
            registrationForm.FormClosed += async (s, args) =>
            {
                this.Show();
                // Auto-refresh student records when registration form closes
                await RefreshStudentRecordsAsync();
                // Show the Student Records panel so user can see the newly registered student
                ShowPanel(pnlStudentRecordsContent);
                UpdateNavIndicator(3); // Index 3 is Student Records
            };

            registrationForm.Show();
        }

        /// <summary>
        /// Refresh the Student Records panel data asynchronously
        /// </summary>
        private async Task RefreshStudentRecordsAsync()
        {
            try
            {
                if (dgvStudentsGrid == null)
                {
                    return; // Grid not initialized yet
                }

                // Load students from database
                var students = await studentRepository.GetAllAsync(activeOnly: false);

                // Create display list with formatted data
                var displayList = students.Select(st => new
                {
                    st.StudentId,
                    st.StudentNumber,
                    FullName = $"{st.FirstName} {(string.IsNullOrEmpty(st.MiddleName) ? "" : st.MiddleName + " ")}{st.LastName}",
                    st.Email,
                    st.Program,
                    YearLevel = st.YearLevel + (st.YearLevel == "1" ? "st" : st.YearLevel == "2" ? "nd" : st.YearLevel == "3" ? "rd" : "th"),
                    st.Section,
                    st.Status
                }).ToList();

                // Update UI on main thread
                if (dgvStudentsGrid.InvokeRequired)
                {
                    dgvStudentsGrid.Invoke(new Action(() =>
                    {
                        dgvStudentsGrid.DataSource = displayList;
                    }));
                }
                else
                {
                    dgvStudentsGrid.DataSource = displayList;
                }
            }
            catch (Exception ex)
            {
                // student records refresh encountered an error, grid may be out of sync
            }
        }

        private void btnNavStudentRecords_Click(object sender, EventArgs e)
        {
            // Navigation behavior removed - StudentRecordScreen now opened via View Details button in grid
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

        private void btnReports_Click_1(object sender, EventArgs e)

        {
            ScanHistory scan = new ScanHistory();
        }
    }
}
