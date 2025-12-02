using System;
using System.Drawing;
using System.Media;
using System.Windows.Forms;
using ITP104_FINAL_PROJECT.Models;
using ITP104_FINAL_PROJECT.Services;

namespace ITP104_FINAL_PROJECT
{
    public partial class OTPVerificationDialog : Form
    {
        private OTPSession session;
        private int remainingSeconds;

        public bool IsVerified { get; private set; }
        public OTPSession VerifiedSession { get; private set; }

        public OTPVerificationDialog(OTPSession otpSession)
        {
            InitializeComponent();
            session = otpSession;
            IsVerified = false;
            InitializeDialog();
        }

        private void InitializeDialog()
        {
            // Set student info
            lblStudentInfo.Text = $"Student: {session.StudentName} ({session.StudentNumber})";

            // Set attendance type with color
            if (session.AttendanceType == AttendanceType.TimeIn)
            {
                lblAttendanceType.Text = "⏰ TIME IN";
                lblAttendanceType.ForeColor = Color.FromArgb(40, 167, 69); // Green
                lblTitle.BackColor = Color.FromArgb(40, 167, 69);
            }
            else if (session.AttendanceType == AttendanceType.EmailChange)
            {
                lblAttendanceType.Text = "📧 EMAIL VERIFICATION";
                lblAttendanceType.ForeColor = Color.FromArgb(33, 150, 243); // Blue
                lblTitle.BackColor = Color.FromArgb(33, 150, 243);
            }
            else
            {
                lblAttendanceType.Text = "⏰ TIME OUT";
                lblAttendanceType.ForeColor = Color.FromArgb(255, 152, 0); // Orange
                lblTitle.BackColor = Color.FromArgb(255, 152, 0);
            }

            // Calculate remaining time
            TimeSpan remaining = session.ExpiresAt - DateTime.Now;
            remainingSeconds = Math.Max(0, (int)remaining.TotalSeconds);

            // Start countdown timer
            timerCountdown.Start();
            UpdateTimerDisplay();

            // Focus on OTP input
            txtOTP.Focus();
        }

        private void UpdateTimerDisplay()
        {
            int minutes = remainingSeconds / 60;
            int seconds = remainingSeconds % 60;
            lblTimer.Text = $"⏱️ Expires in: {minutes}:{seconds:D2}";

            // Change color based on remaining time
            if (remainingSeconds <= 30)
            {
                lblTimer.ForeColor = Color.FromArgb(220, 53, 69); // Red
            }
            else if (remainingSeconds <= 60)
            {
                lblTimer.ForeColor = Color.FromArgb(255, 193, 7); // Yellow
            }
            else
            {
                lblTimer.ForeColor = Color.FromArgb(40, 167, 69); // Green
            }
        }

        private void timerCountdown_Tick(object sender, EventArgs e)
        {
            remainingSeconds--;
            UpdateTimerDisplay();

            if (remainingSeconds <= 0)
            {
                timerCountdown.Stop();
                ShowError("OTP expired. Please scan QR code again.");
                btnVerify.Enabled = false;
                btnResend.Enabled = false;
                SystemSounds.Hand.Play();
            }
        }

        private async void btnVerify_Click(object sender, EventArgs e)
        {
            string enteredOTP = txtOTP.Text.Trim();

            // Validate input
            if (string.IsNullOrWhiteSpace(enteredOTP))
            {
                ShowError("Please enter the OTP code.");
                txtOTP.Focus();
                return;
            }

            if (enteredOTP.Length != 6)
            {
                ShowError("OTP must be 6 digits.");
                txtOTP.Focus();
                return;
            }

            // Disable buttons during verification
            btnVerify.Enabled = false;
            btnResend.Enabled = false;
            btnCancel.Enabled = false;
            txtOTP.Enabled = false;
            lblError.Visible = false;

            try
            {
                // Show verifying status
                lblInstruction.Text = "Verifying OTP...";
                lblInstruction.ForeColor = Color.FromArgb(255, 193, 7);

                // Verify OTP
                var (success, message, verifiedSession) = OTPService.VerifyOTP(session.SessionId, enteredOTP);

                if (success)
                {
                    // Verification successful
                    IsVerified = true;
                    VerifiedSession = verifiedSession;
                    timerCountdown.Stop();

                    lblInstruction.Text = "✓ OTP Verified Successfully!";
                    lblInstruction.ForeColor = Color.FromArgb(40, 167, 69);
                    lblTimer.Text = "✓ Verified";
                    lblTimer.ForeColor = Color.FromArgb(40, 167, 69);

                    SystemSounds.Asterisk.Play();

                    // Close dialog after short delay
                    await System.Threading.Tasks.Task.Delay(500);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    // Verification failed
                    ShowError(message);
                    txtOTP.Clear();
                    txtOTP.Focus();
                    SystemSounds.Hand.Play();

                    // Re-enable controls
                    btnVerify.Enabled = true;
                    btnResend.Enabled = true;
                    btnCancel.Enabled = true;
                    txtOTP.Enabled = true;
                    lblInstruction.Text = "A 6-digit OTP has been sent to your registered email.\r\nPlease enter the code below to verify.";
                    lblInstruction.ForeColor = Color.FromArgb(108, 117, 125);
                }
            }
            catch (Exception ex)
            {
                ShowError($"Verification error: {ex.Message}");
                SystemSounds.Hand.Play();

                // Re-enable controls
                btnVerify.Enabled = true;
                btnResend.Enabled = true;
                btnCancel.Enabled = true;
                txtOTP.Enabled = true;
            }
        }

        private async void btnResend_Click(object sender, EventArgs e)
        {
            btnResend.Enabled = false;
            btnVerify.Enabled = false;
            lblError.Visible = false;

            try
            {
                lblInstruction.Text = "Resending OTP...";
                lblInstruction.ForeColor = Color.FromArgb(255, 193, 7);

                var (success, message) = await OTPService.ResendOTPAsync(session.SessionId);

                if (success)
                {
                    lblInstruction.Text = "✓ OTP resent successfully! Check your email.";
                    lblInstruction.ForeColor = Color.FromArgb(40, 167, 69);
                    SystemSounds.Asterisk.Play();

                    // Reset timer to 5 minutes
                    remainingSeconds = 300;
                    UpdateTimerDisplay();

                    await System.Threading.Tasks.Task.Delay(2000);
                    lblInstruction.Text = "A 6-digit OTP has been sent to your registered email.\r\nPlease enter the code below to verify.";
                    lblInstruction.ForeColor = Color.FromArgb(108, 117, 125);
                }
                else
                {
                    ShowError(message);
                    SystemSounds.Hand.Play();
                }

                txtOTP.Clear();
                txtOTP.Focus();
                btnResend.Enabled = true;
                btnVerify.Enabled = true;
            }
            catch (Exception ex)
            {
                ShowError($"Failed to resend OTP: {ex.Message}");
                SystemSounds.Hand.Play();
                btnResend.Enabled = true;
                btnVerify.Enabled = true;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            timerCountdown.Stop();
            OTPService.RemoveSession(session.SessionId);
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void ShowError(string message)
        {
            lblError.Text = "⚠️ " + message;
            lblError.Visible = true;
        }

        private void txtOTP_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Only allow digits
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
                return;
            }

            // Submit on Enter key
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                btnVerify_Click(sender, e);
            }
        }

        private void OTPVerificationDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            timerCountdown.Stop();

            // Clean up session if not verified
            if (!IsVerified)
            {
                OTPService.RemoveSession(session.SessionId);
            }
        }

        private void lblTitle_Click(object sender, EventArgs e)
        {

        }
    }
}
