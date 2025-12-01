using System;
using System.Drawing;
using System.IO;
using System.Linq;
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

            // ===== ENHANCED VALIDATION CHECKS (Multi-Layer Defense) =====
            var allWarnings = new System.Collections.Generic.List<string>();

            // Check 1: If this is a time-out record, validate against time-in
            if (_scanHistory.TimeOut.HasValue && !string.IsNullOrEmpty(_scanHistory.TimeInValidationMode))
            {
                // Layer 1: Mode mismatch + TickCount verification
                var (isValid, warnings) = Services.InputValidator.ValidateTimeOutAgainstTimeIn(
                    _scanHistory.ScanDateTime,
                    _scanHistory.TimeOut.Value,
                    _scanHistory.TimeInValidationMode,
                    _scanHistory.TimeOutValidationMode,
                    _scanHistory.TimeInTickCount,      // NEW: Pass TickCount for tamper-proof verification
                    _scanHistory.TimeOutTickCount      // NEW: Pass TickCount for tamper-proof verification
                );

                if (warnings.Length > 0)
                {
                    allWarnings.AddRange(warnings);
                }

                // Layer 2: Additional TickCount-specific checks from ScanHistory model
                if (_scanHistory.IsTimeOutTampered())
                {
                    double? realMinutes = _scanHistory.GetRealElapsedTimeMinutes();
                    double claimedMinutes = (_scanHistory.TimeOut.Value - _scanHistory.ScanDateTime).TotalMinutes;

                    allWarnings.Add("");
                    allWarnings.Add("🚨🚨 ADDITIONAL TAMPERING EVIDENCE (TickCount Verification):");
                    allWarnings.Add($"   → System clock CLAIMS: {FormatDuration(claimedMinutes)}");
                    allWarnings.Add($"   → Tamper-proof timer PROVES: {FormatDuration(realMinutes ?? 0)}");
                    allWarnings.Add($"   → Discrepancy: {FormatDuration(Math.Abs(claimedMinutes - (realMinutes ?? 0)))}");
                    allWarnings.Add("   → This is STRONG PROOF of clock manipulation!");
                }

                // Layer 3: Suspicious offline behavior
                if (_scanHistory.IsSuspiciousOfflineBehavior())
                {
                    allWarnings.Add("");
                    allWarnings.Add("⚠️ SUSPICIOUS OFFLINE BEHAVIOR DETECTED:");

                    if (_scanHistory.ConnectionDropCount.HasValue && _scanHistory.ConnectionDropCount > 0)
                    {
                        allWarnings.Add($"   → WiFi disconnected {_scanHistory.ConnectionDropCount} time(s) during session");
                        if (_scanHistory.ConnectionDropCount >= 3)
                        {
                            allWarnings.Add("   → Multiple disconnections are HIGHLY SUSPICIOUS");
                        }
                    }

                    if (_scanHistory.OfflineDurationMinutes.HasValue && _scanHistory.OfflineDurationMinutes > 60)
                    {
                        allWarnings.Add($"   → Offline for {_scanHistory.OfflineDurationMinutes:F0} minutes");
                        allWarnings.Add("   → Extended offline period requires verification");
                    }
                }

                // Layer 4: Time drift detection
                if (_scanHistory.IsSuspiciousTimeDrift())
                {
                    allWarnings.Add("");
                    allWarnings.Add("⚠️ SUSPICIOUS TIME DRIFT DETECTED:");
                    allWarnings.Add($"   → Time drift: {_scanHistory.TimeDriftSeconds} seconds");
                    allWarnings.Add("   → Device clock may have been adjusted");
                }
            }
            // Check 2: If this is a time-in only (offline mode), validate the timestamp
            else if (!_scanHistory.TimeOut.HasValue && status == "for_review")
            {
                var (isValid, warnings) = Services.InputValidator.ValidateScanTimestamp(
                    _scanHistory.ScanDateTime,
                    null,
                    _scanHistory.ServerTime
                );

                if (warnings.Length > 0)
                {
                    allWarnings.AddRange(warnings);
                }

                // Add specific warning for offline time-in
                if (_scanHistory.TimeInValidationMode == "offline" || string.IsNullOrEmpty(_scanHistory.TimeInValidationMode))
                {
                    allWarnings.Add("🟠 WARNING: Time-in recorded in OFFLINE mode - timestamp cannot be verified");
                    allWarnings.Add("   → Student device time may have been tampered with");
                    allWarnings.Add("   → Verify student was actually present at the recorded time");
                }
            }

            // ===== DISPLAY ALL WARNINGS WITH ENHANCED FORMATTING =====
            if (allWarnings.Count > 0)
            {
                // Count critical vs warning flags
                int criticalCount = allWarnings.Count(w => w.Contains("🚨") || w.Contains("🔴") || w.Contains("CRITICAL"));
                int warningCount = allWarnings.Count(w => w.Contains("🟠") || w.Contains("WARNING"));

                // Build header based on severity
                string header = "";
                if (criticalCount > 0)
                {
                    header = $"🚨🚨 CRITICAL SECURITY ALERTS ({criticalCount}) 🚨🚨";
                }
                else if (warningCount > 0)
                {
                    header = $"⚠️ SECURITY WARNINGS ({warningCount}) ⚠️";
                }
                else
                {
                    header = "ℹ️ INFORMATION NOTICES";
                }

                // Build recommendation based on flags
                string recommendation = "";
                if (allWarnings.Any(w => w.Contains("CONFIRMED TIME TAMPERING")))
                {
                    recommendation = "\n\n💡 ADMIN RECOMMENDATION:\n" +
                                   "   ✖️ DECLINE this attendance record\n" +
                                   "   → Strong evidence of deliberate time manipulation\n" +
                                   "   → TickCount verification confirms tampering\n" +
                                   "   → Request student explanation before accepting";
                }
                else if (allWarnings.Any(w => w.Contains("Mode mismatch") || w.Contains("ONLINE (verified) but time-out is OFFLINE")))
                {
                    recommendation = "\n\n💡 ADMIN RECOMMENDATION:\n" +
                                   "   ⚠️ CAREFULLY REVIEW before accepting\n" +
                                   "   → WiFi disconnect pattern detected\n" +
                                   "   → Verify TickCount data matches claimed duration\n" +
                                   "   → Ask student for valid explanation";
                }
                else if (criticalCount > 0)
                {
                    recommendation = "\n\n💡 ADMIN RECOMMENDATION:\n" +
                                   "   ⚠️ REVIEW REQUIRED\n" +
                                   "   → Critical issues detected\n" +
                                   "   → Verify all details before accepting";
                }

                string warningText = $"\n\n{header}\n" +
                                   new string('─', 60) + "\n" +
                                   string.Join("\n", allWarnings) +
                                   recommendation;

                lblNotesValue.Text = (_scanHistory.Notes ?? "") + warningText;

                // Enhanced color coding based on severity
                if (allWarnings.Any(w => w.Contains("CONFIRMED TIME TAMPERING")))
                {
                    lblNotesValue.ForeColor = Color.DarkRed;
                    lblNotesValue.BackColor = Color.MistyRose;
                }
                else if (allWarnings.Any(w => w.Contains("CRITICAL")))
                {
                    lblNotesValue.ForeColor = Color.Red;
                }
                else if (warningCount > 0)
                {
                    lblNotesValue.ForeColor = Color.DarkOrange;
                }
                else
                {
                    lblNotesValue.ForeColor = Color.DarkBlue;
                }
            }

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

        /// <summary>
        /// Helper method to format duration in human-readable format
        /// </summary>
        private string FormatDuration(double minutes)
        {
            if (minutes < 60)
            {
                return $"{minutes:F0} minutes";
            }
            else
            {
                int hours = (int)(minutes / 60);
                int mins = (int)(minutes % 60);
                return $"{hours}h {mins}m";
            }
        }
    }
}
