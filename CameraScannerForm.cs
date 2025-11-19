using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;
using AForge.Video;
using AForge.Video.DirectShow;
using ITP104_FINAL_PROJECT.Data;
using ITP104_FINAL_PROJECT.Models;
using ZXing;

namespace ITP104_FINAL_PROJECT
{
    public partial class CameraScannerForm : Form
    {
        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource;
        private readonly StudentRepository studentRepository;
        private readonly ScanHistoryRepository scanHistoryRepository;
        private const int DEFAULT_DEVICE_ID = 1; // QR Scanner 01 from database
        
        // QR Code detection
        private readonly BarcodeReader barcodeReader;
        private bool isProcessingScan = false;
        private DateTime lastScanTime = DateTime.MinValue;
        private const int SCAN_COOLDOWN_MS = 3000; // 3 seconds between scans
        private int frameCounter = 0;

        public CameraScannerForm()
        {
            InitializeComponent();
            studentRepository = new StudentRepository();
            scanHistoryRepository = new ScanHistoryRepository();
            
            // Initialize ZXing barcode reader for QR code detection
            barcodeReader = new BarcodeReader
            {
                AutoRotate = true,
                Options = new ZXing.Common.DecodingOptions
                {
                    TryHarder = true,
                    TryInverted = true,
                    PossibleFormats = new[] { BarcodeFormat.QR_CODE }
                }
            };
            
            InitializeCameraScanner();
        }

        private void InitializeCameraScanner()
        {
            try
            {
                // Get list of video devices
                videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

                if (videoDevices.Count == 0)
                {
                    lblStatus.Text = "❌ No camera devices found!";
                    lblStatus.ForeColor = Color.Red;
                    btnStartCamera.Enabled = false;
                    return;
                }

                // Populate camera combo box
                foreach (FilterInfo device in videoDevices)
                {
                    cmbCameraDevices.Items.Add(device.Name);
                }

                // Select first camera by default
                if (cmbCameraDevices.Items.Count > 0)
                {
                    cmbCameraDevices.SelectedIndex = 0;
                }

                lblStatus.Text = $"✅ Found {videoDevices.Count} camera(s). Ready to start.";
                lblStatus.ForeColor = Color.Green;
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"❌ Error initializing cameras: {ex.Message}";
                lblStatus.ForeColor = Color.Red;
                btnStartCamera.Enabled = false;
            }
        }

        private void BtnStartCamera_Click(object sender, EventArgs e)
        {
            if (cmbCameraDevices.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a camera device first.", "No Camera Selected",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Create video source
                videoSource = new VideoCaptureDevice(videoDevices[cmbCameraDevices.SelectedIndex].MonikerString);

                // Set up event handler for new frames
                videoSource.NewFrame += VideoSource_NewFrame;

                // Start the video source
                videoSource.Start();

                // Update UI
                lblStatus.Text = "📹 Camera: Running | 🔍 Scanning for QR Codes...";
                lblStatus.ForeColor = Color.Green;
                lblScanFeedback.Text = "🔍 Ready to scan QR codes\n\nHold QR code in front of camera...";
                lblScanFeedback.ForeColor = Color.Blue;
                btnStartCamera.Enabled = false;
                btnStopCamera.Enabled = true;
                cmbCameraDevices.Enabled = false;
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"❌ Error starting camera: {ex.Message}";
                lblStatus.ForeColor = Color.Red;
                MessageBox.Show($"Failed to start camera: {ex.Message}", "Camera Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            }

            // Clear the picture box
            if (pictureBoxCamera.Image != null)
            {
                pictureBoxCamera.Image.Dispose();
                pictureBoxCamera.Image = null;
            }

            // Update UI
            lblStatus.Text = "📹 Camera Status: Stopped";
            lblStatus.ForeColor = Color.Gray;
            lblScanFeedback.Text = "Camera stopped. Click Start Camera to begin scanning.";
            lblScanFeedback.ForeColor = Color.Gray;
            btnStartCamera.Enabled = true;
            btnStopCamera.Enabled = false;
            cmbCameraDevices.Enabled = true;
        }

        private void VideoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                // Clone the frame to avoid threading issues
                Bitmap frame = (Bitmap)eventArgs.Frame.Clone();
                
                // Increment frame counter for visual feedback
                frameCounter++;

                // Try to detect QR code in the frame (if not currently processing)
                if (!isProcessingScan && (DateTime.Now - lastScanTime).TotalMilliseconds > SCAN_COOLDOWN_MS)
                {
                    // Update scanning indicator every 10 frames
                    if (frameCounter % 10 == 0)
                    {
                        this.BeginInvoke(new Action(() =>
                        {
                            if (lblScanFeedback.Text.Contains("Ready to scan"))
                            {
                                string dots = new string('.', (frameCounter / 10) % 4);
                                lblScanFeedback.Text = $"🔍 Scanning for QR codes{dots}\n\nHold QR code steady in camera view";
                                lblScanFeedback.ForeColor = Color.Blue;
                            }
                        }));
                    }
                    
                    var result = barcodeReader.Decode(frame);
                    if (result != null && !string.IsNullOrEmpty(result.Text))
                    {
                        // QR Code detected!
                        lastScanTime = DateTime.Now;
                        isProcessingScan = true;
                        
                        // Draw green border around detected area
                        using (Graphics g = Graphics.FromImage(frame))
                        {
                            using (Pen pen = new Pen(Color.LimeGreen, 5))
                            {
                                g.DrawRectangle(pen, 10, 10, frame.Width - 20, frame.Height - 20);
                            }
                        }
                        
                        // Process on UI thread
                        this.BeginInvoke(new Action(async () =>
                        {
                            await ProcessDetectedQRCode(result.Text);
                            isProcessingScan = false;
                        }));
                    }
                }
                else if (isProcessingScan)
                {
                    // Show processing indicator
                    using (Graphics g = Graphics.FromImage(frame))
                    {
                        using (Pen pen = new Pen(Color.Orange, 5))
                        {
                            g.DrawRectangle(pen, 10, 10, frame.Width - 20, frame.Height - 20);
                        }
                    }
                }

                // Update picture box on UI thread
                if (pictureBoxCamera.InvokeRequired)
                {
                    pictureBoxCamera.Invoke(new Action(() =>
                    {
                        // Dispose previous image
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
            }
            catch (Exception ex)
            {
                // Handle any errors during frame processing
                Console.WriteLine($"Error processing frame: {ex.Message}");
            }
        }

        private async Task ProcessDetectedQRCode(string qrData)
        {
            try
            {
                // Show detecting animation
                lblScanFeedback.Text = "📷 QR Code Detected! Processing...";
                lblScanFeedback.ForeColor = Color.Blue;
                txtStudentId.Text = qrData;
                Application.DoEvents();

                // Process the QR code scan
                await ProcessQRScanAsync(qrData);
            }
            catch (Exception ex)
            {
                lblScanFeedback.Text = $"❌ Error: {ex.Message}";
                lblScanFeedback.ForeColor = Color.Red;
            }
        }

        private async void BtnScan_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtStudentId.Text))
            {
                lblScanFeedback.Text = "⚠️ Please enter a Student ID or QR Code";
                lblScanFeedback.ForeColor = Color.Orange;
                return;
            }

            // Disable button during processing
            btnScan.Enabled = false;

            // Show scanning animation
            lblScanFeedback.Text = "🔄 Processing scan...";
            lblScanFeedback.ForeColor = Color.Blue;
            Application.DoEvents();

            try
            {
                // Process the QR code scan
                string qrData = txtStudentId.Text.Trim();
                await ProcessQRScanAsync(qrData);
            }
            catch (Exception ex)
            {
                lblScanFeedback.Text = $"❌ Error: {ex.Message}";
                lblScanFeedback.ForeColor = Color.Red;
            }
            finally
            {
                btnScan.Enabled = true;
            }
        }

        private async Task ProcessQRScanAsync(string qrData)
        {
            try
            {
                // Format: STUDENT-{studentNumber}
                string studentNumber = qrData;
                if (qrData.StartsWith("STUDENT-"))
                {
                    studentNumber = qrData.Substring(8); // Remove "STUDENT-" prefix
                }

                // Look up student by QR code data
                var students = await studentRepository.SearchAsync(studentNumber);
                Student student = null;

                // Find exact match by QR code or student number
                foreach (var s in students)
                {
                    if (s.QRCodeData == qrData || s.StudentNumber == studentNumber)
                    {
                        student = s;
                        break;
                    }
                }

                if (student == null)
                {
                    lblScanFeedback.Text = $"❌ Student Not Found\n\nQR Code: {qrData}\n\nPlease register this student first.";
                    lblScanFeedback.ForeColor = Color.Red;
                    return;
                }

                // Check if student is active
                if (student.Status.ToLower() != "active")
                {
                    lblScanFeedback.Text = $"⚠️ Student Inactive\n\n{student.FullName}\n{student.StudentNumber}\n\nPlease contact administration.";
                    lblScanFeedback.ForeColor = Color.Orange;
                    return;
                }

                // Record the scan in database
                var result = await scanHistoryRepository.RecordScanAsync(
                    studentId: student.StudentId,
                    deviceId: DEFAULT_DEVICE_ID,
                    scanData: qrData,
                    scanPurpose: "attendance",
                    location: "Main Entrance",
                    notes: null
                );

                // Display result
                if (result.success)
                {
                    if (result.message.Contains("duplicate") || result.message.Contains("Duplicate"))
                    {
                        lblScanFeedback.Text = $"⚠️ Duplicate Scan\n\n{student.FullName}\n{student.StudentNumber}\n\nAlready scanned within 5 minutes.";
                        lblScanFeedback.ForeColor = Color.Orange;
                    }
                    else
                    {
                        lblScanFeedback.Text = $"✅ Scan Successful!\n\n{student.FullName}\n{student.StudentNumber}\n{student.Program} - {student.YearLevel}\n\nTime: {DateTime.Now:hh:mm tt}";
                        lblScanFeedback.ForeColor = Color.Green;
                        
                        // Play beep sound (optional)
                        System.Media.SystemSounds.Beep.Play();
                    }
                }
                else
                {
                    lblScanFeedback.Text = $"❌ Scan Failed\n\n{result.message}";
                    lblScanFeedback.ForeColor = Color.Red;
                }

                // Clear after 5 seconds
                Timer clearTimer = new Timer { Interval = 5000 };
                clearTimer.Tick += (s, args) =>
                {
                    txtStudentId.Clear();
                    lblScanFeedback.Text = "Ready to scan...";
                    lblScanFeedback.ForeColor = Color.Gray;
                    clearTimer.Stop();
                    clearTimer.Dispose();
                };
                clearTimer.Start();
            }
            catch (Exception ex)
            {
                lblScanFeedback.Text = $"❌ Error Processing Scan\n\n{ex.Message}";
                lblScanFeedback.ForeColor = Color.Red;
            }
        }

        private void CameraScannerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopCamera();
        }

        private void pictureBoxCamera_Click(object sender, EventArgs e)
        {

        }
    }
}
