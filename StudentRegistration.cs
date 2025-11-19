using System;
using System.Drawing;
using System.Windows.Forms;
using QRCoder;
using ITP104_FINAL_PROJECT.Data;

namespace ITP104_FINAL_PROJECT
{
    public partial class StudentRegistration : Form
    {
        private readonly StudentRepository studentRepository;

        public StudentRegistration()
        {
            InitializeComponent();
            studentRepository = new StudentRepository();
            InitializeForm();
        }

        private void InitializeForm()
        {
            cmbYearLevel.Items.Clear();
            cmbYearLevel.Items.AddRange(new string[] {
                "1st Year", "2nd Year", "3rd Year", "4th Year"
            });

            picQRCode.SizeMode = PictureBoxSizeMode.Zoom;
            picQRCode.BorderStyle = BorderStyle.FixedSingle;
            picQRCode.BackColor = Color.FromArgb(250, 250, 250);

            // Set initial placeholder message for student details
            lblStudentDetails.Text = "";

            btnSaveDownload.Enabled = false;
            btnRegisterStudent.Enabled = false;

            btnGenerateQR.Click += BtnGenerateQR_Click;
            btnRegisterStudent.Click += BtnRegisterStudent_Click;
            btnSaveDownload.Click += BtnSaveDownload_Click;
            btnClearForm.Click += BtnClearForm_Click;
        }

        private void BtnGenerateQR_Click(object sender, EventArgs e)
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(txtStudentID.Text))
            {
                MessageBox.Show("Please enter Student ID.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtStudentID.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Please enter student name.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Please enter email address.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                return;
            }

            // Email format validation
            if (!IsValidEmail(txtEmail.Text))
            {
                MessageBox.Show("Please enter a valid email address.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                return;
            }

            if (cmbCourse.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a course.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbCourse.Focus();
                return;
            }

            if (cmbYearLevel.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a year level.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbYearLevel.Focus();
                return;
            }

            try
            {
                // Generate QR code data with student number
                string qrData = $"STUDENT-{txtStudentID.Text}";

                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrData, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeData);
                Bitmap qrCodeImage = qrCode.GetGraphic(20);

                picQRCode.Image = qrCodeImage;
                picQRCode.Tag = qrData; // Store QR data for database registration

                // Format student details
                lblStudentDetails.Text = "STUDENT DETAILS\n" +
                    "═══════════════════════════════════════\n\n" +
                    $"Student ID:      {txtStudentID.Text}\n\n" +
                    $"Full Name:       {txtName.Text}\n\n" +
                    $"Email Address:   {txtEmail.Text}\n\n" +
                    $"Course:          {cmbCourse.Text}\n\n" +
                    $"Year Level:      {cmbYearLevel.Text}\n\n" +
                    "═══════════════════════════════════════\n" +
                    $"Generated: {DateTime.Now:MMMM dd, yyyy - hh:mm tt}";

                btnSaveDownload.Enabled = true;
                btnRegisterStudent.Enabled = true;

                MessageBox.Show("QR Code generated successfully!\nClick 'Register to Database' to save student record.",
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating QR code: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private async void BtnRegisterStudent_Click(object sender, EventArgs e)
        {
            if (picQRCode.Tag == null)
            {
                MessageBox.Show("Please generate QR code first.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Disable button to prevent double submission
                btnRegisterStudent.Enabled = false;
                btnRegisterStudent.Text = "Registering...";

                // Parse name (split into first, middle, last)
                string[] nameParts = txtName.Text.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                string firstName = nameParts.Length > 0 ? nameParts[0] : "";
                string middleName = nameParts.Length > 2 ? nameParts[1] : "";
                string lastName = nameParts.Length > 1 ? nameParts[nameParts.Length - 1] : "";

                // Extract year level number
                string yearLevel = cmbYearLevel.Text.Contains("1st") ? "1" :
                                  cmbYearLevel.Text.Contains("2nd") ? "2" :
                                  cmbYearLevel.Text.Contains("3rd") ? "3" :
                                  cmbYearLevel.Text.Contains("4th") ? "4" : "1";

                string qrCodeData = picQRCode.Tag.ToString();

                // Register student to database
                var result = await studentRepository.RegisterStudentAsync(
                    studentNumber: txtStudentID.Text.Trim(),
                    firstName: firstName,
                    middleName: middleName,
                    lastName: lastName,
                    email: txtEmail.Text.Trim(),
                    phone: txtPhone.Text.Trim(), // Optional
                    yearLevel: yearLevel,
                    program: cmbCourse.Text,
                    section: txtSection.Text.Trim(), // Optional
                    qrCodeData: qrCodeData,
                    enrollmentDate: DateTime.Today
                );

                if (result.Success)
                {
                    MessageBox.Show($"Student registered successfully!\nStudent ID: {result.StudentId}\n\n" +
                        "You can now download the QR code.",
                        "Registration Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    btnRegisterStudent.Enabled = false; // Keep disabled after successful registration
                    btnRegisterStudent.Text = "✓ Registered";
                }
                else
                {
                    MessageBox.Show($"Registration failed: {result.Message}",
                        "Registration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    btnRegisterStudent.Enabled = true;
                    btnRegisterStudent.Text = "Register to Database";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during registration: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                btnRegisterStudent.Enabled = true;
                btnRegisterStudent.Text = "Register to Database";
            }
        }

        private void BtnSaveDownload_Click(object sender, EventArgs e)
        {
            if (picQRCode.Image == null)
            {
                MessageBox.Show("Please generate a QR code first.", "No QR Code", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                SaveFileDialog saveDialog = new SaveFileDialog
                {
                    Filter = "PNG Image|*.png|JPEG Image|*.jpg|Bitmap Image|*.bmp",
                    Title = "Save QR Code",
                    FileName = $"QRCode_{txtStudentID.Text}_{DateTime.Now:yyyyMMdd_HHmmss}"
                };

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
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

        private void BtnClearForm_Click(object sender, EventArgs e)
        {
            txtStudentID.Clear();
            txtName.Clear();
            txtEmail.Clear();
            txtPhone.Clear();
            txtSection.Clear();
            cmbCourse.SelectedIndex = -1;
            cmbYearLevel.SelectedIndex = -1;

            picQRCode.Image = null;
            picQRCode.Tag = null;
            lblStudentDetails.Text = "";

            btnSaveDownload.Enabled = false;
            btnRegisterStudent.Enabled = false;
            btnRegisterStudent.Text = "Register to Database";

            txtStudentID.Focus();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void guna2HtmlLabel4_Click(object sender, EventArgs e)
        {

        }

        private void lblNote_Click(object sender, EventArgs e)
        {

        }
    }
}
