namespace ITP104_FINAL_PROJECT
{
    partial class CameraScannerForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblTitle = new System.Windows.Forms.Label();
            this.pnlCameraSelection = new Guna.UI2.WinForms.Guna2Panel();
            this.btnStopCamera = new Guna.UI2.WinForms.Guna2Button();
            this.btnStartCamera = new Guna.UI2.WinForms.Guna2Button();
            this.cmbCameraDevices = new System.Windows.Forms.ComboBox();
            this.lblCamera = new System.Windows.Forms.Label();
            this.pnlCameraPreview = new Guna.UI2.WinForms.Guna2Panel();
            this.pictureBoxCamera = new System.Windows.Forms.PictureBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.pnlScanControl = new Guna.UI2.WinForms.Guna2Panel();
            this.lblScanFeedback = new System.Windows.Forms.Label();
            this.btnScan = new Guna.UI2.WinForms.Guna2Button();
            this.txtStudentId = new Guna.UI2.WinForms.Guna2TextBox();
            this.lblInstruction = new System.Windows.Forms.Label();
            this.lblScanTitle = new System.Windows.Forms.Label();
            this.pnlCameraSelection.SuspendLayout();
            this.pnlCameraPreview.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCamera)).BeginInit();
            this.pnlScanControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblTitle.Location = new System.Drawing.Point(30, 20);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(235, 37);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "📷 Camera Scanner";
            // 
            // pnlCameraSelection
            // 
            this.pnlCameraSelection.BorderRadius = 10;
            this.pnlCameraSelection.Controls.Add(this.btnStopCamera);
            this.pnlCameraSelection.Controls.Add(this.btnStartCamera);
            this.pnlCameraSelection.Controls.Add(this.cmbCameraDevices);
            this.pnlCameraSelection.Controls.Add(this.lblCamera);
            this.pnlCameraSelection.FillColor = System.Drawing.Color.White;
            this.pnlCameraSelection.Location = new System.Drawing.Point(30, 70);
            this.pnlCameraSelection.Name = "pnlCameraSelection";
            this.pnlCameraSelection.ShadowDecoration.Depth = 10;
            this.pnlCameraSelection.ShadowDecoration.Enabled = true;
            this.pnlCameraSelection.Size = new System.Drawing.Size(840, 60);
            this.pnlCameraSelection.TabIndex = 1;
            // 
            // btnStopCamera
            // 
            this.btnStopCamera.BorderRadius = 8;
            this.btnStopCamera.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnStopCamera.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnStopCamera.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnStopCamera.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnStopCamera.Enabled = false;
            this.btnStopCamera.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(53)))), ((int)(((byte)(69)))));
            this.btnStopCamera.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.btnStopCamera.ForeColor = System.Drawing.Color.White;
            this.btnStopCamera.Location = new System.Drawing.Point(700, 10);
            this.btnStopCamera.Name = "btnStopCamera";
            this.btnStopCamera.Size = new System.Drawing.Size(130, 40);
            this.btnStopCamera.TabIndex = 3;
            this.btnStopCamera.Text = "⏹ Stop Camera";
            this.btnStopCamera.Click += new System.EventHandler(this.BtnStopCamera_Click);
            // 
            // btnStartCamera
            // 
            this.btnStartCamera.BorderRadius = 8;
            this.btnStartCamera.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnStartCamera.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnStartCamera.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnStartCamera.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnStartCamera.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(175)))), ((int)(((byte)(80)))));
            this.btnStartCamera.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.btnStartCamera.ForeColor = System.Drawing.Color.White;
            this.btnStartCamera.Location = new System.Drawing.Point(560, 10);
            this.btnStartCamera.Name = "btnStartCamera";
            this.btnStartCamera.Size = new System.Drawing.Size(130, 40);
            this.btnStartCamera.TabIndex = 2;
            this.btnStartCamera.Text = "▶ Start Camera";
            this.btnStartCamera.Click += new System.EventHandler(this.BtnStartCamera_Click);
            // 
            // cmbCameraDevices
            // 
            this.cmbCameraDevices.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCameraDevices.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cmbCameraDevices.FormattingEnabled = true;
            this.cmbCameraDevices.Location = new System.Drawing.Point(140, 18);
            this.cmbCameraDevices.Name = "cmbCameraDevices";
            this.cmbCameraDevices.Size = new System.Drawing.Size(400, 25);
            this.cmbCameraDevices.TabIndex = 1;
            // 
            // lblCamera
            // 
            this.lblCamera.AutoSize = true;
            this.lblCamera.Font = new System.Drawing.Font("Segoe UI Semibold", 11F, System.Drawing.FontStyle.Bold);
            this.lblCamera.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblCamera.Location = new System.Drawing.Point(15, 18);
            this.lblCamera.Name = "lblCamera";
            this.lblCamera.Size = new System.Drawing.Size(118, 20);
            this.lblCamera.TabIndex = 0;
            this.lblCamera.Text = "Select Camera:";
            // 
            // pnlCameraPreview
            // 
            this.pnlCameraPreview.BorderRadius = 10;
            this.pnlCameraPreview.Controls.Add(this.pictureBoxCamera);
            this.pnlCameraPreview.FillColor = System.Drawing.Color.Black;
            this.pnlCameraPreview.Location = new System.Drawing.Point(30, 150);
            this.pnlCameraPreview.Name = "pnlCameraPreview";
            this.pnlCameraPreview.ShadowDecoration.Depth = 10;
            this.pnlCameraPreview.ShadowDecoration.Enabled = true;
            this.pnlCameraPreview.Size = new System.Drawing.Size(640, 480);
            this.pnlCameraPreview.TabIndex = 2;
            // 
            // pictureBoxCamera
            // 
            this.pictureBoxCamera.BackColor = System.Drawing.Color.Black;
            this.pictureBoxCamera.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBoxCamera.Location = new System.Drawing.Point(0, 0);
            this.pictureBoxCamera.Name = "pictureBoxCamera";
            this.pictureBoxCamera.Size = new System.Drawing.Size(640, 480);
            this.pictureBoxCamera.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxCamera.TabIndex = 0;
            this.pictureBoxCamera.TabStop = false;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Italic);
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.lblStatus.Location = new System.Drawing.Point(30, 640);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(213, 19);
            this.lblStatus.TabIndex = 3;
            this.lblStatus.Text = "📹 Camera Status: Not Started";
            // 
            // pnlScanControl
            // 
            this.pnlScanControl.BorderRadius = 10;
            this.pnlScanControl.Controls.Add(this.lblScanFeedback);
            this.pnlScanControl.Controls.Add(this.btnScan);
            this.pnlScanControl.Controls.Add(this.txtStudentId);
            this.pnlScanControl.Controls.Add(this.lblInstruction);
            this.pnlScanControl.Controls.Add(this.lblScanTitle);
            this.pnlScanControl.FillColor = System.Drawing.Color.White;
            this.pnlScanControl.Location = new System.Drawing.Point(690, 150);
            this.pnlScanControl.Name = "pnlScanControl";
            this.pnlScanControl.ShadowDecoration.Depth = 10;
            this.pnlScanControl.ShadowDecoration.Enabled = true;
            this.pnlScanControl.Size = new System.Drawing.Size(180, 480);
            this.pnlScanControl.TabIndex = 4;
            // 
            // lblScanFeedback
            // 
            this.lblScanFeedback.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic);
            this.lblScanFeedback.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.lblScanFeedback.Location = new System.Drawing.Point(15, 220);
            this.lblScanFeedback.Name = "lblScanFeedback";
            this.lblScanFeedback.Size = new System.Drawing.Size(150, 200);
            this.lblScanFeedback.TabIndex = 4;
            this.lblScanFeedback.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // btnScan
            // 
            this.btnScan.BorderRadius = 10;
            this.btnScan.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnScan.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnScan.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnScan.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnScan.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(123)))), ((int)(((byte)(255)))));
            this.btnScan.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnScan.ForeColor = System.Drawing.Color.White;
            this.btnScan.Location = new System.Drawing.Point(15, 150);
            this.btnScan.Name = "btnScan";
            this.btnScan.Size = new System.Drawing.Size(150, 50);
            this.btnScan.TabIndex = 3;
            this.btnScan.Text = "🔍 SCAN";
            this.btnScan.Click += new System.EventHandler(this.BtnScan_Click);
            // 
            // txtStudentId
            // 
            this.txtStudentId.BorderRadius = 8;
            this.txtStudentId.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtStudentId.DefaultText = "";
            this.txtStudentId.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtStudentId.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtStudentId.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtStudentId.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtStudentId.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtStudentId.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtStudentId.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtStudentId.Location = new System.Drawing.Point(15, 90);
            this.txtStudentId.Name = "txtStudentId";
            this.txtStudentId.PasswordChar = '\0';
            this.txtStudentId.PlaceholderText = "e.g., 2021-0001";
            this.txtStudentId.SelectedText = "";
            this.txtStudentId.Size = new System.Drawing.Size(150, 40);
            this.txtStudentId.TabIndex = 2;
            // 
            // lblInstruction
            // 
            this.lblInstruction.AutoSize = true;
            this.lblInstruction.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblInstruction.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.lblInstruction.Location = new System.Drawing.Point(15, 60);
            this.lblInstruction.Name = "lblInstruction";
            this.lblInstruction.Size = new System.Drawing.Size(105, 15);
            this.lblInstruction.TabIndex = 1;
            this.lblInstruction.Text = "Enter Student ID:";
            // 
            // lblScanTitle
            // 
            this.lblScanTitle.AutoSize = true;
            this.lblScanTitle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblScanTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblScanTitle.Location = new System.Drawing.Point(15, 20);
            this.lblScanTitle.Name = "lblScanTitle";
            this.lblScanTitle.Size = new System.Drawing.Size(110, 21);
            this.lblScanTitle.TabIndex = 0;
            this.lblScanTitle.Text = "Manual Scan";
            // 
            // CameraScannerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(242)))), ((int)(((byte)(245)))));
            this.ClientSize = new System.Drawing.Size(900, 700);
            this.Controls.Add(this.pnlScanControl);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.pnlCameraPreview);
            this.Controls.Add(this.pnlCameraSelection);
            this.Controls.Add(this.lblTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "CameraScannerForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "QR Code Scanner - Camera View";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CameraScannerForm_FormClosing);
            this.pnlCameraSelection.ResumeLayout(false);
            this.pnlCameraSelection.PerformLayout();
            this.pnlCameraPreview.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCamera)).EndInit();
            this.pnlScanControl.ResumeLayout(false);
            this.pnlScanControl.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private Guna.UI2.WinForms.Guna2Panel pnlCameraSelection;
        private Guna.UI2.WinForms.Guna2Button btnStopCamera;
        private Guna.UI2.WinForms.Guna2Button btnStartCamera;
        private System.Windows.Forms.ComboBox cmbCameraDevices;
        private System.Windows.Forms.Label lblCamera;
        private Guna.UI2.WinForms.Guna2Panel pnlCameraPreview;
        private System.Windows.Forms.PictureBox pictureBoxCamera;
        private System.Windows.Forms.Label lblStatus;
        private Guna.UI2.WinForms.Guna2Panel pnlScanControl;
        private System.Windows.Forms.Label lblScanFeedback;
        private Guna.UI2.WinForms.Guna2Button btnScan;
        private Guna.UI2.WinForms.Guna2TextBox txtStudentId;
        private System.Windows.Forms.Label lblInstruction;
        private System.Windows.Forms.Label lblScanTitle;
    }
}
