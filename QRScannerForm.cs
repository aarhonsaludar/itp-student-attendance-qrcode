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
            catch (Exception ex)
            {
                // Handle any errors silently to prevent frame processing from stopping
                System.Diagnostics.Debug.WriteLine($"Frame processing error: {ex.Message}");
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
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"QR scan error: {ex.Message}");
            }
        }

        private async Task ProcessQRScanAsync(string qrData)
        {
            try
            {
                // Update UI to show processing
                UpdateUI(() =>
                {
                    lblStatus.Text = "Status: Processing scan...";
                    lblStatus.ForeColor = Color.Orange;
                });

                // Record attendance scan with Time In/Time Out logic
                var (success, message, scanType) = await scanHistoryRepository.RecordAttendanceScanAsync(
                    qrData: qrData,
                    deviceId: DEFAULT_DEVICE_ID,
                    location: DEFAULT_LOCATION
                );

                // Determine result color and status text based on scan type
                Color resultColor;
                string statusText;
                bool playCustomBeep = false;

                if (success)
                {
                    // Success - Time In or Time Out recorded
                    resultColor = Color.Green;
                    playCustomBeep = true; // Play custom beep for successful scans

                    if (scanType == "TIME_IN")
                    {
                        statusText = $"✓ Time In recorded at {DateTime.Now:HH:mm:ss}";
                    }
                    else if (scanType == "TIME_OUT")
                    {
                        statusText = $"✓ Time Out recorded at {DateTime.Now:HH:mm:ss}";
                    }
                    else
                    {
                        statusText = $"✓ Success at {DateTime.Now:HH:mm:ss}";
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
            Application.Exit();
        }
    }
}
