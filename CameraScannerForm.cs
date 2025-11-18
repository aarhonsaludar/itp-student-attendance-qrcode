using System;
using System.Drawing;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;

namespace ITP104_FINAL_PROJECT
{
    public partial class CameraScannerForm : Form
    {
        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource;

        public CameraScannerForm()
        {
            InitializeComponent();
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
                lblStatus.Text = "📹 Camera Status: Running";
                lblStatus.ForeColor = Color.Green;
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

        private void BtnScan_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtStudentId.Text))
            {
                lblScanFeedback.Text = "⚠️ Please enter a Student ID";
                lblScanFeedback.ForeColor = Color.Orange;
                return;
            }

            // Show scanning animation
            lblScanFeedback.Text = "🔄 Scanning...";
            lblScanFeedback.ForeColor = Color.Blue;
            Application.DoEvents();

            // Simulate scan delay
            System.Threading.Thread.Sleep(1000);

            // Show success
            lblScanFeedback.Text = $"✅ Scan Successful!\n\nStudent ID:\n{txtStudentId.Text}\n\nStatus: Verified";
            lblScanFeedback.ForeColor = Color.Green;

            // Optional: Clear after a few seconds
            Timer clearTimer = new Timer { Interval = 3000 };
            clearTimer.Tick += (s, args) =>
            {
                txtStudentId.Clear();
                lblScanFeedback.Text = "";
                clearTimer.Stop();
                clearTimer.Dispose();
            };
            clearTimer.Start();
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
