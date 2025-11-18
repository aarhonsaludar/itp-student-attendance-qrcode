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
            this.components = new System.ComponentModel.Container();
            this.lblTitle = new System.Windows.Forms.Label();
            this.pnlCameraSelection = new Guna.UI2.WinForms.Guna2Panel();
            this.btnStopCamera = new Guna.UI2.WinForms.Guna2Button();
            this.btnStartCamera = new Guna.UI2.WinForms.Guna2Button();
            this.cmbCameraDevices = new System.Windows.Forms.ComboBox();
            this.lblCamera = new System.Windows.Forms.Label();
            this.pnlCameraPreview = new Guna.UI2.WinForms.Guna2Panel();
            this.lblStatus = new System.Windows.Forms.Label();
            this.pnlScanControl = new Guna.UI2.WinForms.Guna2Panel();
            this.lblScanFeedback = new System.Windows.Forms.Label();
            this.btnScan = new Guna.UI2.WinForms.Guna2Button();
            this.txtStudentId = new Guna.UI2.WinForms.Guna2TextBox();
            this.lblInstruction = new System.Windows.Forms.Label();
            this.lblScanTitle = new System.Windows.Forms.Label();
            this.guna2BorderlessForm1 = new Guna.UI2.WinForms.Guna2BorderlessForm(this.components);
            this.guna2ControlBox3 = new Guna.UI2.WinForms.Guna2ControlBox();
            this.guna2ControlBox1 = new Guna.UI2.WinForms.Guna2ControlBox();
            this.pnlHeader = new Guna.UI2.WinForms.Guna2Panel();
            this.guna2ControlBox2 = new Guna.UI2.WinForms.Guna2ControlBox();
            this.guna2ControlBox4 = new Guna.UI2.WinForms.Guna2ControlBox();
            this.guna2ControlBox5 = new Guna.UI2.WinForms.Guna2ControlBox();
            this.btnLogout = new Guna.UI2.WinForms.Guna2Button();
            this.lblSystemStatus = new System.Windows.Forms.Label();
            this.lblDatabaseStatus = new System.Windows.Forms.Label();
            this.lblScannerStatus = new System.Windows.Forms.Label();
            this.lblSystemTitle = new System.Windows.Forms.Label();
            this.lblUserName = new System.Windows.Forms.Label();
            this.pictureBoxLogo = new System.Windows.Forms.PictureBox();
            this.pictureBoxCamera = new System.Windows.Forms.PictureBox();
            this.btnExit = new Guna.UI2.WinForms.Guna2Button();
            this.pnlCameraSelection.SuspendLayout();
            this.pnlCameraPreview.SuspendLayout();
            this.pnlScanControl.SuspendLayout();
            this.pnlHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCamera)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblTitle.Location = new System.Drawing.Point(32, 23);
            this.lblTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(333, 46);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "📷 Camera Scanner";
            // 
            // pnlCameraSelection
            // 
            this.pnlCameraSelection.BackColor = System.Drawing.Color.Transparent;
            this.pnlCameraSelection.BorderRadius = 10;
            this.pnlCameraSelection.Controls.Add(this.btnStopCamera);
            this.pnlCameraSelection.Controls.Add(this.btnStartCamera);
            this.pnlCameraSelection.Controls.Add(this.cmbCameraDevices);
            this.pnlCameraSelection.Controls.Add(this.lblCamera);
            this.pnlCameraSelection.FillColor = System.Drawing.Color.White;
            this.pnlCameraSelection.Location = new System.Drawing.Point(40, 112);
            this.pnlCameraSelection.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pnlCameraSelection.Name = "pnlCameraSelection";
            this.pnlCameraSelection.ShadowDecoration.Depth = 10;
            this.pnlCameraSelection.ShadowDecoration.Enabled = true;
            this.pnlCameraSelection.Size = new System.Drawing.Size(1120, 74);
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
            this.btnStopCamera.Font = new System.Drawing.Font("Century Gothic", 10F, System.Drawing.FontStyle.Bold);
            this.btnStopCamera.ForeColor = System.Drawing.Color.White;
            this.btnStopCamera.Location = new System.Drawing.Point(933, 12);
            this.btnStopCamera.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnStopCamera.Name = "btnStopCamera";
            this.btnStopCamera.Size = new System.Drawing.Size(173, 49);
            this.btnStopCamera.TabIndex = 3;
            this.btnStopCamera.Text = "Stop Camera";
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
            this.btnStartCamera.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Bold);
            this.btnStartCamera.ForeColor = System.Drawing.Color.White;
            this.btnStartCamera.Location = new System.Drawing.Point(747, 12);
            this.btnStartCamera.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnStartCamera.Name = "btnStartCamera";
            this.btnStartCamera.Size = new System.Drawing.Size(173, 49);
            this.btnStartCamera.TabIndex = 2;
            this.btnStartCamera.Text = "Start Camera";
            this.btnStartCamera.Click += new System.EventHandler(this.BtnStartCamera_Click);
            // 
            // cmbCameraDevices
            // 
            this.cmbCameraDevices.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCameraDevices.Font = new System.Drawing.Font("Century Gothic", 10F);
            this.cmbCameraDevices.FormattingEnabled = true;
            this.cmbCameraDevices.Location = new System.Drawing.Point(187, 22);
            this.cmbCameraDevices.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmbCameraDevices.Name = "cmbCameraDevices";
            this.cmbCameraDevices.Size = new System.Drawing.Size(532, 29);
            this.cmbCameraDevices.TabIndex = 1;
            // 
            // lblCamera
            // 
            this.lblCamera.AutoSize = true;
            this.lblCamera.Font = new System.Drawing.Font("Century Gothic", 11F, System.Drawing.FontStyle.Bold);
            this.lblCamera.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblCamera.Location = new System.Drawing.Point(20, 22);
            this.lblCamera.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCamera.Name = "lblCamera";
            this.lblCamera.Size = new System.Drawing.Size(154, 23);
            this.lblCamera.TabIndex = 0;
            this.lblCamera.Text = "Select Camera:";
            // 
            // pnlCameraPreview
            // 
            this.pnlCameraPreview.BackColor = System.Drawing.Color.Transparent;
            this.pnlCameraPreview.BorderRadius = 10;
            this.pnlCameraPreview.Controls.Add(this.pictureBoxCamera);
            this.pnlCameraPreview.FillColor = System.Drawing.Color.Black;
            this.pnlCameraPreview.Location = new System.Drawing.Point(40, 210);
            this.pnlCameraPreview.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pnlCameraPreview.Name = "pnlCameraPreview";
            this.pnlCameraPreview.ShadowDecoration.Depth = 10;
            this.pnlCameraPreview.ShadowDecoration.Enabled = true;
            this.pnlCameraPreview.Size = new System.Drawing.Size(853, 591);
            this.pnlCameraPreview.TabIndex = 2;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Century Gothic", 10F, System.Drawing.FontStyle.Italic);
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.lblStatus.Location = new System.Drawing.Point(40, 805);
            this.lblStatus.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(248, 21);
            this.lblStatus.TabIndex = 3;
            this.lblStatus.Text = "Camera Status: Not Started";
            // 
            // pnlScanControl
            // 
            this.pnlScanControl.BackColor = System.Drawing.Color.Transparent;
            this.pnlScanControl.BorderRadius = 10;
            this.pnlScanControl.Controls.Add(this.lblScanFeedback);
            this.pnlScanControl.Controls.Add(this.btnScan);
            this.pnlScanControl.Controls.Add(this.txtStudentId);
            this.pnlScanControl.Controls.Add(this.lblInstruction);
            this.pnlScanControl.Controls.Add(this.lblScanTitle);
            this.pnlScanControl.FillColor = System.Drawing.Color.White;
            this.pnlScanControl.Location = new System.Drawing.Point(920, 210);
            this.pnlScanControl.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pnlScanControl.Name = "pnlScanControl";
            this.pnlScanControl.ShadowDecoration.Depth = 10;
            this.pnlScanControl.ShadowDecoration.Enabled = true;
            this.pnlScanControl.Size = new System.Drawing.Size(240, 591);
            this.pnlScanControl.TabIndex = 4;
            // 
            // lblScanFeedback
            // 
            this.lblScanFeedback.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic);
            this.lblScanFeedback.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.lblScanFeedback.Location = new System.Drawing.Point(20, 271);
            this.lblScanFeedback.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblScanFeedback.Name = "lblScanFeedback";
            this.lblScanFeedback.Size = new System.Drawing.Size(200, 246);
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
            this.btnScan.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Bold);
            this.btnScan.ForeColor = System.Drawing.Color.White;
            this.btnScan.Location = new System.Drawing.Point(20, 185);
            this.btnScan.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnScan.Name = "btnScan";
            this.btnScan.Size = new System.Drawing.Size(200, 62);
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
            this.txtStudentId.Font = new System.Drawing.Font("Century Gothic", 10F);
            this.txtStudentId.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtStudentId.Location = new System.Drawing.Point(20, 111);
            this.txtStudentId.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.txtStudentId.Name = "txtStudentId";
            this.txtStudentId.PlaceholderText = "e.g., 2300-001";
            this.txtStudentId.SelectedText = "";
            this.txtStudentId.Size = new System.Drawing.Size(200, 49);
            this.txtStudentId.TabIndex = 2;
            // 
            // lblInstruction
            // 
            this.lblInstruction.AutoSize = true;
            this.lblInstruction.Font = new System.Drawing.Font("Century Gothic", 9F);
            this.lblInstruction.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.lblInstruction.Location = new System.Drawing.Point(20, 74);
            this.lblInstruction.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblInstruction.Name = "lblInstruction";
            this.lblInstruction.Size = new System.Drawing.Size(129, 20);
            this.lblInstruction.TabIndex = 1;
            this.lblInstruction.Text = "Enter Student ID:";
            // 
            // lblScanTitle
            // 
            this.lblScanTitle.AutoSize = true;
            this.lblScanTitle.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Bold);
            this.lblScanTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblScanTitle.Location = new System.Drawing.Point(20, 25);
            this.lblScanTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblScanTitle.Name = "lblScanTitle";
            this.lblScanTitle.Size = new System.Drawing.Size(137, 23);
            this.lblScanTitle.TabIndex = 0;
            this.lblScanTitle.Text = "Manual Scan";
            // 
            // guna2BorderlessForm1
            // 
            this.guna2BorderlessForm1.ContainerControl = this;
            this.guna2BorderlessForm1.DockIndicatorTransparencyValue = 0.6D;
            this.guna2BorderlessForm1.TransparentWhileDrag = true;
            // 
            // guna2ControlBox3
            // 
            this.guna2ControlBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.guna2ControlBox3.ControlBoxType = Guna.UI2.WinForms.Enums.ControlBoxType.MinimizeBox;
            this.guna2ControlBox3.FillColor = System.Drawing.Color.Black;
            this.guna2ControlBox3.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.guna2ControlBox3.Location = new System.Drawing.Point(1077, -2);
            this.guna2ControlBox3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.guna2ControlBox3.Name = "guna2ControlBox3";
            this.guna2ControlBox3.Size = new System.Drawing.Size(53, 34);
            this.guna2ControlBox3.TabIndex = 10;
            // 
            // guna2ControlBox1
            // 
            this.guna2ControlBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.guna2ControlBox1.FillColor = System.Drawing.Color.Black;
            this.guna2ControlBox1.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.guna2ControlBox1.Location = new System.Drawing.Point(1148, -2);
            this.guna2ControlBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.guna2ControlBox1.Name = "guna2ControlBox1";
            this.guna2ControlBox1.Size = new System.Drawing.Size(53, 34);
            this.guna2ControlBox1.TabIndex = 9;
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.pnlHeader.Controls.Add(this.btnExit);
            this.pnlHeader.Controls.Add(this.guna2ControlBox2);
            this.pnlHeader.Controls.Add(this.guna2ControlBox4);
            this.pnlHeader.Controls.Add(this.guna2ControlBox5);
            this.pnlHeader.Controls.Add(this.btnLogout);
            this.pnlHeader.Controls.Add(this.lblSystemStatus);
            this.pnlHeader.Controls.Add(this.lblDatabaseStatus);
            this.pnlHeader.Controls.Add(this.lblScannerStatus);
            this.pnlHeader.Controls.Add(this.lblSystemTitle);
            this.pnlHeader.Controls.Add(this.lblUserName);
            this.pnlHeader.Controls.Add(this.pictureBoxLogo);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.pnlHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1200, 97);
            this.pnlHeader.TabIndex = 11;
            // 
            // guna2ControlBox2
            // 
            this.guna2ControlBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.guna2ControlBox2.ControlBoxType = Guna.UI2.WinForms.Enums.ControlBoxType.MinimizeBox;
            this.guna2ControlBox2.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.guna2ControlBox2.IconColor = System.Drawing.Color.White;
            this.guna2ControlBox2.Location = new System.Drawing.Point(2300, 12);
            this.guna2ControlBox2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.guna2ControlBox2.Name = "guna2ControlBox2";
            this.guna2ControlBox2.Size = new System.Drawing.Size(45, 30);
            this.guna2ControlBox2.TabIndex = 12;
            // 
            // guna2ControlBox4
            // 
            this.guna2ControlBox4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.guna2ControlBox4.ControlBoxType = Guna.UI2.WinForms.Enums.ControlBoxType.MaximizeBox;
            this.guna2ControlBox4.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.guna2ControlBox4.IconColor = System.Drawing.Color.White;
            this.guna2ControlBox4.Location = new System.Drawing.Point(2346, 12);
            this.guna2ControlBox4.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.guna2ControlBox4.Name = "guna2ControlBox4";
            this.guna2ControlBox4.Size = new System.Drawing.Size(45, 30);
            this.guna2ControlBox4.TabIndex = 11;
            // 
            // guna2ControlBox5
            // 
            this.guna2ControlBox5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.guna2ControlBox5.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.guna2ControlBox5.HoverState.FillColor = System.Drawing.Color.Red;
            this.guna2ControlBox5.HoverState.IconColor = System.Drawing.Color.White;
            this.guna2ControlBox5.IconColor = System.Drawing.Color.White;
            this.guna2ControlBox5.Location = new System.Drawing.Point(2355, 12);
            this.guna2ControlBox5.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.guna2ControlBox5.Name = "guna2ControlBox5";
            this.guna2ControlBox5.Size = new System.Drawing.Size(45, 30);
            this.guna2ControlBox5.TabIndex = 10;
            // 
            // btnLogout
            // 
            this.btnLogout.BorderColor = System.Drawing.Color.Transparent;
            this.btnLogout.BorderRadius = 6;
            this.btnLogout.BorderThickness = 1;
            this.btnLogout.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnLogout.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnLogout.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnLogout.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnLogout.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(53)))), ((int)(((byte)(69)))));
            this.btnLogout.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnLogout.ForeColor = System.Drawing.Color.White;
            this.btnLogout.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(73)))), ((int)(((byte)(89)))));
            this.btnLogout.Location = new System.Drawing.Point(1404, 31);
            this.btnLogout.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(101, 34);
            this.btnLogout.TabIndex = 14;
            this.btnLogout.Text = "Logout";
            // 
            // lblSystemStatus
            // 
            this.lblSystemStatus.AutoSize = true;
            this.lblSystemStatus.BackColor = System.Drawing.Color.Transparent;
            this.lblSystemStatus.Font = new System.Drawing.Font("Segoe UI Semibold", 9F);
            this.lblSystemStatus.ForeColor = System.Drawing.Color.White;
            this.lblSystemStatus.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblSystemStatus.Location = new System.Drawing.Point(885, 68);
            this.lblSystemStatus.Name = "lblSystemStatus";
            this.lblSystemStatus.Size = new System.Drawing.Size(121, 20);
            this.lblSystemStatus.TabIndex = 15;
            this.lblSystemStatus.Text = "Status Indicators";
            // 
            // lblDatabaseStatus
            // 
            this.lblDatabaseStatus.AutoSize = true;
            this.lblDatabaseStatus.BackColor = System.Drawing.Color.Transparent;
            this.lblDatabaseStatus.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblDatabaseStatus.ForeColor = System.Drawing.Color.Lime;
            this.lblDatabaseStatus.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblDatabaseStatus.Location = new System.Drawing.Point(1011, 68);
            this.lblDatabaseStatus.Name = "lblDatabaseStatus";
            this.lblDatabaseStatus.Size = new System.Drawing.Size(163, 20);
            this.lblDatabaseStatus.TabIndex = 16;
            this.lblDatabaseStatus.Text = "● Database: Connected";
            // 
            // lblScannerStatus
            // 
            this.lblScannerStatus.AutoSize = true;
            this.lblScannerStatus.BackColor = System.Drawing.Color.Transparent;
            this.lblScannerStatus.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblScannerStatus.ForeColor = System.Drawing.Color.Lime;
            this.lblScannerStatus.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblScannerStatus.Location = new System.Drawing.Point(883, 43);
            this.lblScannerStatus.Name = "lblScannerStatus";
            this.lblScannerStatus.Size = new System.Drawing.Size(122, 20);
            this.lblScannerStatus.TabIndex = 17;
            this.lblScannerStatus.Text = "● Scanner: Ready";
            // 
            // lblSystemTitle
            // 
            this.lblSystemTitle.AutoSize = true;
            this.lblSystemTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblSystemTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold);
            this.lblSystemTitle.ForeColor = System.Drawing.Color.White;
            this.lblSystemTitle.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblSystemTitle.Location = new System.Drawing.Point(111, 28);
            this.lblSystemTitle.Name = "lblSystemTitle";
            this.lblSystemTitle.Size = new System.Drawing.Size(387, 31);
            this.lblSystemTitle.TabIndex = 18;
            this.lblSystemTitle.Text = "Student ID Scanning System";
            // 
            // lblUserName
            // 
            this.lblUserName.AutoSize = true;
            this.lblUserName.BackColor = System.Drawing.Color.Transparent;
            this.lblUserName.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblUserName.ForeColor = System.Drawing.Color.LightGray;
            this.lblUserName.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblUserName.Location = new System.Drawing.Point(883, 16);
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.Size = new System.Drawing.Size(103, 23);
            this.lblUserName.TabIndex = 19;
            this.lblUserName.Text = "User: Admin";
            // 
            // pictureBoxLogo
            // 
            this.pictureBoxLogo.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxLogo.BackgroundImage = global::ITP104_FINAL_PROJECT.Properties.Resources.attendancelogo2;
            this.pictureBoxLogo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBoxLogo.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.pictureBoxLogo.Location = new System.Drawing.Point(29, 16);
            this.pictureBoxLogo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pictureBoxLogo.Name = "pictureBoxLogo";
            this.pictureBoxLogo.Size = new System.Drawing.Size(76, 70);
            this.pictureBoxLogo.TabIndex = 0;
            this.pictureBoxLogo.TabStop = false;
            // 
            // pictureBoxCamera
            // 
            this.pictureBoxCamera.BackColor = System.Drawing.Color.Black;
            this.pictureBoxCamera.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBoxCamera.Location = new System.Drawing.Point(0, 0);
            this.pictureBoxCamera.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBoxCamera.Name = "pictureBoxCamera";
            this.pictureBoxCamera.Size = new System.Drawing.Size(853, 591);
            this.pictureBoxCamera.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxCamera.TabIndex = 0;
            this.pictureBoxCamera.TabStop = false;
            this.pictureBoxCamera.Click += new System.EventHandler(this.pictureBoxCamera_Click);
            // 
            // btnExit
            // 
            this.btnExit.BorderRadius = 10;
            this.btnExit.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnExit.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnExit.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnExit.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnExit.FillColor = System.Drawing.Color.Red;
            this.btnExit.Font = new System.Drawing.Font("Century Gothic", 12F);
            this.btnExit.ForeColor = System.Drawing.Color.White;
            this.btnExit.Location = new System.Drawing.Point(1077, 16);
            this.btnExit.Margin = new System.Windows.Forms.Padding(2);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(97, 33);
            this.btnExit.TabIndex = 20;
            this.btnExit.Text = "Exit";
            // 
            // CameraScannerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(242)))), ((int)(((byte)(245)))));
            this.ClientSize = new System.Drawing.Size(1200, 862);
            this.Controls.Add(this.pnlHeader);
            this.Controls.Add(this.guna2ControlBox3);
            this.Controls.Add(this.guna2ControlBox1);
            this.Controls.Add(this.pnlScanControl);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.pnlCameraPreview);
            this.Controls.Add(this.pnlCameraSelection);
            this.Controls.Add(this.lblTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.Name = "CameraScannerForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "QR Code Scanner - Camera View";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CameraScannerForm_FormClosing);
            this.pnlCameraSelection.ResumeLayout(false);
            this.pnlCameraSelection.PerformLayout();
            this.pnlCameraPreview.ResumeLayout(false);
            this.pnlScanControl.ResumeLayout(false);
            this.pnlScanControl.PerformLayout();
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCamera)).EndInit();
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
        private System.Windows.Forms.Label lblStatus;
        private Guna.UI2.WinForms.Guna2Panel pnlScanControl;
        private System.Windows.Forms.Label lblScanFeedback;
        private Guna.UI2.WinForms.Guna2Button btnScan;
        private Guna.UI2.WinForms.Guna2TextBox txtStudentId;
        private System.Windows.Forms.Label lblInstruction;
        private System.Windows.Forms.Label lblScanTitle;
        private Guna.UI2.WinForms.Guna2BorderlessForm guna2BorderlessForm1;
        private Guna.UI2.WinForms.Guna2ControlBox guna2ControlBox3;
        private Guna.UI2.WinForms.Guna2ControlBox guna2ControlBox1;
        private Guna.UI2.WinForms.Guna2Panel pnlHeader;
        private Guna.UI2.WinForms.Guna2ControlBox guna2ControlBox2;
        private Guna.UI2.WinForms.Guna2ControlBox guna2ControlBox4;
        private Guna.UI2.WinForms.Guna2ControlBox guna2ControlBox5;
        private Guna.UI2.WinForms.Guna2Button btnLogout;
        private System.Windows.Forms.Label lblSystemStatus;
        private System.Windows.Forms.Label lblDatabaseStatus;
        private System.Windows.Forms.Label lblScannerStatus;
        private System.Windows.Forms.Label lblSystemTitle;
        private System.Windows.Forms.Label lblUserName;
        private System.Windows.Forms.PictureBox pictureBoxLogo;
        private System.Windows.Forms.PictureBox pictureBoxCamera;
        private Guna.UI2.WinForms.Guna2Button btnExit;
    }
}
