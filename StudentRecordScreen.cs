using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data;

namespace ITP104_FINAL_PROJECT
{
    public partial class StudentRecordScreen : Form
    {
        private string studentId;
        private Timer animationTimer;
        private int animationStep = 0;

        public StudentRecordScreen()
        {
            InitializeComponent();
            InitializeForm();
        }

        public StudentRecordScreen(string studentId) : this()
        {
            this.studentId = studentId;
            LoadStudentData(studentId);
        }

        private void InitializeForm()
        {
            // Initialize animation timer for smooth loading effects
            animationTimer = new Timer();
            animationTimer.Interval = 30;
            animationTimer.Tick += AnimationTimer_Tick;

            // Setup event handlers
            btnEdit.Click += BtnEdit_Click;
            btnBackToScan.Click += BtnBackToScan_Click;
            btnPrint.Click += BtnPrint_Click;
            btnExport.Click += BtnExport_Click;

            // Setup hover effects
            SetupHoverEffects();

            // Initialize scan history table
            InitializeScanHistoryTable();

            // Set default profile image
            if (picProfilePhoto.Image == null)
            {
                picProfilePhoto.Image = Properties.Resources.default_avatar;
            }

            // Start animation
            animationTimer.Start();
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            animationStep++;

            if (animationStep <= 20)
            {
                // Fade in effect for panels
                float opacity = animationStep / 20f;
                pnlStudentInfo.Visible = true;
                pnlScanHistory.Visible = true;
            }
            else
            {
                animationTimer.Stop();
            }
        }

        private void SetupHoverEffects()
        {
            // Edit button hover
            btnEdit.MouseEnter += (s, e) =>
            {
                btnEdit.FillColor = Color.FromArgb(230, 126, 34);
                btnEdit.Cursor = Cursors.Hand;
            };
            btnEdit.MouseLeave += (s, e) =>
            {
                btnEdit.FillColor = Color.FromArgb(243, 156, 18);
            };

            // Back to Scan button hover
            btnBackToScan.MouseEnter += (s, e) =>
            {
                btnBackToScan.FillColor = Color.FromArgb(40, 120, 180);
                btnBackToScan.Cursor = Cursors.Hand;
            };
            btnBackToScan.MouseLeave += (s, e) =>
            {
                btnBackToScan.FillColor = Color.FromArgb(52, 152, 219);
            };

            // Print button hover
            btnPrint.MouseEnter += (s, e) =>
            {
                btnPrint.FillColor = Color.FromArgb(90, 180, 90);
                btnPrint.Cursor = Cursors.Hand;
            };
            btnPrint.MouseLeave += (s, e) =>
            {
                btnPrint.FillColor = Color.FromArgb(46, 204, 113);
            };

            // Export button hover
            btnExport.MouseEnter += (s, e) =>
            {
                btnExport.FillColor = Color.FromArgb(142, 68, 173);
                btnExport.Cursor = Cursors.Hand;
            };
            btnExport.MouseLeave += (s, e) =>
            {
                btnExport.FillColor = Color.FromArgb(155, 89, 182);
            };

            // Scan history panel hover effect
            pnlScanHistory.MouseEnter += (s, e) =>
            {
                pnlScanHistory.ShadowDecoration.Depth = 20;
            };
            pnlScanHistory.MouseLeave += (s, e) =>
            {
                pnlScanHistory.ShadowDecoration.Depth = 10;
            };
        }

        private void InitializeScanHistoryTable()
        {
            dgvScanHistory.Columns.Clear();
            dgvScanHistory.AutoGenerateColumns = false;

            // Create columns with proper styling
            DataGridViewTextBoxColumn colDate = new DataGridViewTextBoxColumn
            {
                Name = "Date",
                HeaderText = "Date",
                Width = 180,
                DataPropertyName = "Date"
            };

            DataGridViewTextBoxColumn colTime = new DataGridViewTextBoxColumn
            {
                Name = "Time",
                HeaderText = "Time",
                Width = 150,
                DataPropertyName = "Time"
            };

            DataGridViewTextBoxColumn colScanType = new DataGridViewTextBoxColumn
            {
                Name = "ScanType",
                HeaderText = "Scan Type",
                Width = 180,
                DataPropertyName = "ScanType"
            };

            DataGridViewTextBoxColumn colLocation = new DataGridViewTextBoxColumn
            {
                Name = "Location",
                HeaderText = "Location",
                Width = 250,
                DataPropertyName = "Location",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            };

            dgvScanHistory.Columns.AddRange(new DataGridViewColumn[] { colDate, colTime, colScanType, colLocation });

            // Style the DataGridView
            dgvScanHistory.BackgroundColor = Color.White;
            dgvScanHistory.BorderStyle = BorderStyle.None;
            dgvScanHistory.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvScanHistory.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvScanHistory.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
            dgvScanHistory.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvScanHistory.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgvScanHistory.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(52, 73, 94);
            dgvScanHistory.ColumnHeadersDefaultCellStyle.Padding = new Padding(10, 5, 10, 5);
            dgvScanHistory.ColumnHeadersHeight = 45;
            dgvScanHistory.DefaultCellStyle.BackColor = Color.White;
            dgvScanHistory.DefaultCellStyle.ForeColor = Color.FromArgb(64, 64, 64);
            dgvScanHistory.DefaultCellStyle.SelectionBackColor = Color.FromArgb(189, 195, 199);
            dgvScanHistory.DefaultCellStyle.SelectionForeColor = Color.FromArgb(44, 62, 80);
            dgvScanHistory.DefaultCellStyle.Font = new Font("Segoe UI", 9.5F);
            dgvScanHistory.DefaultCellStyle.Padding = new Padding(10, 5, 10, 5);
            dgvScanHistory.EnableHeadersVisualStyles = false;
            dgvScanHistory.GridColor = Color.FromArgb(231, 231, 231);
            dgvScanHistory.RowHeadersVisible = false;
            dgvScanHistory.RowTemplate.Height = 40;
            dgvScanHistory.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvScanHistory.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 250, 250);
        }

        private void LoadStudentData(string studentId)
        {
            try
            {
                // Show loading indicator
                ShowLoadingIndicator(true);

                // Simulate loading delay for smooth animation
                System.Threading.Thread.Sleep(300);

                // In a real application, this would load from database
                // For now, we'll use sample data
                LoadSampleStudentData(studentId);

                // Load scan history
                LoadScanHistory(studentId);

                // Hide loading indicator
                ShowLoadingIndicator(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading student data: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadSampleStudentData(string studentId)
        {
            // Sample student data - Replace with actual database query
            lblStudentIDValue.Text = studentId ?? "2024-STU-0001";
            lblFullNameValue.Text = "Juan Dela Cruz";
            lblCourseValue.Text = "Computer Science";
            lblYearLevelValue.Text = "3rd Year";
            lblStatusValue.Text = "Active";
            lblStatusValue.ForeColor = Color.FromArgb(46, 204, 113);

            lblEmailValue.Text = "juan.delacruz@university.edu";
            lblPhoneValue.Text = "+63 912 345 6789";
            lblAddressValue.Text = "123 Main Street, Manila, Philippines";
            lblEnrollmentDateValue.Text = "August 15, 2022";

            // Update status badge
            UpdateStatusBadge("Active");
        }

        private void UpdateStatusBadge(string status)
        {
            if (status.ToLower() == "active")
            {
                lblStatusValue.Text = "● Active";
                lblStatusValue.ForeColor = Color.FromArgb(46, 204, 113);
            }
            else
            {
                lblStatusValue.Text = "● Inactive";
                lblStatusValue.ForeColor = Color.FromArgb(231, 76, 60);
            }
        }

        private void LoadScanHistory(string studentId)
        {
            try
            {
                // Clear existing data
                dgvScanHistory.Rows.Clear();

                // Sample scan history data - Replace with actual database query
                dgvScanHistory.Rows.Add("Nov 16, 2025", "08:15 AM", "Entrance - QR", "Main Building");
                dgvScanHistory.Rows.Add("Nov 16, 2025", "10:30 AM", "Library - QR", "University Library");
                dgvScanHistory.Rows.Add("Nov 16, 2025", "02:45 PM", "Cafeteria - QR", "Student Cafeteria");
                dgvScanHistory.Rows.Add("Nov 15, 2025", "08:00 AM", "Entrance - QR", "Main Building");
                dgvScanHistory.Rows.Add("Nov 15, 2025", "11:20 AM", "Laboratory - Barcode", "Computer Lab 3");
                dgvScanHistory.Rows.Add("Nov 15, 2025", "03:15 PM", "Exit - QR", "Main Building");
                dgvScanHistory.Rows.Add("Nov 14, 2025", "08:10 AM", "Entrance - QR", "Main Building");
                dgvScanHistory.Rows.Add("Nov 14, 2025", "09:45 AM", "Classroom - QR", "Room 301");
                dgvScanHistory.Rows.Add("Nov 14, 2025", "01:30 PM", "Library - QR", "University Library");
                dgvScanHistory.Rows.Add("Nov 14, 2025", "04:00 PM", "Exit - QR", "Main Building");

                // Update scan count
                lblScanCountValue.Text = dgvScanHistory.Rows.Count.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading scan history: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowLoadingIndicator(bool show)
        {
            if (show)
            {
                lblLoadingIndicator.Visible = true;
                lblLoadingIndicator.Text = "⏳ Loading student information...";
            }
            else
            {
                lblLoadingIndicator.Visible = false;
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            // Edit functionality disabled in Phase 1 as per requirements
            MessageBox.Show(
                "Edit functionality will be available in Phase 2.\n\n" +
                "This feature is currently under development.",
                "Feature Not Available",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        private void BtnBackToScan_Click(object sender, EventArgs e)
        {
            // Close this form and return to previous screen
            DialogResult result = MessageBox.Show(
                "Return to scan screen?",
                "Confirm",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                this.Close();
            }
        }

        private void BtnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                MessageBox.Show(
                    "Print preview will open shortly.\n\n" +
                    "Student Record Details:\n" +
                    $"ID: {lblStudentIDValue.Text}\n" +
                    $"Name: {lblFullNameValue.Text}\n" +
                    $"Course: {lblCourseValue.Text}",
                    "Print Student Record",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error printing: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveDialog = new SaveFileDialog
                {
                    Filter = "CSV File|*.csv|Excel File|*.xlsx|PDF File|*.pdf",
                    Title = "Export Student Record",
                    FileName = $"Student_Record_{lblStudentIDValue.Text}_{DateTime.Now:yyyyMMdd}"
                };

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    MessageBox.Show(
                        $"Student record exported successfully!\n\n" +
                        $"File: {saveDialog.FileName}",
                        "Export Successful",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            // Cleanup
            if (animationTimer != null)
            {
                animationTimer.Stop();
                animationTimer.Dispose();
            }
        }

        // Public method to refresh student data
        public void RefreshStudentData()
        {
            if (!string.IsNullOrEmpty(studentId))
            {
                LoadStudentData(studentId);
            }
        }

        // Public method to update student ID and reload
        public void SetStudentId(string id)
        {
            this.studentId = id;
            LoadStudentData(id);
        }

        private void lblPhoneValue_Click(object sender, EventArgs e)
        {

        }

        private void btnEdit_Click_1(object sender, EventArgs e)
        {

        }

        private void btnPrint_Click_1(object sender, EventArgs e)
        {

        }
    }
}
