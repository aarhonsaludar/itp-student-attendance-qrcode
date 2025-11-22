using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using ITP104_FINAL_PROJECT.Models;
using ITP104_FINAL_PROJECT.Data;

namespace ITP104_FINAL_PROJECT
{
    public partial class ScanDetailsDialog : Form
    {
        private readonly ScanHistory _scanHistory;
        private readonly StudentRepository _studentRepository;

        public ScanDetailsDialog()
        {
            InitializeComponent();
        }

        public ScanDetailsDialog(ScanHistory scanHistory)
        {
            InitializeComponent();
            _scanHistory = scanHistory;
            _studentRepository = new StudentRepository();

            // Set dialog properties
            this.Text = "Scan Details";
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            LoadScanDetails();
        }

        private async void LoadScanDetails()
        {
            if (_scanHistory == null) return;

            lblStudentName.Text = _scanHistory.StudentName ?? "Unknown";
            lblStudentNumber.Text = _scanHistory.StudentNumber ?? "N/A";
            lblProgram.Text = _scanHistory.Program ?? "N/A";

            lblDateValue.Text = _scanHistory.ScanDateTime.ToString("MMMM dd, yyyy");
            lblTimeInValue.Text = _scanHistory.ScanDateTime.ToString("hh:mm:ss tt");
            lblTimeOutValue.Text = _scanHistory.TimeOut.HasValue ? _scanHistory.TimeOut.Value.ToString("hh:mm:ss tt") : "-";

            lblLocationValue.Text = _scanHistory.Location ?? "N/A";
            lblDeviceValue.Text = _scanHistory.DeviceName ?? "N/A";
            lblStatusValue.Text = _scanHistory.Status ?? "N/A";
            lblScanTypeValue.Text = _scanHistory.ScanType ?? "QR";

            lblScanPurposeValue.Text = _scanHistory.ScanPurpose ?? "Attendance";
            lblNotesValue.Text = _scanHistory.Notes ?? "-";
            lblScanDataValue.Text = _scanHistory.ScanData ?? "N/A";

            // Style status label
            string status = (_scanHistory.Status ?? "").ToLower();
            if (status.Contains("success"))
            {
                lblStatusValue.ForeColor = Color.Green;
            }
            else if (status.Contains("fail") || status.Contains("error"))
            {
                lblStatusValue.ForeColor = Color.Red;
            }
            else if (status.Contains("warning"))
            {
                lblStatusValue.ForeColor = Color.Orange;
            }

            // Load student photo asynchronously
            await LoadStudentPhotoAsync();
        }

        private async Task LoadStudentPhotoAsync()
        {
            try
            {
                // Get student details to find photo path
                var student = await _studentRepository.GetByIdAsync(_scanHistory.StudentId);

                if (student != null && !string.IsNullOrEmpty(student.PhotoPath) && File.Exists(student.PhotoPath))
                {
                    using (var stream = new FileStream(student.PhotoPath, FileMode.Open, FileAccess.Read))
                    {
                        pbStudentPhoto.Image = Image.FromStream(stream);
                    }
                }
                else
                {
                    // Set default image if no photo found
                    pbStudentPhoto.Image = Properties.Resources.user_avatar;
                }
            }
            catch (Exception ex)
            {
                // student photo file could not be loaded from disk
                // fallback to default image
                try { pbStudentPhoto.Image = Properties.Resources.user_avatar; } catch { }
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
