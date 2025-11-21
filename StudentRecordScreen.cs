using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using System.Threading.Tasks;
using ITP104_FINAL_PROJECT.Data;
using ITP104_FINAL_PROJECT.Models;

namespace ITP104_FINAL_PROJECT
{
    public partial class StudentRecordScreen : Form
    {
        private string studentId;
        private Timer animationTimer;
        private int animationStep = 0;
        private readonly StudentRepository studentRepository;
        private readonly ScanHistoryRepository scanHistoryRepository;

        public StudentRecordScreen()
        {
            InitializeComponent();
            studentRepository = new StudentRepository();
            scanHistoryRepository = new ScanHistoryRepository();
            InitializeForm();
        }

        public StudentRecordScreen(string studentId) : this()
        {
            this.studentId = studentId;
            LoadStudentDataAsync(studentId);
        }

        private void InitializeForm()
        {
            // Initialize animation timer for smooth loading effects
            animationTimer = new Timer();
            animationTimer.Interval = 30;
            animationTimer.Tick += AnimationTimer_Tick;

            // Setup event handlers
            btnEdit.Click += btnEdit_Click_1;
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
                Width = 150,
                DataPropertyName = "Date"
            };

            DataGridViewTextBoxColumn colTimeIn = new DataGridViewTextBoxColumn
            {
                Name = "TimeIn",
                HeaderText = "Time In",
                Width = 120,
                DataPropertyName = "TimeIn"
            };

            DataGridViewTextBoxColumn colTimeOut = new DataGridViewTextBoxColumn
            {
                Name = "TimeOut",
                HeaderText = "Time Out",
                Width = 120,
                DataPropertyName = "TimeOut"
            };

            DataGridViewTextBoxColumn colScanType = new DataGridViewTextBoxColumn
            {
                Name = "ScanType",
                HeaderText = "Scan Type",
                Width = 150,
                DataPropertyName = "ScanType"
            };

            DataGridViewTextBoxColumn colLocation = new DataGridViewTextBoxColumn
            {
                Name = "Location",
                HeaderText = "Location",
                Width = 200,
                DataPropertyName = "Location",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            };

            dgvScanHistory.Columns.AddRange(new DataGridViewColumn[] { colDate, colTimeIn, colTimeOut, colScanType, colLocation });

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

        private async Task LoadStudentDataAsync(string studentId)
        {
            try
            {
                // Show loading indicator
                ShowLoadingIndicator(true);

                // Load student data from database
                int studentIdInt = int.Parse(studentId);
                Student student = await studentRepository.GetByIdAsync(studentIdInt);

                if (student != null)
                {
                    // Populate student information
                    lblStudentIDValue.Text = student.StudentNumber;
                    
                    // Full name with proper spacing
                    string fullName = student.FirstName;
                    if (!string.IsNullOrWhiteSpace(student.MiddleName))
                    {
                        fullName += " " + student.MiddleName;
                    }
                    fullName += " " + student.LastName;
                    lblFullNameValue.Text = fullName.Trim();
                    
                    // Course/Program
                    lblCourseValue.Text = student.Program;

                    // Format year level with proper suffix
                    string yearLevel = student.YearLevel;
                    string suffix = yearLevel == "1" ? "st" : 
                                   yearLevel == "2" ? "nd" : 
                                   yearLevel == "3" ? "rd" : "th";
                    lblYearLevelValue.Text = $"{yearLevel}{suffix} Year";

                    // Contact information
                    lblEmailValue.Text = student.Email ?? "N/A";
                    lblPhoneValue.Text = student.Phone ?? "N/A";
                    
                    // Address (not in schema, always N/A)
                    lblAddressValue.Text = "N/A";
                    
                    // Enrollment date
                    lblEnrollmentDateValue.Text = student.EnrollmentDate.ToString("MMMM dd, yyyy");

                    // Update status badge
                    UpdateStatusBadge(student.Status);
                    
                    // Update Sex/Gender label dynamically
                    if (label2 != null)
                    {
                        label2.Text = student.Sex ?? "Not Specified";
                    }
                    
                    // Force all labels to update immediately
                    lblStudentIDValue.Refresh();
                    lblFullNameValue.Refresh();
                    lblCourseValue.Refresh();
                    lblYearLevelValue.Refresh();
                    lblEmailValue.Refresh();
                    lblPhoneValue.Refresh();

                    // Load scan history for this student
                    await LoadScanHistoryAsync(studentIdInt);
                }
                else
                {
                    MessageBox.Show("Student not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                }

                // Hide loading indicator
                ShowLoadingIndicator(false);
            }
            catch (Exception ex)
            {
                ShowLoadingIndicator(false);
                MessageBox.Show($"Error loading student data: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadStudentData(string studentId)
        {
            // Redirect to async method - fire and forget pattern
            _ = LoadStudentDataAsync(studentId);
        }

        private void UpdateStatusBadge(string status)
        {
            if (lblStatusValue != null)
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
        }

        private async Task LoadScanHistoryAsync(int studentId)
        {
            try
            {
                // Get scan history from database
                var scanHistory = await scanHistoryRepository.GetStudentScansAsync(studentId);

                // Clear existing rows
                dgvScanHistory.Rows.Clear();

                if (scanHistory != null && scanHistory.Count > 0)
                {
                    foreach (var scan in scanHistory)
                    {
                        string date = scan.ScanDateTime.ToString("MM/dd/yyyy");
                        string timeIn = scan.ScanDateTime.ToString("hh:mm tt");
                        string timeOut = scan.TimeOut.HasValue ? scan.TimeOut.Value.ToString("hh:mm tt") : "-";
                        string scanType = scan.ScanType;
                        string location = scan.Location ?? "Main Building";

                        dgvScanHistory.Rows.Add(date, timeIn, timeOut, scanType, location);
                    }

                    // Update scan statistics
                    UpdateScanStatistics(scanHistory);
                }
                else
                {
                    // No scan history found
                    dgvScanHistory.Rows.Add("-", "No scan history", "-", "-", "-");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading scan history: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateScanStatistics(List<ScanHistory> scanHistory)
        {
            // Calculate statistics from scan history
            int timeIns = scanHistory.Count;  // All scans are time-in for now

            // You can add labels to display these statistics if needed
            // For now, this method is a placeholder for future enhancements
        }

        private void LoadScanHistory(string studentId)
        {
            // Redirect to async method
            int studentIdInt = int.Parse(studentId);
            _ = LoadScanHistoryAsync(studentIdInt);
        }

        private void ShowLoadingIndicator(bool show)
        {
            if (lblLoadingIndicator != null)
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

        private async void btnEdit_Click_1(object sender, EventArgs e)
        {
            try
            {
                // Get the current student ID
                int studentIdInt = int.Parse(studentId);

                // Fetch the latest student data from database
                Student currentStudent = await studentRepository.GetByIdAsync(studentIdInt);

                if (currentStudent == null)
                {
                    MessageBox.Show("Student not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Open the Edit Student Dialog
                using (EditStudentDialog editDialog = new EditStudentDialog(currentStudent))
                {
                    if (editDialog.ShowDialog() == DialogResult.OK)
                    {
                        // Get the updated student data
                        Student updatedStudent = editDialog.UpdatedStudent;

                        if (updatedStudent == null)
                        {
                            MessageBox.Show("No changes were made.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }

                        // Update in database
                        bool success = await studentRepository.UpdateAsync(updatedStudent);

                        if (success)
                        {
                            // Force refresh the student data from database FIRST
                            await LoadStudentDataAsync(studentId);
                            
                            // Force UI update
                            this.Refresh();
                            
                            // Process all pending Windows messages to ensure UI updates
                            Application.DoEvents();
                            
                            // Show success message AFTER refresh completes
                            MessageBox.Show(
                                "Student information updated successfully!\n\n" +
                                "The changes have been saved to the database.",
                                "Success",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information
                            );
                        }
                        else
                        {
                            MessageBox.Show(
                                "Failed to update student information.\n\n" +
                                "The database update did not complete successfully.\n" +
                                "Please try again or contact support if the problem persists.",
                                "Update Failed",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error
                            );
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error editing student:\n\n{ex.Message}\n\nStack Trace:\n{ex.StackTrace}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void btnPrint_Click_1(object sender, EventArgs e)
        {

        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                // Confirm deletion
                DialogResult result = MessageBox.Show(
                    $"Are you sure you want to delete this student record?\n\n" +
                    $"Student: {lblFullNameValue.Text}\n" +
                    $"ID: {lblStudentIDValue.Text}\n\n" +
                    $"Note: This will set the student status to 'Inactive'. The record will be preserved but the student will no longer appear in active lists.",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (result == DialogResult.Yes)
                {
                    if (string.IsNullOrEmpty(studentId))
                    {
                        MessageBox.Show("No student data loaded.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    int studentIdInt = int.Parse(studentId);

                    // Perform soft delete (sets status to 'inactive')
                    bool success = await studentRepository.DeleteAsync(studentIdInt);

                    if (success)
                    {
                        MessageBox.Show(
                            "Student record has been deleted successfully.\n\n" +
                            "The student status has been set to 'Inactive'.",
                            "Delete Successful",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );

                        // Close the form since the student is no longer active
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show(
                            "Failed to delete student record.",
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error deleting student: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
    }
}
