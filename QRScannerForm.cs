using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Media;
using AForge.Video;
using AForge.Video.DirectShow;
using ZXing;
using ITP104_FINAL_PROJECT.Data;
using ITP104_FINAL_PROJECT.Models;
using ITP104_FINAL_PROJECT.Services;

namespace ITP104_FINAL_PROJECT
{
    public partial class QRScannerForm : Form
    {
        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource;
        private BarcodeReader barcodeReader;
        private Bitmap currentFrame;

        // Database repositories
        private readonly StudentRepository studentRepository;
        private readonly ScanHistoryRepository scanHistoryRepository;

        // Static property to track scanner state globally
        public static bool IsScannerRunning { get; private set; } = false;
        private const int DEFAULT_DEVICE_ID = 1;
        private const string DEFAULT_LOCATION = "Pamantasan ng Cabuyao Building";

        // Scan box dimensions and position
        private Rectangle scanBox;
        private const int SCAN_BOX_WIDTH = 600;
        private const int SCAN_BOX_HEIGHT = 500;

        // Scan throttling
        private DateTime lastScanTime = DateTime.MinValue;
        private const int SCAN_COOLDOWN_MS = 2000;
        private bool isProcessingScan = false;

        public QRScannerForm()
        {
            InitializeComponent();
            studentRepository = new StudentRepository();
            scanHistoryRepository = new ScanHistoryRepository();
            InitializeScanner();
        }

        private void InitializeScanner()
        {
            // Initialize ZXing barcode reader
            barcodeReader = new BarcodeReader
            {
                AutoRotate = true,
                Options = new ZXing.Common.DecodingOptions
                {
                    TryHarder = true,
                    PossibleFormats = new[] { BarcodeFormat.QR_CODE },
                    TryInverted = true
                }
            };

            // Get available video devices
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            if (videoDevices.Count == 0)
            {
                MessageBox.Show("No camera devices found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Populate camera dropdown
            foreach (FilterInfo device in videoDevices)
            {
                cmbCameras.Items.Add(device.Name);
            }

            if (cmbCameras.Items.Count > 0)
            {
                cmbCameras.SelectedIndex = 0;
            }
        }

        private void BtnStartCamera_Click(object sender, EventArgs e)
        {
            if (cmbCameras.SelectedIndex < 0)
            {
                MessageBox.Show("Please select a camera device.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            StartCamera();
        }

        private void StartCamera()
        {
            if (videoSource != null && videoSource.IsRunning)
            {
                StopCamera();
            }

            try
            {
                videoSource = new VideoCaptureDevice(videoDevices[cmbCameras.SelectedIndex].MonikerString);
                videoSource.NewFrame += VideoSource_NewFrame;
                videoSource.Start();
                IsScannerRunning = true;  // Update global scanner state

                btnStartCamera.Enabled = false;
                btnStopCamera.Enabled = true;
                lblStatus.Text = "Status: Camera running - Position QR code in scan box";
                lblStatus.ForeColor = Color.Green;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting camera: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnStopCamera_Click(object sender, EventArgs e)
        {
            StopCamera();
        }

        private void StopCamera()
        {
            if (videoSource != null && videoSource.IsRunning)
            {
                videoSource.SignalToStop();
                videoSource.WaitForStop();
                videoSource.NewFrame -= VideoSource_NewFrame;
                videoSource = null;
                IsScannerRunning = false;  // Update global scanner state
            }

            btnStartCamera.Enabled = true;
            btnStopCamera.Enabled = false;
            lblStatus.Text = "Status: Camera stopped";
            lblStatus.ForeColor = Color.Gray;
            lblResult.Text = "QR Code: (none)";
            lblResult.ForeColor = Color.Black;
            pictureBoxCamera.Image = null;
        }

        private void VideoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                // Clone the frame
                Bitmap frame = (Bitmap)eventArgs.Frame.Clone();

                // Store current frame for processing
                if (currentFrame != null)
                {
                    currentFrame.Dispose();
                }
                currentFrame = (Bitmap)frame.Clone();

                // Calculate scan box position (centered on the frame)
                int boxX = (frame.Width - SCAN_BOX_WIDTH) / 2;
                int boxY = (frame.Height - SCAN_BOX_HEIGHT) / 2;
                scanBox = new Rectangle(boxX, boxY, SCAN_BOX_WIDTH, SCAN_BOX_HEIGHT);

                // Draw scan box overlay on the frame
                using (Graphics g = Graphics.FromImage(frame))
                {
                    // Draw semi-transparent overlay outside scan box
                    using (SolidBrush overlayBrush = new SolidBrush(Color.FromArgb(100, 0, 0, 0)))
                    {
                        // Top overlay
                        g.FillRectangle(overlayBrush, 0, 0, frame.Width, boxY);
                        // Bottom overlay
                        g.FillRectangle(overlayBrush, 0, boxY + SCAN_BOX_HEIGHT, frame.Width, frame.Height - boxY - SCAN_BOX_HEIGHT);
                        // Left overlay
                        g.FillRectangle(overlayBrush, 0, boxY, boxX, SCAN_BOX_HEIGHT);
                        // Right overlay
                        g.FillRectangle(overlayBrush, boxX + SCAN_BOX_WIDTH, boxY, frame.Width - boxX - SCAN_BOX_WIDTH, SCAN_BOX_HEIGHT);
                    }

                    // Draw scan box border
                    Pen borderPen = isProcessingScan ? new Pen(Color.Orange, 4) : new Pen(Color.Lime, 4);
                    g.DrawRectangle(borderPen, scanBox);
                    borderPen.Dispose();

                    // Draw corner brackets
                    using (Pen cornerPen = new Pen(Color.White, 6))
                    {
                        int cornerLength = 30;

                        // Top-left corner
                        g.DrawLine(cornerPen, boxX, boxY, boxX + cornerLength, boxY);
                        g.DrawLine(cornerPen, boxX, boxY, boxX, boxY + cornerLength);

                        // Top-right corner
                        g.DrawLine(cornerPen, boxX + SCAN_BOX_WIDTH - cornerLength, boxY, boxX + SCAN_BOX_WIDTH, boxY);
                        g.DrawLine(cornerPen, boxX + SCAN_BOX_WIDTH, boxY, boxX + SCAN_BOX_WIDTH, boxY + cornerLength);

                        // Bottom-left corner
                        g.DrawLine(cornerPen, boxX, boxY + SCAN_BOX_HEIGHT - cornerLength, boxX, boxY + SCAN_BOX_HEIGHT);
                        g.DrawLine(cornerPen, boxX, boxY + SCAN_BOX_HEIGHT, boxX + cornerLength, boxY + SCAN_BOX_HEIGHT);

                        // Bottom-right corner
                        g.DrawLine(cornerPen, boxX + SCAN_BOX_WIDTH - cornerLength, boxY + SCAN_BOX_HEIGHT, boxX + SCAN_BOX_WIDTH, boxY + SCAN_BOX_HEIGHT);
                        g.DrawLine(cornerPen, boxX + SCAN_BOX_WIDTH, boxY + SCAN_BOX_HEIGHT - cornerLength, boxX + SCAN_BOX_WIDTH, boxY + SCAN_BOX_HEIGHT);
                    }

                    // Draw scan instruction text
                    string instruction = "Position QR code inside the box";
                    using (Font font = new Font("Segoe UI", 14, FontStyle.Bold))
                    using (SolidBrush textBrush = new SolidBrush(Color.White))
                    {
                        SizeF textSize = g.MeasureString(instruction, font);
                        float textX = (frame.Width - textSize.Width) / 2;
                        float textY = boxY - textSize.Height - 10;

                        // Draw text shadow
                        g.DrawString(instruction, font, Brushes.Black, textX + 2, textY + 2);
                        g.DrawString(instruction, font, textBrush, textX, textY);
                    }
                }

                // Display frame in PictureBox
                if (pictureBoxCamera.InvokeRequired)
                {
                    pictureBoxCamera.Invoke(new Action(() =>
                    {
                        if (pictureBoxCamera.Image != null)
                        {
                            pictureBoxCamera.Image.Dispose();
                        }
                        pictureBoxCamera.Image = frame;
                    }));
                }
                else
                {
                    if (pictureBoxCamera.Image != null)
                    {
                        pictureBoxCamera.Image.Dispose();
                    }
                    pictureBoxCamera.Image = frame;
                }

                // Scan for QR codes within the scan box only
                ScanQRCodeInBox();
            }
            catch (Exception)
            {
                // frame processing encountered an error but continues to next frame
            }
        }

        private void ScanQRCodeInBox()
        {
            // Check scan cooldown
            if (isProcessingScan || (DateTime.Now - lastScanTime).TotalMilliseconds < SCAN_COOLDOWN_MS)
            {
                return;
            }

            if (currentFrame == null || scanBox.IsEmpty)
            {
                return;
            }

            try
            {
                // Crop the frame to the scan box area only
                Bitmap croppedFrame = CropImage(currentFrame, scanBox);

                // Decode QR code from cropped image
                var result = barcodeReader.Decode(croppedFrame);

                croppedFrame.Dispose();

                if (result != null && !string.IsNullOrEmpty(result.Text))
                {
                    isProcessingScan = true;
                    lastScanTime = DateTime.Now;

                    // Process the QR code with database integration
                    _ = ProcessQRScanAsync(result.Text);
                }
            }
            catch (Exception)
            {
                // qr code detection failed for current frame, will retry on next scan
            }
        }

        private async Task ProcessQRScanAsync(string qrData)
        {
            try
            {
                // Update UI to show processing
                UpdateUI(() =>
                {
                    lblStatus.Text = "Status: Verifying student...";
                    lblStatus.ForeColor = Color.Orange;
                });

                // ===================================================
                // STEP 1: Get student information from QR code
                // ===================================================
                var student = await studentRepository.GetByQRCodeAsync(qrData);
                
                if (student == null)
                {
                    UpdateUI(() =>
                    {
                        lblResult.Text = "Student not found";
                        lblResult.ForeColor = Color.Red;
                        lblStatus.Text = "✗ Scan failed";
                        lblStatus.ForeColor = Color.Red;
                    });
                    SystemSounds.Hand.Play();
                    isProcessingScan = false;
                    return;
                }

                // ===================================================
                // STEP 2: Determine attendance type (Time In or Time Out)
                // ===================================================
                var attendanceType = await DetermineAttendanceTypeAsync(student.StudentId);

                // ===================================================
                // STEP 3: Try to send OTP via email (skip if offline)
                // ===================================================
                OTPSession otpSession = null;
                bool isOfflineMode = false;
                
                try
                {
                    UpdateUI(() =>
                    {
                        lblStatus.Text = "Status: Sending OTP to email...";
                        lblStatus.ForeColor = Color.Blue;
                    });

                    otpSession = await OTPService.InitiateAttendanceAsync(student, attendanceType, qrData);
                }
                catch (Exception ex)
                {
                    // Check if it's a network/connection error (offline mode)
                    bool isNetworkError = ex.Message.Contains("No such host is known") ||
                                         ex.Message.Contains("Unable to connect") ||
                                         ex.Message.Contains("network") ||
                                         ex.Message.Contains("connection") ||
                                         ex.GetType().Name.Contains("Socket") ||
                                         ex.GetType().Name.Contains("Http");

                    if (isNetworkError)
                    {
                        // OFFLINE MODE: Skip OTP verification
                        isOfflineMode = true;
                        
                        DialogResult offlineResult = DialogResult.No;
                        
                        UpdateUI(() =>
                        {
                            offlineResult = MessageBox.Show(
                                $"⚠️ OFFLINE MODE DETECTED\n\n" +
                                $"Cannot send OTP email - No internet connection.\n\n" +
                                $"Do you want to record attendance in OFFLINE MODE?\n\n" +
                                $"Note: This attendance will be flagged for manual review.",
                                "Offline Mode - Skip OTP Verification",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Warning
                            );
                        });

                        if (offlineResult != DialogResult.Yes)
                        {
                            UpdateUI(() =>
                            {
                                lblResult.Text = "Offline attendance cancelled";
                                lblResult.ForeColor = Color.Orange;
                                lblStatus.Text = "⚠ Cancelled";
                                lblStatus.ForeColor = Color.Orange;
                            });
                            SystemSounds.Exclamation.Play();
                            isProcessingScan = false;
                            return;
                        }

                        // User chose to proceed with offline mode
                        UpdateUI(() =>
                        {
                            lblStatus.Text = "Status: Recording offline attendance...";
                            lblStatus.ForeColor = Color.Orange;
                        });
                    }
                    else
                    {
                        // Other error (not network related) - show error and exit
                        UpdateUI(() =>
                        {
                            MessageBox.Show(
                                $"Failed to send OTP:\n\n{ex.Message}\n\nPlease ensure the student has a valid email address registered.",
                                "OTP Send Failed",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error
                            );
                            lblResult.Text = "Failed to send OTP";
                            lblResult.ForeColor = Color.Red;
                            lblStatus.Text = "✗ OTP send failed";
                            lblStatus.ForeColor = Color.Red;
                        });
                        SystemSounds.Hand.Play();
                        isProcessingScan = false;
                        return;
                    }
                }

                // ===================================================
                // STEP 4: Show OTP verification dialog (skip if offline)
                // ===================================================
                bool otpVerified = false;
                
                if (!isOfflineMode)
                {
                    UpdateUI(() =>
                    {
                        using (var otpDialog = new OTPVerificationDialog(otpSession))
                        {
                            var dialogResult = otpDialog.ShowDialog();
                            otpVerified = (dialogResult == DialogResult.OK && otpDialog.IsVerified);
                        }
                    });

                    if (!otpVerified)
                    {
                        UpdateUI(() =>
                        {
                            lblResult.Text = "OTP verification cancelled";
                            lblResult.ForeColor = Color.Orange;
                            lblStatus.Text = "⚠ Verification cancelled";
                            lblStatus.ForeColor = Color.Orange;
                        });
                        SystemSounds.Exclamation.Play();
                        isProcessingScan = false;
                        return;
                    }
                }
                else
                {
                    // Offline mode - OTP verification skipped
                    otpVerified = true; // Allow attendance recording
                }

                // ===================================================
                // STEP 5: OTP verified - Record attendance with database timestamp
                // Client does NOT send any time - database generates it
                // ===================================================
                UpdateUI(() =>
                {
                    lblStatus.Text = "Status: Recording attendance...";
                    lblStatus.ForeColor = Color.Blue;
                });

                var (success, message, scanType, timestamp, timeIn, timeOut) = await scanHistoryRepository.RecordAttendanceScanAsync(
                    qrData: qrData,
                    deviceId: DEFAULT_DEVICE_ID,
                    location: DEFAULT_LOCATION
                );

                // Determine result color and status text based on scan type
                Color resultColor;
                string statusText;
                bool playCustomBeep = false;

                if (scanType == "TIME_TAMPERED")
                {
                    // ===================================================
                    // CRITICAL: Time tampering detected - attendance BLOCKED
                    // ===================================================
                    resultColor = Color.DarkRed;
                    statusText = "🚫 TIME TAMPERING DETECTED - BLOCKED";

                    // Show critical error dialog
                    UpdateUI(() =>
                    {
                        MessageBox.Show(
                            message,
                            "🚫 ATTENDANCE BLOCKED - Time Tampering Detected",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                    });

                    // Play error sound
                    SystemSounds.Hand.Play();
                }
                else if (success)
                {
                    // Success - Time In or Time Out recorded
                    resultColor = Color.Green;
                    playCustomBeep = true; // Play custom beep for successful scans

                    // ===================================================
                    // CRITICAL: Display ONLY database-generated timestamp
                    // NEVER use DateTime.Now from client
                    // ===================================================
                    string dbTime = timestamp.HasValue ? timestamp.Value.ToString("HH:mm:ss") : "Unknown";
                    string dbDate = timestamp.HasValue ? timestamp.Value.ToString("yyyy-MM-dd") : "Unknown";

                    if (scanType == "TIME_IN")
                    {
                        statusText = $"✓ Time In recorded at {dbTime}";
                        // Show success message dialog for Time In with DATABASE timestamp
                        UpdateUI(() =>
                        {
                            MessageBox.Show(
                                $"Time In Successfully Recorded\n\n" +
                                $"Database Server Time: {dbTime}\n" +
                                $"Date: {dbDate}\n\n" +
                                $"Student ID: {qrData}\n\n" +
                                $"⚠️ Timestamp generated by database server (tamper-proof)",
                                "✓ Time In Success",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information
                            );
                        });
                    }
                    else if (scanType == "TIME_OUT")
                    {
                        statusText = $"✓ Time Out recorded at {dbTime}";

                        // Calculate duration if both times available
                        string durationText = "";
                        if (timeIn.HasValue && timeOut.HasValue)
                        {
                            TimeSpan duration = timeOut.Value - timeIn.Value;
                            durationText = $"\nDuration: {duration.Hours}h {duration.Minutes}m";
                        }

                        // Show success message dialog for Time Out with DATABASE timestamp
                        UpdateUI(() =>
                        {
                            MessageBox.Show(
                                $"Time Out Successfully Recorded\n\n" +
                                $"Time In: {(timeIn.HasValue ? timeIn.Value.ToString("HH:mm:ss") : "N/A")}\n" +
                                $"Time Out: {dbTime}\n" +
                                $"Date: {dbDate}{durationText}\n\n" +
                                $"Student ID: {qrData}\n\n" +
                                $"⚠️ Timestamps generated by database server (tamper-proof)",
                                "✓ Time Out Success",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information
                            );
                        });
                    }
                    else
                    {
                        statusText = $"✓ Success at {dbTime}";
                    }
                }
                else if (scanType == "COMPLETED")
                {
                    // Already completed attendance for today
                    resultColor = Color.Orange;
                    statusText = "⚠ Attendance already completed";
                }
                else if (scanType == "DUPLICATE")
                {
                    // Duplicate scan (too soon)
                    resultColor = Color.Orange;
                    statusText = "⚠ Duplicate scan detected";
                }
                else if (scanType == "FOR_REVIEW")
                {
                    // Offline mode - Scan recorded but requires manual review
                    resultColor = Color.Orange;
                    statusText = "⚠ For Review - Offline Mode";
                    playCustomBeep = true; // Play success sound since attendance was recorded

                    // Show offline mode dialog
                    UpdateUI(() =>
                    {
                        MessageBox.Show(
                            message,
                            "⚠ For Review - Offline Attendance",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );
                    });
                }
                else
                {
                    // Error (student not found, inactive, or other error)
                    resultColor = Color.Red;
                    statusText = "✗ Scan failed";
                }

                // Play appropriate sound
                if (playCustomBeep)
                {
                    // Play custom beep.wav from resources for successful Time In/Time Out
                    try
                    {
                        SoundPlayer player = new SoundPlayer(Properties.Resources.beep);
                        player.Play();
                    }
                    catch
                    {
                        // Fallback to system beep if resource not found
                        SystemSounds.Beep.Play();
                    }
                }
                else if (scanType == "COMPLETED" || scanType == "DUPLICATE")
                {
                    // Warning sound for already completed or duplicate
                    SystemSounds.Exclamation.Play();
                }
                else if (scanType == "FOR_REVIEW")
                {
                    // Success sound already played above for FOR_REVIEW
                }
                else
                {
                    // Error sound for failures
                    SystemSounds.Hand.Play();
                }

                // Update UI with final result
                UpdateUI(() =>
                {
                    lblResult.Text = message;
                    lblResult.ForeColor = resultColor;
                    lblStatus.Text = statusText;
                    lblStatus.ForeColor = resultColor;
                });

                // Reset processing flag after delay
                await Task.Delay(500);
                isProcessingScan = false;
            }
            catch (Exception ex)
            {
                // Handle errors
                UpdateUI(() =>
                {
                    lblResult.Text = $"Error: {ex.Message}";
                    lblResult.ForeColor = Color.Red;
                    lblStatus.Text = "✗ Scan failed";
                    lblStatus.ForeColor = Color.Red;
                });

                SystemSounds.Hand.Play();
                isProcessingScan = false;
            }
        }

        private void UpdateUI(Action action)
        {
            if (InvokeRequired)
            {
                Invoke(action);
            }
            else
            {
                action();
            }
        }

        private Bitmap CropImage(Bitmap source, Rectangle cropArea)
        {
            // Ensure crop area is within bounds
            cropArea.Intersect(new Rectangle(0, 0, source.Width, source.Height));

            if (cropArea.Width <= 0 || cropArea.Height <= 0)
            {
                return null;
            }

            // Create cropped bitmap
            Bitmap croppedBitmap = new Bitmap(cropArea.Width, cropArea.Height);

            using (Graphics g = Graphics.FromImage(croppedBitmap))
            {
                g.DrawImage(source,
                    new Rectangle(0, 0, cropArea.Width, cropArea.Height),
                    cropArea,
                    GraphicsUnit.Pixel);
            }

            return croppedBitmap;
        }

        private void QRScannerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopCamera();

            if (currentFrame != null)
            {
                currentFrame.Dispose();
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            lblResult.Text = "QR Code: (none)";
            lblResult.ForeColor = Color.Black;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Determines whether the next scan should be Time In or Time Out
        /// </summary>
        private async Task<AttendanceType> DetermineAttendanceTypeAsync(int studentId)
        {
            try
            {
                // Check if student has an active Time In without Time Out for today
                var hasActiveTimeIn = await scanHistoryRepository.HasActiveTodayTimeInAsync(studentId);
                
                if (hasActiveTimeIn)
                {
                    return AttendanceType.TimeOut;
                }
                else
                {
                    return AttendanceType.TimeIn;
                }
            }
            catch
            {
                // Default to Time In if unable to determine
                return AttendanceType.TimeIn;
            }
        }
    }
}
