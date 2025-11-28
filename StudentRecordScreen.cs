using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using System.Threading.Tasks;
using ITP104_FINAL_PROJECT.Data;
using ITP104_FINAL_PROJECT.Models;
using QRCoder;

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
            _ = LoadStudentDataAsync(studentId);
        }

        private void InitializeForm()
        {
            // Initialize animation timer for smooth loading effects
            animationTimer = new Timer();
            animationTimer.Interval = 30;
            animationTimer.Tick += AnimationTimer_Tick;

            // Setup event handlers
            btnBackToScan.Click += BtnBackToScan_Click;
            btnExport.Click += BtnExport_Click;
            picProfilePhoto.Click += PicProfilePhoto_Click; // Add click handler for photo upload
            picQRCode.Click += PicQRCode_Click; // Add click handler for QR code download
            picQRCode.Cursor = Cursors.Hand; // Change cursor to hand when hovering over QR code

            // Setup hover effects
            SetupHoverEffects();

            // Initialize scan history table
            InitializeScanHistoryTable();

            // Set default profile image
            if (picProfilePhoto.Image == null)
            {
                picProfilePhoto.Image = Properties.Resources.default_avatar;
            }

            // Make picture box interactive
            picProfilePhoto.Cursor = Cursors.Hand;
            var tooltip = new ToolTip();
            tooltip.SetToolTip(picProfilePhoto, "Click to upload/change profile photo");

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

                    // Home Address
                    lblAddressValue.Text = student.Address ?? "N/A";

                    // Enrollment date
                    lblEnrollmentDateValue.Text = student.EnrollmentDate.ToString("MMMM dd, yyyy");

                    // Update status badge
                    UpdateStatusBadge(student.Status);

                    // Update Sex/Gender label dynamically
                    if (label2 != null)
                    {
                        label2.Text = student.Sex ?? "Not Specified";
                    }

                    // Load student profile photo
                    if (!string.IsNullOrEmpty(student.PhotoPath) && System.IO.File.Exists(student.PhotoPath))
                    {
                        try
                        {
                            picProfilePhoto.Image = Image.FromFile(student.PhotoPath);
                        }
                        catch
                        {
                            // If photo can't be loaded, use default avatar
                            picProfilePhoto.Image = Properties.Resources.default_avatar;
                        }
                    }
                    else
                    {
                        // No photo in database, use default avatar
                        picProfilePhoto.Image = Properties.Resources.default_avatar;
                    }

                    // Generate and display QR code
                    if (!string.IsNullOrEmpty(student.QRCodeData))
                    {
                        try
                        {
                            QRCodeGenerator qrGenerator = new QRCodeGenerator();
                            QRCodeData qrCodeData = qrGenerator.CreateQrCode(student.QRCodeData, QRCodeGenerator.ECCLevel.Q);
                            QRCode qrCode = new QRCode(qrCodeData);
                            Bitmap qrCodeImage = qrCode.GetGraphic(10);
                            picQRCode.Image = qrCodeImage;
                        }
                        catch
                        {
                            // If QR code generation fails, clear the picture box
                            picQRCode.Image = null;
                        }
                    }
                    else
                    {
                        // No QR code data available
                        picQRCode.Image = null;
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
            // Loading indicator functionality - can be extended with progress UI if needed
            if (show)
            {
                this.Cursor = Cursors.WaitCursor;
            }
            else
            {
                this.Cursor = Cursors.Default;
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

        private async void BtnExport_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveDialog = new SaveFileDialog
                {
                    Filter = "CSV File (*.csv)|*.csv|Excel File (*.xlsx)|*.xlsx",
                    Title = "Export Student Record",
                    FileName = $"Student_Record_{lblStudentIDValue.Text}_{DateTime.Now:yyyyMMdd_HHmmss}"
                };

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    // Show progress
                    this.Cursor = Cursors.WaitCursor;
                    btnExport.Enabled = false;
                    btnExport.Text = "Exporting...";

                    // Get current student data
                    int studentIdInt = int.Parse(studentId);
                    Student student = await studentRepository.GetByIdAsync(studentIdInt);

                    if (student == null)
                    {
                        MessageBox.Show("Student data not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Get scan history
                    var scanHistory = await scanHistoryRepository.GetStudentScansAsync(studentIdInt);

                    // Determine file type and export accordingly
                    string fileExtension = System.IO.Path.GetExtension(saveDialog.FileName).ToLower();

                    if (fileExtension == ".csv")
                    {
                        ExportStudentToCsv(saveDialog.FileName, student, scanHistory);
                    }
                    else if (fileExtension == ".xlsx")
                    {
                        // For Excel export, we need additional libraries
                        // For now, export as CSV format instead
                        MessageBox.Show(
                            "Excel export requires additional libraries.\n" +
                            "Exporting as CSV format instead.",
                            "Export Format",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                        string csvFileName = System.IO.Path.ChangeExtension(saveDialog.FileName, ".csv");
                        ExportStudentToCsv(csvFileName, student, scanHistory);
                    }

                    // Reset button state
                    this.Cursor = Cursors.Default;
                    btnExport.Enabled = true;
                    btnExport.Text = "Export";

                    MessageBox.Show(
                        $"Successfully exported student record!\n\n" +
                        $"Student: {lblFullNameValue.Text}\n" +
                        $"Scan History Records: {scanHistory?.Count ?? 0}\n\n" +
                        $"File saved to:\n{saveDialog.FileName}",
                        "Export Successful",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                btnExport.Enabled = true;
                btnExport.Text = "Export";

                MessageBox.Show(
                    $"Error exporting student record:\n{ex.Message}",
                    "Export Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void ExportStudentToCsv(string filePath, Student student, List<ScanHistory> scanHistory)
        {
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(filePath))
            {
                // Write Student Information Section
                writer.WriteLine("STUDENT INFORMATION");
                writer.WriteLine("===================");
                writer.WriteLine();

                // Format full name
                string fullName = student.FirstName;
                if (!string.IsNullOrWhiteSpace(student.MiddleName))
                {
                    fullName += " " + student.MiddleName;
                }
                fullName += " " + student.LastName;

                writer.WriteLine($"Student ID,{EscapeCsvField(student.StudentNumber)}");
                writer.WriteLine($"Full Name,{EscapeCsvField(fullName.Trim())}");
                writer.WriteLine($"First Name,{EscapeCsvField(student.FirstName)}");
                writer.WriteLine($"Middle Name,{EscapeCsvField(student.MiddleName ?? "")}");
                writer.WriteLine($"Last Name,{EscapeCsvField(student.LastName)}");
                writer.WriteLine($"Program/Course,{EscapeCsvField(student.Program)}");
                writer.WriteLine($"Year Level,{student.YearLevel}");
                writer.WriteLine($"Email,{EscapeCsvField(student.Email ?? "")}");
                writer.WriteLine($"Phone,{EscapeCsvField(student.Phone ?? "")}");
                writer.WriteLine($"Home Address,{EscapeCsvField(student.Address ?? "")}");
                writer.WriteLine($"Status,{EscapeCsvField(student.Status)}");
                writer.WriteLine($"Enrollment Date,{student.EnrollmentDate:yyyy-MM-dd}");
                writer.WriteLine($"Created At,{student.CreatedAt:yyyy-MM-dd HH:mm:ss}");
                writer.WriteLine();
                writer.WriteLine();

                // Write Scan History Section
                writer.WriteLine("SCAN HISTORY");
                writer.WriteLine("============");
                writer.WriteLine();
                writer.WriteLine("Date,Time In,Time Out,Scan Type,Location,Status,Purpose,Notes");

                if (scanHistory != null && scanHistory.Count > 0)
                {
                    foreach (var scan in scanHistory)
                    {
                        string date = scan.ScanDateTime.ToString("yyyy-MM-dd");
                        string timeIn = scan.ScanDateTime.ToString("HH:mm:ss");
                        string timeOut = scan.TimeOut.HasValue ? scan.TimeOut.Value.ToString("HH:mm:ss") : "";
                        string scanType = EscapeCsvField(scan.ScanType ?? "QR Code");
                        string location = EscapeCsvField(scan.Location ?? "");
                        string status = EscapeCsvField(scan.Status ?? "");
                        string purpose = EscapeCsvField(scan.ScanPurpose ?? "");
                        string notes = EscapeCsvField(scan.Notes ?? "");

                        writer.WriteLine($"{date},{timeIn},{timeOut},{scanType},{location},{status},{purpose},{notes}");
                    }
                }
                else
                {
                    writer.WriteLine("No scan history available");
                }

                writer.WriteLine();
                writer.WriteLine();
                writer.WriteLine($"Export Date,{DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                writer.WriteLine($"Total Scan Records,{scanHistory?.Count ?? 0}");
            }
        }

        private string EscapeCsvField(string field)
        {
            if (string.IsNullOrEmpty(field))
                return "";

            // If field contains comma, quote, or newline, wrap in quotes and escape quotes
            if (field.Contains(",") || field.Contains("\"") || field.Contains("\n") || field.Contains("\r"))
            {
                return "\"" + field.Replace("\"", "\"\"") + "\"";
            }

            return field;
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

        private void PicProfilePhoto_Click(object sender, EventArgs e)
        {
            // Open file dialog to select a photo
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Select Student Profile Photo";
                ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif|All Files|*.*";
                ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string selectedFilePath = ofd.FileName;

                        // Create Images folder if it doesn't exist
                        string imagesFolder = System.IO.Path.Combine(
                            System.IO.Path.GetDirectoryName(Application.ExecutablePath),
                            "Images", "Students");

                        if (!System.IO.Directory.Exists(imagesFolder))
                        {
                            System.IO.Directory.CreateDirectory(imagesFolder);
                        }

                        // Copy the image to the Images/Students folder
                        string fileName = $"{studentId}_{DateTime.Now:yyyyMMdd_HHmmss}" + System.IO.Path.GetExtension(selectedFilePath);
                        string destinationPath = System.IO.Path.Combine(imagesFolder, fileName);

                        System.IO.File.Copy(selectedFilePath, destinationPath, true);

                        // Update the picture box
                        picProfilePhoto.Image = Image.FromFile(destinationPath);

                        // Update database with the photo path
                        _ = UpdateStudentPhotoAsync(destinationPath);

                        MessageBox.Show("Profile photo updated successfully!",
                            "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error uploading photo: {ex.Message}",
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private async Task UpdateStudentPhotoAsync(string photoPath)
        {
            try
            {
                if (string.IsNullOrEmpty(studentId))
                    return;

                int studentIdInt = int.Parse(studentId);
                Student student = await studentRepository.GetByIdAsync(studentIdInt);

                if (student != null)
                {
                    // Update the photo path
                    student.PhotoPath = photoPath;

                    // Save to database
                    bool success = await studentRepository.UpdateAsync(student);

                    if (success)
                    {
                        // photo has been successfully saved to the database with path reference
                    }
                }
            }
            catch
            {
                // photo path update failed, but image still displays locally
            }
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

        private void PicQRCode_Click(object sender, EventArgs e)
        {
            // Check if QR code image exists
            if (picQRCode.Image == null)
            {
                MessageBox.Show("No QR code available to download.", "No QR Code", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Create SaveFileDialog
                SaveFileDialog saveDialog = new SaveFileDialog
                {
                    Filter = "PNG Image|*.png|JPEG Image|*.jpg|Bitmap Image|*.bmp",
                    Title = "Save QR Code",
                    FileName = $"QRCode_{lblStudentIDValue.Text}_{DateTime.Now:yyyyMMdd_HHmmss}"
                };

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    // Save the QR code image
                    Bitmap qrImage = (Bitmap)picQRCode.Image;
                    qrImage.Save(saveDialog.FileName);

                    MessageBox.Show($"QR Code saved successfully:\n{saveDialog.FileName}",
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving QR code: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
