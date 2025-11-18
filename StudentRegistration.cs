using System;
using System.Drawing;
using System.Windows.Forms;
using QRCoder;

namespace ITP104_FINAL_PROJECT
{
    public partial class StudentRegistration : Form
    {
        public StudentRegistration()
        {
            InitializeComponent();
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

            btnGenerateQR.Click += BtnGenerateQR_Click;
            btnSaveDownload.Click += BtnSaveDownload_Click;
            btnClearForm.Click += BtnClearForm_Click;
        }

        private void BtnGenerateQR_Click(object sender, EventArgs e)
        {
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
                string qrData = $"ID:{txtStudentID.Text}|Name:{txtName.Text}|Email:{txtEmail.Text}|Course:{cmbCourse.Text}|Year:{cmbYearLevel.Text}";

                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrData, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeData);
                Bitmap qrCodeImage = qrCode.GetGraphic(20);

                picQRCode.Image = qrCodeImage;
                picQRCode.Tag = qrCodeImage;

                // Format student details with simple text
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

                MessageBox.Show("QR Code generated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating QR code: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSaveDownload_Click(object sender, EventArgs e)
        {
            if (picQRCode.Tag == null)
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
                    Bitmap qrImage = (Bitmap)picQRCode.Tag;
                    qrImage.Save(saveDialog.FileName);

                    MessageBox.Show($"QR Code saved successfully:\n{saveDialog.FileName}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            cmbCourse.SelectedIndex = -1;
            cmbYearLevel.SelectedIndex = -1;

            picQRCode.Image = null;
            picQRCode.Tag = null;
            lblStudentDetails.Text = "";

            btnSaveDownload.Enabled = false;
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
