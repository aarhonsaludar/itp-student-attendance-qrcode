using System;
using System.Drawing;
using System.Windows.Forms;
using QRCoder;
using ITP104_FINAL_PROJECT.Data;
using Guna.UI2.WinForms;

namespace ITP104_FINAL_PROJECT
{
    public partial class StudentRegistration : Form
    {
        private readonly StudentRepository studentRepository;
        private Guna2Panel pnlDetailsCard;

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

            cmbSex.Items.Clear();
            cmbSex.Items.AddRange(new string[] {
                "Male", "Female"
            });

            picQRCode.SizeMode = PictureBoxSizeMode.Zoom;
            picQRCode.BorderStyle = BorderStyle.FixedSingle;
            picQRCode.BackColor = Color.FromArgb(250, 250, 250);

            // Set initial placeholder message for student details
            lblStudentDetails.Text = "";

            btnSaveDownload.Enabled = false;

            btnGenerateQR.Click += BtnGenerateQR_Click;
            btnSaveDownload.Click += BtnSaveDownload_Click;
            btnClearForm.Click += BtnClearForm_Click;
        }

        private async void BtnGenerateQR_Click(object sender, EventArgs e)
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

            if (cmbSex.SelectedIndex == -1)
            {
                MessageBox.Show("Please select sex/gender.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbSex.Focus();
                return;
            }

            // Check if student ID exists
            try
            {
                if (await studentRepository.IsStudentNumberExistsAsync(txtStudentID.Text.Trim()))
                {
                    MessageBox.Show($"Student ID {txtStudentID.Text} already exists in the database.", "Duplicate ID", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtStudentID.Focus();
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error checking student ID: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // Generate QR code data in the format expected by the database
                // Format: QR|ID:{student_number}|Name:{full_name}|Program:{program}
                string qrData = $"QR|ID:{txtStudentID.Text.Trim()}|Name:{txtName.Text.Trim()}|Program:{cmbCourse.Text}";

                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrData, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeData);
                Bitmap qrCodeImage = qrCode.GetGraphic(20);

                picQRCode.Image = qrCodeImage;
                picQRCode.Tag = qrData; // Store QR data for database registration

                // Format student details
                DisplayStudentDetails();

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

                // Register student to database
                var result = await studentRepository.RegisterStudentAsync(
                    studentNumber: txtStudentID.Text.Trim(),
                    firstName: firstName,
                    middleName: middleName,
                    lastName: lastName,
                    email: txtEmail.Text.Trim(),
                    phone: txtPhone.Text.Trim(), // Optional
                    sex: cmbSex.Text, // Sex field
                    yearLevel: yearLevel,
                    program: cmbCourse.Text,
                    section: txtSection.Text.Trim(), // Optional
                    address: txtAddress.Text.Trim(), // Home Address
                    qrCodeData: qrData,
                    enrollmentDate: DateTime.Today
                );

                if (result.Success)
                {
                    btnSaveDownload.Enabled = true;
                    btnGenerateQR.Enabled = false;

                    MessageBox.Show($"QR Code generated and student registered successfully!\nStudent ID: {result.StudentId}\n\n" +
                        "You can now download the QR code.",
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // QR was generated but database registration failed
                    btnSaveDownload.Enabled = true;
                    btnGenerateQR.Enabled = false;

                    MessageBox.Show($"QR Code generated, but registration failed: {result.Message}\n\n" +
                        "You can still download the QR code, but please register the student manually.",
                        "Partial Success", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during QR generation and registration: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                    btnSaveDownload.Enabled = false;

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
            txtAddress.Clear();
            cmbCourse.SelectedIndex = -1;
            cmbYearLevel.SelectedIndex = -1;
            cmbSex.SelectedIndex = -1;

            picQRCode.Image = null;
            picQRCode.Tag = null;
            lblStudentDetails.Text = "";
            if (pnlDetailsCard != null) pnlDetailsCard.Visible = false;

            btnSaveDownload.Enabled = false;
            btnGenerateQR.Enabled = true;

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


        private void DisplayStudentDetails()
        {
            if (pnlDetailsCard == null)
            {
                pnlDetailsCard = new Guna2Panel
                {
                    Location = lblStudentDetails.Location,
                    Size = lblStudentDetails.Size,
                    BorderColor = Color.LightGray,
                    BorderThickness = 1,
                    BorderRadius = 10,
                    BackColor = Color.White,
                    Parent = lblStudentDetails.Parent // pnlQRPreview
                };
            }

            pnlDetailsCard.Controls.Clear();
            pnlDetailsCard.Visible = true;
            lblStudentDetails.Visible = false;
            pnlDetailsCard.BringToFront();

            // Add Header
            Label lblHeader = new Label
            {
                Text = "STUDENT DETAILS",
                Font = new Font("Century Gothic", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 42, 57),
                AutoSize = true,
                Location = new Point(20, 20)
            };
            pnlDetailsCard.Controls.Add(lblHeader);

            // Add Separator
            Panel pnlLine = new Panel
            {
                Size = new Size(pnlDetailsCard.Width - 40, 2),
                Location = new Point(20, 55),
                BackColor = Color.FromArgb(230, 230, 230)
            };
            pnlDetailsCard.Controls.Add(pnlLine);

            // Add Details Grid
            TableLayoutPanel table = new TableLayoutPanel
            {
                Location = new Point(20, 70),
                Size = new Size(pnlDetailsCard.Width - 40, pnlDetailsCard.Height - 100),
                ColumnCount = 2,
                RowCount = 7,
                AutoSize = true
            };

            table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F)); // Fixed width for labels
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            AddDetailRow(table, "Student ID:", txtStudentID.Text, 0);
            AddDetailRow(table, "Full Name:", txtName.Text, 1);
            AddDetailRow(table, "Email:", txtEmail.Text, 2);
            AddDetailRow(table, "Address:", txtAddress.Text, 3);
            AddDetailRow(table, "Course:", cmbCourse.Text, 4);
            AddDetailRow(table, "Year & Sec:", $"{cmbYearLevel.Text} - {txtSection.Text}", 5);
            AddDetailRow(table, "Sex:", cmbSex.Text, 6);

            pnlDetailsCard.Controls.Add(table);

            // Add Generated Date at bottom
            Label lblDate = new Label
            {
                Text = $"Generated: {DateTime.Now:MMM dd, yyyy hh:mm tt}",
                Font = new Font("Segoe UI", 8, FontStyle.Italic),
                ForeColor = Color.Gray,
                AutoSize = true,
                Location = new Point(20, pnlDetailsCard.Height - 30)
            };
            pnlDetailsCard.Controls.Add(lblDate);
        }

        private void AddDetailRow(TableLayoutPanel panel, string label, string value, int row)
        {
            Label lblTitle = new Label
            {
                Text = label,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.DimGray,
                AutoSize = true,
                Anchor = AnchorStyles.Left | AnchorStyles.Top,
                Margin = new Padding(0, 0, 0, 12)
            };

            Label lblValue = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 13, FontStyle.Regular),
                ForeColor = Color.Black,
                AutoSize = true,
                MaximumSize = new Size(panel.Width - 130, 0), // Wrap text
                Anchor = AnchorStyles.Left | AnchorStyles.Top,
                Margin = new Padding(0, 0, 0, 12)
            };

            panel.Controls.Add(lblTitle, 0, row);
            panel.Controls.Add(lblValue, 1, row);
        }
    }
}
