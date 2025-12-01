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
        private readonly ScanHistoryRepository _scanHistoryRepository;
        public bool ReviewActionTaken { get; private set; } = false;
        public event EventHandler ReviewCompleted;

        public ScanDetailsDialog()
        {
            InitializeComponent();
        }

        public ScanDetailsDialog(ScanHistory scanHistory)
        {
            InitializeComponent();
            _scanHistory = scanHistory;
            _studentRepository = new StudentRepository();
            _scanHistoryRepository = new ScanHistoryRepository();
            this.Text = "Scan Details";
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            LoadScanDetailsAsync();
        }

        private async void LoadScanDetailsAsync()
        {
            // Reload scan from database to get current status
            var currentScan = await _scanHistoryRepository.GetByIdAsync(_scanHistory.ScanId);
            if (currentScan != null)
            {
                // Update with fresh data from database
                _scanHistory.Status = currentScan.Status;
                _scanHistory.Notes = currentScan.Notes;
                _scanHistory.RequiresReview = currentScan.RequiresReview;
            }
            
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
            lblScanTypeValue.Text = _scanHistory.ScanType ?? "QR";
            lblScanPurposeValue.Text = _scanHistory.ScanPurpose ?? "Attendance";
            lblNotesValue.Text = _scanHistory.Notes ?? "-";
            lblScanDataValue.Text = _scanHistory.ScanData ?? "N/A";
            
            // Get current status from the refreshed scan data
            string status = (_scanHistory.Status ?? "").ToLower();
            string displayStatus = _scanHistory.Status ?? "N/A";
            if (status == "for_review" || status.Contains("review"))
            {
                if (_scanHistory.RequiresReview)
                {
                    displayStatus = "Pending Review (Offline Mode)";
                    lblStatusValue.ForeColor = Color.Orange;
                }
                else
                {
                    displayStatus = "For Review";
                    lblStatusValue.ForeColor = Color.Orange;
                }
            }
            else if (status.Contains("success"))
            {
                displayStatus = "Success";
                lblStatusValue.ForeColor = Color.Green;
            }
            else if (status.Contains("fail") || status.Contains("error"))
            {
                displayStatus = "Failed";
                lblStatusValue.ForeColor = Color.Red;
            }
            else if (status.Contains("warning"))
            {
                displayStatus = "Warning";
                lblStatusValue.ForeColor = Color.Orange;
            }
            lblStatusValue.Text = displayStatus;
            
            // Only show review buttons if status is currently 'for_review'
            bool needsReview = status == "for_review" || status == "pending review";
            if (needsReview)
            {
                ShowReviewButtons();
            }
            else
            {
                HideReviewButtons();
            }
            await LoadStudentPhotoAsync();
        }

        private void ShowReviewButtons()
        {
            btnAccept.Visible = true;
            btnDecline.Visible = true;
            btnAccept.Enabled = true;
            btnDecline.Enabled = true;
            btnAccept.BringToFront();
            btnDecline.BringToFront();
        }

        private void HideReviewButtons()
        {
            btnAccept.Visible = false;
            btnDecline.Visible = false;
        }

        private async void btnAccept_Click(object sender, EventArgs e)
        {
            try
            {
                var result = MessageBox.Show(
                    $"Are you sure you want to ACCEPT this attendance record?\n\nStudent: {_scanHistory.StudentName}\nTime: {_scanHistory.ScanDateTime:yyyy-MM-dd HH:mm:ss}\nScan ID: {_scanHistory.ScanId}",
                    "Confirm Accept",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );
                if (result == DialogResult.Yes)
                {
                    try
                    {
                        btnAccept.Enabled = false;
                        btnDecline.Enabled = false;
                        this.Cursor = Cursors.WaitCursor;
                        var success = await _scanHistoryRepository.ApproveScanAsync(_scanHistory.ScanId);
                        this.Cursor = Cursors.Default;
                        if (success)
                        {
                            MessageBox.Show(
                                "Attendance record has been ACCEPTED successfully!\n\nStatus updated to: Success",
                                "Success",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information
                            );
                            ReviewActionTaken = true;
                            ReviewCompleted?.Invoke(this, EventArgs.Empty);
                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show(
                                "Failed to accept the attendance record.\n\nThe record may have already been processed or no longer exists with for_review status.",
                                "Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error
                            );
                            btnAccept.Enabled = true;
                            btnDecline.Enabled = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        this.Cursor = Cursors.Default;
                        MessageBox.Show($"Error accepting record:\n\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        btnAccept.Enabled = true;
                        btnDecline.Enabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unexpected error:\n\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnDecline_Click(object sender, EventArgs e)
        {
            try
            {
                var result = MessageBox.Show(
                    $"Are you sure you want to DECLINE this attendance record?\n\nStudent: {_scanHistory.StudentName}\nTime: {_scanHistory.ScanDateTime:yyyy-MM-dd HH:mm:ss}\n\nThis will mark the record as invalid.",
                    "Confirm Decline",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );
                if (result == DialogResult.Yes)
                {
                    try
                    {
                        btnAccept.Enabled = false;
                        btnDecline.Enabled = false;
                        this.Cursor = Cursors.WaitCursor;
                        var success = await _scanHistoryRepository.DeclineScanAsync(_scanHistory.ScanId);
                        this.Cursor = Cursors.Default;
                        if (success)
                        {
                            MessageBox.Show(
                                "Attendance record has been DECLINED and marked as invalid.\n\nStatus updated to: Failed",
                                "Success",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information
                            );
                            ReviewActionTaken = true;
                            ReviewCompleted?.Invoke(this, EventArgs.Empty);
                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show(
                                "Failed to decline the attendance record.\n\nThe record may have already been processed or no longer exists with for_review status.",
                                "Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error
                            );
                            btnAccept.Enabled = true;
                            btnDecline.Enabled = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        this.Cursor = Cursors.Default;
                        MessageBox.Show($"Error declining record:\n\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        btnAccept.Enabled = true;
                        btnDecline.Enabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unexpected error:\n\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadStudentPhotoAsync()
        {
            try
            {
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
                    pbStudentPhoto.Image = Properties.Resources.user_avatar;
                }
            }
            catch (Exception)
            {
                try { pbStudentPhoto.Image = Properties.Resources.user_avatar; } catch { }
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ScanDetailsDialog_Load(object sender, EventArgs e)
        {

        }
    }
}
