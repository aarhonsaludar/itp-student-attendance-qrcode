namespace ITP104_FINAL_PROJECT
{
    partial class SettingsScreen
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
            this.pnlHeader = new Guna.UI2.WinForms.Guna2Panel();
            this.btnClose = new Guna.UI2.WinForms.Guna2Button();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblSubtitle = new System.Windows.Forms.Label();
            this.pnlMain = new Guna.UI2.WinForms.Guna2Panel();
            this.pnlScannerConfig = new Guna.UI2.WinForms.Guna2Panel();
            this.lblScannerConfig = new System.Windows.Forms.Label();
            this.lblQRScanner = new System.Windows.Forms.Label();
            this.toggleQRScanner = new Guna.UI2.WinForms.Guna2ToggleSwitch();
            this.lblConnectionTimeout = new System.Windows.Forms.Label();
            this.numConnectionTimeout = new Guna.UI2.WinForms.Guna2NumericUpDown();
            this.lblTimeoutSeconds = new System.Windows.Forms.Label();
            this.lblBeepOnScan = new System.Windows.Forms.Label();
            this.toggleBeepOnScan = new Guna.UI2.WinForms.Guna2ToggleSwitch();
            this.pnlSystemConfig = new Guna.UI2.WinForms.Guna2Panel();
            this.lblSystemConfig = new System.Windows.Forms.Label();
            this.lblAutoLogout = new System.Windows.Forms.Label();
            this.numAutoLogout = new Guna.UI2.WinForms.Guna2NumericUpDown();
            this.lblLogoutMinutes = new System.Windows.Forms.Label();
            this.lblTheme = new System.Windows.Forms.Label();
            this.cmbTheme = new Guna.UI2.WinForms.Guna2ComboBox();
            this.lblThemePreview = new System.Windows.Forms.Label();
            this.lblLanguage = new System.Windows.Forms.Label();
            this.cmbLanguage = new Guna.UI2.WinForms.Guna2ComboBox();
            this.pnlDatabaseConfig = new Guna.UI2.WinForms.Guna2Panel();
            this.lblDatabaseConfig = new System.Windows.Forms.Label();
            this.lblDatabaseNote = new System.Windows.Forms.Label();
            this.lblServerAddress = new System.Windows.Forms.Label();
            this.txtServerAddress = new Guna.UI2.WinForms.Guna2TextBox();
            this.lblPort = new System.Windows.Forms.Label();
            this.txtPort = new Guna.UI2.WinForms.Guna2TextBox();
            this.btnTestConnection = new Guna.UI2.WinForms.Guna2Button();
            this.pnlActions = new Guna.UI2.WinForms.Guna2Panel();
            this.btnResetDefaults = new Guna.UI2.WinForms.Guna2Button();
            this.btnSaveSettings = new Guna.UI2.WinForms.Guna2Button();
            this.pnlHeader.SuspendLayout();
            this.pnlMain.SuspendLayout();
            this.pnlScannerConfig.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numConnectionTimeout)).BeginInit();
            this.pnlSystemConfig.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numAutoLogout)).BeginInit();
            this.pnlDatabaseConfig.SuspendLayout();
            this.pnlActions.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(128)))), ((int)(((byte)(185)))));
            this.pnlHeader.Controls.Add(this.btnClose);
            this.pnlHeader.Controls.Add(this.lblSubtitle);
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1000, 100);
            this.pnlHeader.TabIndex = 0;
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.BorderRadius = 8;
            this.btnClose.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnClose.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnClose.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnClose.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnClose.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(53)))), ((int)(((byte)(69)))));
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(900, 30);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(80, 40);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "✖ Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(20, 20);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(211, 45);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "⚙️ Settings";
            // 
            // lblSubtitle
            // 
            this.lblSubtitle.AutoSize = true;
            this.lblSubtitle.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblSubtitle.ForeColor = System.Drawing.Color.White;
            this.lblSubtitle.Location = new System.Drawing.Point(25, 65);
            this.lblSubtitle.Name = "lblSubtitle";
            this.lblSubtitle.Size = new System.Drawing.Size(340, 20);
            this.lblSubtitle.TabIndex = 1;
            this.lblSubtitle.Text = "Configure system preferences and scanner settings";
            // 
            // pnlMain
            // 
            this.pnlMain.AutoScroll = true;
            this.pnlMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(242)))), ((int)(((byte)(245)))));
            this.pnlMain.Controls.Add(this.pnlDatabaseConfig);
            this.pnlMain.Controls.Add(this.pnlSystemConfig);
            this.pnlMain.Controls.Add(this.pnlScannerConfig);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(0, 100);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Padding = new System.Windows.Forms.Padding(30);
            this.pnlMain.Size = new System.Drawing.Size(1000, 600);
            this.pnlMain.TabIndex = 1;
            // 
            // pnlScannerConfig
            // 
            this.pnlScannerConfig.BackColor = System.Drawing.Color.White;
            this.pnlScannerConfig.BorderRadius = 15;
            this.pnlScannerConfig.Controls.Add(this.toggleBeepOnScan);
            this.pnlScannerConfig.Controls.Add(this.lblBeepOnScan);
            this.pnlScannerConfig.Controls.Add(this.lblTimeoutSeconds);
            this.pnlScannerConfig.Controls.Add(this.numConnectionTimeout);
            this.pnlScannerConfig.Controls.Add(this.lblConnectionTimeout);
            this.pnlScannerConfig.Controls.Add(this.toggleQRScanner);
            this.pnlScannerConfig.Controls.Add(this.lblQRScanner);
            this.pnlScannerConfig.Controls.Add(this.lblScannerConfig);
            this.pnlScannerConfig.Location = new System.Drawing.Point(30, 30);
            this.pnlScannerConfig.Name = "pnlScannerConfig";
            this.pnlScannerConfig.ShadowDecoration.Depth = 10;
            this.pnlScannerConfig.ShadowDecoration.Enabled = true;
            this.pnlScannerConfig.Size = new System.Drawing.Size(440, 280);
            this.pnlScannerConfig.TabIndex = 0;
            // 
            // lblScannerConfig
            // 
            this.lblScannerConfig.AutoSize = true;
            this.lblScannerConfig.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblScannerConfig.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblScannerConfig.Location = new System.Drawing.Point(20, 20);
            this.lblScannerConfig.Name = "lblScannerConfig";
            this.lblScannerConfig.Size = new System.Drawing.Size(228, 25);
            this.lblScannerConfig.TabIndex = 0;
            this.lblScannerConfig.Text = "📱 Scanner Configuration";
            // 
            // lblQRScanner
            // 
            this.lblQRScanner.AutoSize = true;
            this.lblQRScanner.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblQRScanner.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.lblQRScanner.Location = new System.Drawing.Point(25, 70);
            this.lblQRScanner.Name = "lblQRScanner";
            this.lblQRScanner.Size = new System.Drawing.Size(135, 20);
            this.lblQRScanner.TabIndex = 1;
            this.lblQRScanner.Text = "Enable QR Scanner";
            // 
            // toggleQRScanner
            // 
            this.toggleQRScanner.Checked = true;
            this.toggleQRScanner.CheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.toggleQRScanner.CheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.toggleQRScanner.CheckedState.InnerBorderColor = System.Drawing.Color.White;
            this.toggleQRScanner.CheckedState.InnerColor = System.Drawing.Color.White;
            this.toggleQRScanner.Location = new System.Drawing.Point(360, 70);
            this.toggleQRScanner.Name = "toggleQRScanner";
            this.toggleQRScanner.Size = new System.Drawing.Size(50, 25);
            this.toggleQRScanner.TabIndex = 2;
            this.toggleQRScanner.UncheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
            this.toggleQRScanner.UncheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
            this.toggleQRScanner.UncheckedState.InnerBorderColor = System.Drawing.Color.White;
            this.toggleQRScanner.UncheckedState.InnerColor = System.Drawing.Color.White;
            // 
            // lblConnectionTimeout
            // 
            this.lblConnectionTimeout.AutoSize = true;
            this.lblConnectionTimeout.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblConnectionTimeout.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.lblConnectionTimeout.Location = new System.Drawing.Point(25, 130);
            this.lblConnectionTimeout.Name = "lblConnectionTimeout";
            this.lblConnectionTimeout.Size = new System.Drawing.Size(154, 20);
            this.lblConnectionTimeout.TabIndex = 3;
            this.lblConnectionTimeout.Text = "Connection Timeout:";
            // 
            // numConnectionTimeout
            // 
            this.numConnectionTimeout.BackColor = System.Drawing.Color.Transparent;
            this.numConnectionTimeout.BorderRadius = 8;
            this.numConnectionTimeout.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.numConnectionTimeout.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.numConnectionTimeout.Location = new System.Drawing.Point(250, 125);
            this.numConnectionTimeout.Maximum = new decimal(new int[] {
            120,
            0,
            0,
            0});
            this.numConnectionTimeout.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numConnectionTimeout.Name = "numConnectionTimeout";
            this.numConnectionTimeout.Size = new System.Drawing.Size(100, 36);
            this.numConnectionTimeout.TabIndex = 4;
            this.numConnectionTimeout.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // lblTimeoutSeconds
            // 
            this.lblTimeoutSeconds.AutoSize = true;
            this.lblTimeoutSeconds.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblTimeoutSeconds.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.lblTimeoutSeconds.Location = new System.Drawing.Point(360, 133);
            this.lblTimeoutSeconds.Name = "lblTimeoutSeconds";
            this.lblTimeoutSeconds.Size = new System.Drawing.Size(60, 19);
            this.lblTimeoutSeconds.TabIndex = 5;
            this.lblTimeoutSeconds.Text = "seconds";
            // 
            // lblBeepOnScan
            // 
            this.lblBeepOnScan.AutoSize = true;
            this.lblBeepOnScan.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblBeepOnScan.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.lblBeepOnScan.Location = new System.Drawing.Point(25, 195);
            this.lblBeepOnScan.Name = "lblBeepOnScan";
            this.lblBeepOnScan.Size = new System.Drawing.Size(104, 20);
            this.lblBeepOnScan.TabIndex = 6;
            this.lblBeepOnScan.Text = "Beep on Scan";
            // 
            // toggleBeepOnScan
            // 
            this.toggleBeepOnScan.Checked = true;
            this.toggleBeepOnScan.CheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.toggleBeepOnScan.CheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.toggleBeepOnScan.CheckedState.InnerBorderColor = System.Drawing.Color.White;
            this.toggleBeepOnScan.CheckedState.InnerColor = System.Drawing.Color.White;
            this.toggleBeepOnScan.Location = new System.Drawing.Point(360, 195);
            this.toggleBeepOnScan.Name = "toggleBeepOnScan";
            this.toggleBeepOnScan.Size = new System.Drawing.Size(50, 25);
            this.toggleBeepOnScan.TabIndex = 7;
            this.toggleBeepOnScan.UncheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
            this.toggleBeepOnScan.UncheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
            this.toggleBeepOnScan.UncheckedState.InnerBorderColor = System.Drawing.Color.White;
            this.toggleBeepOnScan.UncheckedState.InnerColor = System.Drawing.Color.White;
            // 
            // pnlSystemConfig
            // 
            this.pnlSystemConfig.BackColor = System.Drawing.Color.White;
            this.pnlSystemConfig.BorderRadius = 15;
            this.pnlSystemConfig.Controls.Add(this.cmbLanguage);
            this.pnlSystemConfig.Controls.Add(this.lblLanguage);
            this.pnlSystemConfig.Controls.Add(this.lblThemePreview);
            this.pnlSystemConfig.Controls.Add(this.cmbTheme);
            this.pnlSystemConfig.Controls.Add(this.lblTheme);
            this.pnlSystemConfig.Controls.Add(this.lblLogoutMinutes);
            this.pnlSystemConfig.Controls.Add(this.numAutoLogout);
            this.pnlSystemConfig.Controls.Add(this.lblAutoLogout);
            this.pnlSystemConfig.Controls.Add(this.lblSystemConfig);
            this.pnlSystemConfig.Location = new System.Drawing.Point(500, 30);
            this.pnlSystemConfig.Name = "pnlSystemConfig";
            this.pnlSystemConfig.ShadowDecoration.Depth = 10;
            this.pnlSystemConfig.ShadowDecoration.Enabled = true;
            this.pnlSystemConfig.Size = new System.Drawing.Size(440, 280);
            this.pnlSystemConfig.TabIndex = 1;
            // 
            // lblSystemConfig
            // 
            this.lblSystemConfig.AutoSize = true;
            this.lblSystemConfig.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblSystemConfig.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblSystemConfig.Location = new System.Drawing.Point(20, 20);
            this.lblSystemConfig.Name = "lblSystemConfig";
            this.lblSystemConfig.Size = new System.Drawing.Size(226, 25);
            this.lblSystemConfig.TabIndex = 0;
            this.lblSystemConfig.Text = "🖥️ System Configuration";
            // 
            // lblAutoLogout
            // 
            this.lblAutoLogout.AutoSize = true;
            this.lblAutoLogout.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblAutoLogout.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.lblAutoLogout.Location = new System.Drawing.Point(25, 70);
            this.lblAutoLogout.Name = "lblAutoLogout";
            this.lblAutoLogout.Size = new System.Drawing.Size(133, 20);
            this.lblAutoLogout.TabIndex = 1;
            this.lblAutoLogout.Text = "Auto-Logout Timer:";
            // 
            // numAutoLogout
            // 
            this.numAutoLogout.BackColor = System.Drawing.Color.Transparent;
            this.numAutoLogout.BorderRadius = 8;
            this.numAutoLogout.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.numAutoLogout.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.numAutoLogout.Location = new System.Drawing.Point(250, 65);
            this.numAutoLogout.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.numAutoLogout.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numAutoLogout.Name = "numAutoLogout";
            this.numAutoLogout.Size = new System.Drawing.Size(100, 36);
            this.numAutoLogout.TabIndex = 2;
            this.numAutoLogout.Value = new decimal(new int[] {
            15,
            0,
            0,
            0});
            // 
            // lblLogoutMinutes
            // 
            this.lblLogoutMinutes.AutoSize = true;
            this.lblLogoutMinutes.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblLogoutMinutes.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.lblLogoutMinutes.Location = new System.Drawing.Point(360, 73);
            this.lblLogoutMinutes.Name = "lblLogoutMinutes";
            this.lblLogoutMinutes.Size = new System.Drawing.Size(59, 19);
            this.lblLogoutMinutes.TabIndex = 3;
            this.lblLogoutMinutes.Text = "minutes";
            // 
            // lblTheme
            // 
            this.lblTheme.AutoSize = true;
            this.lblTheme.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblTheme.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.lblTheme.Location = new System.Drawing.Point(25, 130);
            this.lblTheme.Name = "lblTheme";
            this.lblTheme.Size = new System.Drawing.Size(122, 20);
            this.lblTheme.TabIndex = 4;
            this.lblTheme.Text = "Theme Selection:";
            // 
            // cmbTheme
            // 
            this.cmbTheme.BackColor = System.Drawing.Color.Transparent;
            this.cmbTheme.BorderRadius = 8;
            this.cmbTheme.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbTheme.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTheme.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.cmbTheme.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.cmbTheme.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cmbTheme.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(112)))));
            this.cmbTheme.ItemHeight = 30;
            this.cmbTheme.Items.AddRange(new object[] {
            "Light",
            "Dark",
            "Auto"});
            this.cmbTheme.Location = new System.Drawing.Point(250, 125);
            this.cmbTheme.Name = "cmbTheme";
            this.cmbTheme.Size = new System.Drawing.Size(160, 36);
            this.cmbTheme.StartIndex = 0;
            this.cmbTheme.TabIndex = 5;
            this.cmbTheme.SelectedIndexChanged += new System.EventHandler(this.cmbTheme_SelectedIndexChanged);
            // 
            // lblThemePreview
            // 
            this.lblThemePreview.AutoSize = true;
            this.lblThemePreview.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic);
            this.lblThemePreview.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.lblThemePreview.Location = new System.Drawing.Point(252, 165);
            this.lblThemePreview.Name = "lblThemePreview";
            this.lblThemePreview.Size = new System.Drawing.Size(124, 15);
            this.lblThemePreview.TabIndex = 6;
            this.lblThemePreview.Text = "Theme preview: Light";
            // 
            // lblLanguage
            // 
            this.lblLanguage.AutoSize = true;
            this.lblLanguage.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblLanguage.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.lblLanguage.Location = new System.Drawing.Point(25, 210);
            this.lblLanguage.Name = "lblLanguage";
            this.lblLanguage.Size = new System.Drawing.Size(77, 20);
            this.lblLanguage.TabIndex = 7;
            this.lblLanguage.Text = "Language:";
            // 
            // cmbLanguage
            // 
            this.cmbLanguage.BackColor = System.Drawing.Color.Transparent;
            this.cmbLanguage.BorderRadius = 8;
            this.cmbLanguage.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLanguage.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.cmbLanguage.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.cmbLanguage.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cmbLanguage.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(112)))));
            this.cmbLanguage.ItemHeight = 30;
            this.cmbLanguage.Items.AddRange(new object[] {
            "English",
            "Filipino",
            "Spanish",
            "Chinese"});
            this.cmbLanguage.Location = new System.Drawing.Point(250, 205);
            this.cmbLanguage.Name = "cmbLanguage";
            this.cmbLanguage.Size = new System.Drawing.Size(160, 36);
            this.cmbLanguage.StartIndex = 0;
            this.cmbLanguage.TabIndex = 8;
            // 
            // pnlDatabaseConfig
            // 
            this.pnlDatabaseConfig.BackColor = System.Drawing.Color.White;
            this.pnlDatabaseConfig.BorderRadius = 15;
            this.pnlDatabaseConfig.Controls.Add(this.btnTestConnection);
            this.pnlDatabaseConfig.Controls.Add(this.txtPort);
            this.pnlDatabaseConfig.Controls.Add(this.lblPort);
            this.pnlDatabaseConfig.Controls.Add(this.txtServerAddress);
            this.pnlDatabaseConfig.Controls.Add(this.lblServerAddress);
            this.pnlDatabaseConfig.Controls.Add(this.lblDatabaseNote);
            this.pnlDatabaseConfig.Controls.Add(this.lblDatabaseConfig);
            this.pnlDatabaseConfig.Location = new System.Drawing.Point(30, 340);
            this.pnlDatabaseConfig.Name = "pnlDatabaseConfig";
            this.pnlDatabaseConfig.ShadowDecoration.Depth = 10;
            this.pnlDatabaseConfig.ShadowDecoration.Enabled = true;
            this.pnlDatabaseConfig.Size = new System.Drawing.Size(910, 200);
            this.pnlDatabaseConfig.TabIndex = 2;
            // 
            // lblDatabaseConfig
            // 
            this.lblDatabaseConfig.AutoSize = true;
            this.lblDatabaseConfig.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblDatabaseConfig.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblDatabaseConfig.Location = new System.Drawing.Point(20, 20);
            this.lblDatabaseConfig.Name = "lblDatabaseConfig";
            this.lblDatabaseConfig.Size = new System.Drawing.Size(198, 25);
            this.lblDatabaseConfig.TabIndex = 0;
            this.lblDatabaseConfig.Text = "🗄️ Database Settings";
            // 
            // lblDatabaseNote
            // 
            this.lblDatabaseNote.AutoSize = true;
            this.lblDatabaseNote.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic);
            this.lblDatabaseNote.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(53)))), ((int)(((byte)(69)))));
            this.lblDatabaseNote.Location = new System.Drawing.Point(25, 55);
            this.lblDatabaseNote.Name = "lblDatabaseNote";
            this.lblDatabaseNote.Size = new System.Drawing.Size(389, 15);
            this.lblDatabaseNote.TabIndex = 1;
            this.lblDatabaseNote.Text = "⚠️ Note: Database configuration is currently disabled (Placeholder only)";
            // 
            // lblServerAddress
            // 
            this.lblServerAddress.AutoSize = true;
            this.lblServerAddress.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblServerAddress.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.lblServerAddress.Location = new System.Drawing.Point(25, 95);
            this.lblServerAddress.Name = "lblServerAddress";
            this.lblServerAddress.Size = new System.Drawing.Size(116, 20);
            this.lblServerAddress.TabIndex = 2;
            this.lblServerAddress.Text = "Server Address:";
            // 
            // txtServerAddress
            // 
            this.txtServerAddress.BorderRadius = 8;
            this.txtServerAddress.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtServerAddress.DefaultText = "localhost";
            this.txtServerAddress.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtServerAddress.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtServerAddress.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtServerAddress.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtServerAddress.Enabled = false;
            this.txtServerAddress.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtServerAddress.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtServerAddress.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtServerAddress.Location = new System.Drawing.Point(180, 90);
            this.txtServerAddress.Name = "txtServerAddress";
            this.txtServerAddress.PasswordChar = '\0';
            this.txtServerAddress.PlaceholderText = "";
            this.txtServerAddress.SelectedText = "";
            this.txtServerAddress.Size = new System.Drawing.Size(300, 36);
            this.txtServerAddress.TabIndex = 3;
            // 
            // lblPort
            // 
            this.lblPort.AutoSize = true;
            this.lblPort.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblPort.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.lblPort.Location = new System.Drawing.Point(520, 95);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(40, 20);
            this.lblPort.TabIndex = 4;
            this.lblPort.Text = "Port:";
            // 
            // txtPort
            // 
            this.txtPort.BorderRadius = 8;
            this.txtPort.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtPort.DefaultText = "1433";
            this.txtPort.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtPort.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtPort.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtPort.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtPort.Enabled = false;
            this.txtPort.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtPort.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtPort.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtPort.Location = new System.Drawing.Point(580, 90);
            this.txtPort.Name = "txtPort";
            this.txtPort.PasswordChar = '\0';
            this.txtPort.PlaceholderText = "";
            this.txtPort.SelectedText = "";
            this.txtPort.Size = new System.Drawing.Size(120, 36);
            this.txtPort.TabIndex = 5;
            // 
            // btnTestConnection
            // 
            this.btnTestConnection.BorderRadius = 8;
            this.btnTestConnection.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnTestConnection.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnTestConnection.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnTestConnection.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnTestConnection.Enabled = false;
            this.btnTestConnection.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(117)))), ((int)(((byte)(125)))));
            this.btnTestConnection.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnTestConnection.ForeColor = System.Drawing.Color.White;
            this.btnTestConnection.Location = new System.Drawing.Point(180, 145);
            this.btnTestConnection.Name = "btnTestConnection";
            this.btnTestConnection.Size = new System.Drawing.Size(180, 40);
            this.btnTestConnection.TabIndex = 6;
            this.btnTestConnection.Text = "🔌 Test Connection";
            this.btnTestConnection.Click += new System.EventHandler(this.btnTestConnection_Click);
            // 
            // pnlActions
            // 
            this.pnlActions.BackColor = System.Drawing.Color.White;
            this.pnlActions.Controls.Add(this.btnSaveSettings);
            this.pnlActions.Controls.Add(this.btnResetDefaults);
            this.pnlActions.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlActions.Location = new System.Drawing.Point(0, 700);
            this.pnlActions.Name = "pnlActions";
            this.pnlActions.ShadowDecoration.Depth = 15;
            this.pnlActions.ShadowDecoration.Enabled = true;
            this.pnlActions.Size = new System.Drawing.Size(1000, 80);
            this.pnlActions.TabIndex = 2;
            // 
            // btnResetDefaults
            // 
            this.btnResetDefaults.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnResetDefaults.BorderRadius = 10;
            this.btnResetDefaults.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnResetDefaults.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnResetDefaults.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnResetDefaults.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnResetDefaults.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(117)))), ((int)(((byte)(125)))));
            this.btnResetDefaults.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnResetDefaults.ForeColor = System.Drawing.Color.White;
            this.btnResetDefaults.Location = new System.Drawing.Point(650, 20);
            this.btnResetDefaults.Name = "btnResetDefaults";
            this.btnResetDefaults.Size = new System.Drawing.Size(160, 45);
            this.btnResetDefaults.TabIndex = 0;
            this.btnResetDefaults.Text = "🔄 Reset Defaults";
            this.btnResetDefaults.Click += new System.EventHandler(this.btnResetDefaults_Click);
            // 
            // btnSaveSettings
            // 
            this.btnSaveSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveSettings.BorderRadius = 10;
            this.btnSaveSettings.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnSaveSettings.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnSaveSettings.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnSaveSettings.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnSaveSettings.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(167)))), ((int)(((byte)(69)))));
            this.btnSaveSettings.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnSaveSettings.ForeColor = System.Drawing.Color.White;
            this.btnSaveSettings.Location = new System.Drawing.Point(820, 20);
            this.btnSaveSettings.Name = "btnSaveSettings";
            this.btnSaveSettings.Size = new System.Drawing.Size(160, 45);
            this.btnSaveSettings.TabIndex = 1;
            this.btnSaveSettings.Text = "💾 Save Settings";
            this.btnSaveSettings.Click += new System.EventHandler(this.btnSaveSettings_Click);
            // 
            // SettingsScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1000, 780);
            this.Controls.Add(this.pnlMain);
            this.Controls.Add(this.pnlActions);
            this.Controls.Add(this.pnlHeader);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "SettingsScreen";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Settings - Student Attendance System";
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.pnlMain.ResumeLayout(false);
            this.pnlScannerConfig.ResumeLayout(false);
            this.pnlScannerConfig.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numConnectionTimeout)).EndInit();
            this.pnlSystemConfig.ResumeLayout(false);
            this.pnlSystemConfig.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numAutoLogout)).EndInit();
            this.pnlDatabaseConfig.ResumeLayout(false);
            this.pnlDatabaseConfig.PerformLayout();
            this.pnlActions.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Guna.UI2.WinForms.Guna2Panel pnlHeader;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblSubtitle;
        private Guna.UI2.WinForms.Guna2Button btnClose;
        private Guna.UI2.WinForms.Guna2Panel pnlMain;
        private Guna.UI2.WinForms.Guna2Panel pnlScannerConfig;
        private System.Windows.Forms.Label lblScannerConfig;
        private System.Windows.Forms.Label lblQRScanner;
        private Guna.UI2.WinForms.Guna2ToggleSwitch toggleQRScanner;
        private System.Windows.Forms.Label lblConnectionTimeout;
        private Guna.UI2.WinForms.Guna2NumericUpDown numConnectionTimeout;
        private System.Windows.Forms.Label lblTimeoutSeconds;
        private System.Windows.Forms.Label lblBeepOnScan;
        private Guna.UI2.WinForms.Guna2ToggleSwitch toggleBeepOnScan;
        private Guna.UI2.WinForms.Guna2Panel pnlSystemConfig;
        private System.Windows.Forms.Label lblSystemConfig;
        private System.Windows.Forms.Label lblAutoLogout;
        private Guna.UI2.WinForms.Guna2NumericUpDown numAutoLogout;
        private System.Windows.Forms.Label lblLogoutMinutes;
        private System.Windows.Forms.Label lblTheme;
        private Guna.UI2.WinForms.Guna2ComboBox cmbTheme;
        private System.Windows.Forms.Label lblThemePreview;
        private System.Windows.Forms.Label lblLanguage;
        private Guna.UI2.WinForms.Guna2ComboBox cmbLanguage;
        private Guna.UI2.WinForms.Guna2Panel pnlDatabaseConfig;
        private System.Windows.Forms.Label lblDatabaseConfig;
        private System.Windows.Forms.Label lblDatabaseNote;
        private System.Windows.Forms.Label lblServerAddress;
        private Guna.UI2.WinForms.Guna2TextBox txtServerAddress;
        private System.Windows.Forms.Label lblPort;
        private Guna.UI2.WinForms.Guna2TextBox txtPort;
        private Guna.UI2.WinForms.Guna2Button btnTestConnection;
        private Guna.UI2.WinForms.Guna2Panel pnlActions;
        private Guna.UI2.WinForms.Guna2Button btnResetDefaults;
        private Guna.UI2.WinForms.Guna2Button btnSaveSettings;
    }
}
