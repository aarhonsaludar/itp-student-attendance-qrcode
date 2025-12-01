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
            this.components = new System.ComponentModel.Container();
            this.pnlHeader = new Guna.UI2.WinForms.Guna2Panel();
            this.btnClose = new Guna.UI2.WinForms.Guna2Button();
            this.lblSubtitle = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.pnlMain = new Guna.UI2.WinForms.Guna2Panel();
            this.pnlDatabaseConfig = new Guna.UI2.WinForms.Guna2Panel();
            this.btnTestConnection = new Guna.UI2.WinForms.Guna2Button();
            this.txtPort = new Guna.UI2.WinForms.Guna2TextBox();
            this.lblPort = new System.Windows.Forms.Label();
            this.txtServerAddress = new Guna.UI2.WinForms.Guna2TextBox();
            this.lblServerAddress = new System.Windows.Forms.Label();
            this.lblDatabaseNote = new System.Windows.Forms.Label();
            this.lblDatabaseConfig = new System.Windows.Forms.Label();
            this.pnlSystemConfig = new Guna.UI2.WinForms.Guna2Panel();
            this.cmbLanguage = new Guna.UI2.WinForms.Guna2ComboBox();
            this.lblLanguage = new System.Windows.Forms.Label();
            this.lblThemePreview = new System.Windows.Forms.Label();
            this.cmbTheme = new Guna.UI2.WinForms.Guna2ComboBox();
            this.lblTheme = new System.Windows.Forms.Label();
            this.lblLogoutMinutes = new System.Windows.Forms.Label();
            this.numAutoLogout = new Guna.UI2.WinForms.Guna2NumericUpDown();
            this.lblAutoLogout = new System.Windows.Forms.Label();
            this.lblSystemConfig = new System.Windows.Forms.Label();
            this.pnlScannerConfig = new Guna.UI2.WinForms.Guna2Panel();
            this.toggleBeepOnScan = new Guna.UI2.WinForms.Guna2ToggleSwitch();
            this.lblBeepOnScan = new System.Windows.Forms.Label();
            this.lblTimeoutSeconds = new System.Windows.Forms.Label();
            this.numConnectionTimeout = new Guna.UI2.WinForms.Guna2NumericUpDown();
            this.lblConnectionTimeout = new System.Windows.Forms.Label();
            this.toggleQRScanner = new Guna.UI2.WinForms.Guna2ToggleSwitch();
            this.lblQRScanner = new System.Windows.Forms.Label();
            this.lblScannerConfig = new System.Windows.Forms.Label();
            this.pnlActions = new Guna.UI2.WinForms.Guna2Panel();
            this.btnSaveSettings = new Guna.UI2.WinForms.Guna2Button();
            this.btnResetDefaults = new Guna.UI2.WinForms.Guna2Button();
            this.guna2BorderlessForm1 = new Guna.UI2.WinForms.Guna2BorderlessForm(this.components);
            this.pnlHeader.SuspendLayout();
            this.pnlMain.SuspendLayout();
            this.pnlDatabaseConfig.SuspendLayout();
            this.pnlSystemConfig.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numAutoLogout)).BeginInit();
            this.pnlScannerConfig.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numConnectionTimeout)).BeginInit();
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
            this.pnlHeader.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1333, 123);
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
            this.btnClose.Font = new System.Drawing.Font("Century Gothic", 10F, System.Drawing.FontStyle.Bold);
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(1200, 37);
            this.btnClose.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(107, 49);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // lblSubtitle
            // 
            this.lblSubtitle.AutoSize = true;
            this.lblSubtitle.Font = new System.Drawing.Font("Century Gothic", 11F);
            this.lblSubtitle.ForeColor = System.Drawing.Color.White;
            this.lblSubtitle.Location = new System.Drawing.Point(33, 80);
            this.lblSubtitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSubtitle.Name = "lblSubtitle";
            this.lblSubtitle.Size = new System.Drawing.Size(477, 22);
            this.lblSubtitle.TabIndex = 1;
            this.lblSubtitle.Text = "Configure system preferences and scanner settings";
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Century Gothic", 24F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(27, 25);
            this.lblTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(316, 47);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Settings Screen";
            // 
            // pnlMain
            // 
            this.pnlMain.AutoScroll = true;
            this.pnlMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(242)))), ((int)(((byte)(245)))));
            this.pnlMain.Controls.Add(this.pnlDatabaseConfig);
            this.pnlMain.Controls.Add(this.pnlSystemConfig);
            this.pnlMain.Controls.Add(this.pnlScannerConfig);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(0, 123);
            this.pnlMain.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Padding = new System.Windows.Forms.Padding(40, 37, 40, 37);
            this.pnlMain.Size = new System.Drawing.Size(1333, 739);
            this.pnlMain.TabIndex = 1;
            // 
            // pnlDatabaseConfig
            // 
            this.pnlDatabaseConfig.BackColor = System.Drawing.Color.Transparent;
            this.pnlDatabaseConfig.BorderRadius = 15;
            this.pnlDatabaseConfig.Controls.Add(this.btnTestConnection);
            this.pnlDatabaseConfig.Controls.Add(this.txtPort);
            this.pnlDatabaseConfig.Controls.Add(this.lblPort);
            this.pnlDatabaseConfig.Controls.Add(this.txtServerAddress);
            this.pnlDatabaseConfig.Controls.Add(this.lblServerAddress);
            this.pnlDatabaseConfig.Controls.Add(this.lblDatabaseNote);
            this.pnlDatabaseConfig.Controls.Add(this.lblDatabaseConfig);
            this.pnlDatabaseConfig.Location = new System.Drawing.Point(40, 418);
            this.pnlDatabaseConfig.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pnlDatabaseConfig.Name = "pnlDatabaseConfig";
            this.pnlDatabaseConfig.ShadowDecoration.Depth = 10;
            this.pnlDatabaseConfig.ShadowDecoration.Enabled = true;
            this.pnlDatabaseConfig.Size = new System.Drawing.Size(1213, 246);
            this.pnlDatabaseConfig.TabIndex = 2;
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
            this.btnTestConnection.Font = new System.Drawing.Font("Century Gothic", 10F, System.Drawing.FontStyle.Bold);
            this.btnTestConnection.ForeColor = System.Drawing.Color.White;
            this.btnTestConnection.Location = new System.Drawing.Point(240, 178);
            this.btnTestConnection.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnTestConnection.Name = "btnTestConnection";
            this.btnTestConnection.Size = new System.Drawing.Size(240, 49);
            this.btnTestConnection.TabIndex = 6;
            this.btnTestConnection.Text = "Test Connection";
            this.btnTestConnection.Click += new System.EventHandler(this.btnTestConnection_Click);
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
            this.txtPort.Font = new System.Drawing.Font("Century Gothic", 10F);
            this.txtPort.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtPort.Location = new System.Drawing.Point(773, 111);
            this.txtPort.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.txtPort.Name = "txtPort";
            this.txtPort.PlaceholderText = "";
            this.txtPort.SelectedText = "";
            this.txtPort.Size = new System.Drawing.Size(160, 44);
            this.txtPort.TabIndex = 5;
            // 
            // lblPort
            // 
            this.lblPort.AutoSize = true;
            this.lblPort.Font = new System.Drawing.Font("Century Gothic", 11F);
            this.lblPort.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.lblPort.Location = new System.Drawing.Point(693, 117);
            this.lblPort.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(51, 22);
            this.lblPort.TabIndex = 4;
            this.lblPort.Text = "Port:";
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
            this.txtServerAddress.Font = new System.Drawing.Font("Century Gothic", 10F);
            this.txtServerAddress.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtServerAddress.Location = new System.Drawing.Point(240, 111);
            this.txtServerAddress.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.txtServerAddress.Name = "txtServerAddress";
            this.txtServerAddress.PlaceholderText = "";
            this.txtServerAddress.SelectedText = "";
            this.txtServerAddress.Size = new System.Drawing.Size(400, 44);
            this.txtServerAddress.TabIndex = 3;
            // 
            // lblServerAddress
            // 
            this.lblServerAddress.AutoSize = true;
            this.lblServerAddress.Font = new System.Drawing.Font("Century Gothic", 11F);
            this.lblServerAddress.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.lblServerAddress.Location = new System.Drawing.Point(33, 117);
            this.lblServerAddress.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblServerAddress.Name = "lblServerAddress";
            this.lblServerAddress.Size = new System.Drawing.Size(151, 22);
            this.lblServerAddress.TabIndex = 2;
            this.lblServerAddress.Text = "Server Address:";
            // 
            // lblDatabaseNote
            // 
            this.lblDatabaseNote.AutoSize = true;
            this.lblDatabaseNote.Font = new System.Drawing.Font("Century Gothic", 9F, System.Drawing.FontStyle.Italic);
            this.lblDatabaseNote.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(53)))), ((int)(((byte)(69)))));
            this.lblDatabaseNote.Location = new System.Drawing.Point(33, 68);
            this.lblDatabaseNote.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDatabaseNote.Name = "lblDatabaseNote";
            this.lblDatabaseNote.Size = new System.Drawing.Size(538, 18);
            this.lblDatabaseNote.TabIndex = 1;
            this.lblDatabaseNote.Text = "⚠️ Note: Database configuration is currently disabled (Placeholder only)";
            // 
            // lblDatabaseConfig
            // 
            this.lblDatabaseConfig.AutoSize = true;
            this.lblDatabaseConfig.Font = new System.Drawing.Font("Century Gothic", 14F, System.Drawing.FontStyle.Bold);
            this.lblDatabaseConfig.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblDatabaseConfig.Location = new System.Drawing.Point(27, 25);
            this.lblDatabaseConfig.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDatabaseConfig.Name = "lblDatabaseConfig";
            this.lblDatabaseConfig.Size = new System.Drawing.Size(221, 28);
            this.lblDatabaseConfig.TabIndex = 0;
            this.lblDatabaseConfig.Text = "Database Settings";
            // 
            // pnlSystemConfig
            // 
            this.pnlSystemConfig.BackColor = System.Drawing.Color.Transparent;
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
            this.pnlSystemConfig.Location = new System.Drawing.Point(667, 37);
            this.pnlSystemConfig.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pnlSystemConfig.Name = "pnlSystemConfig";
            this.pnlSystemConfig.ShadowDecoration.Depth = 10;
            this.pnlSystemConfig.ShadowDecoration.Enabled = true;
            this.pnlSystemConfig.Size = new System.Drawing.Size(587, 345);
            this.pnlSystemConfig.TabIndex = 1;
            // 
            // cmbLanguage
            // 
            this.cmbLanguage.BackColor = System.Drawing.Color.Transparent;
            this.cmbLanguage.BorderRadius = 8;
            this.cmbLanguage.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLanguage.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.cmbLanguage.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.cmbLanguage.Font = new System.Drawing.Font("Century Gothic", 10F);
            this.cmbLanguage.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(112)))));
            this.cmbLanguage.ItemHeight = 30;
            this.cmbLanguage.Items.AddRange(new object[] {
            "English",
            "Filipino",
            "Spanish",
            "Chinese"});
            this.cmbLanguage.Location = new System.Drawing.Point(333, 252);
            this.cmbLanguage.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmbLanguage.Name = "cmbLanguage";
            this.cmbLanguage.Size = new System.Drawing.Size(212, 36);
            this.cmbLanguage.StartIndex = 0;
            this.cmbLanguage.TabIndex = 8;
            // 
            // lblLanguage
            // 
            this.lblLanguage.AutoSize = true;
            this.lblLanguage.Font = new System.Drawing.Font("Century Gothic", 11F);
            this.lblLanguage.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.lblLanguage.Location = new System.Drawing.Point(33, 258);
            this.lblLanguage.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblLanguage.Name = "lblLanguage";
            this.lblLanguage.Size = new System.Drawing.Size(112, 22);
            this.lblLanguage.TabIndex = 7;
            this.lblLanguage.Text = "Language:";
            // 
            // lblThemePreview
            // 
            this.lblThemePreview.AutoSize = true;
            this.lblThemePreview.Font = new System.Drawing.Font("Century Gothic", 9F, System.Drawing.FontStyle.Italic);
            this.lblThemePreview.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.lblThemePreview.Location = new System.Drawing.Point(336, 203);
            this.lblThemePreview.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblThemePreview.Name = "lblThemePreview";
            this.lblThemePreview.Size = new System.Drawing.Size(165, 18);
            this.lblThemePreview.TabIndex = 6;
            this.lblThemePreview.Text = "Theme preview: Light";
            // 
            // cmbTheme
            // 
            this.cmbTheme.BackColor = System.Drawing.Color.Transparent;
            this.cmbTheme.BorderRadius = 8;
            this.cmbTheme.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbTheme.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTheme.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.cmbTheme.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.cmbTheme.Font = new System.Drawing.Font("Century Gothic", 10F);
            this.cmbTheme.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(112)))));
            this.cmbTheme.ItemHeight = 30;
            this.cmbTheme.Items.AddRange(new object[] {
            "Light",
            "Dark",
            "Auto"});
            this.cmbTheme.Location = new System.Drawing.Point(333, 154);
            this.cmbTheme.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmbTheme.Name = "cmbTheme";
            this.cmbTheme.Size = new System.Drawing.Size(212, 36);
            this.cmbTheme.StartIndex = 0;
            this.cmbTheme.TabIndex = 5;
            this.cmbTheme.SelectedIndexChanged += new System.EventHandler(this.cmbTheme_SelectedIndexChanged);
            // 
            // lblTheme
            // 
            this.lblTheme.AutoSize = true;
            this.lblTheme.Font = new System.Drawing.Font("Century Gothic", 11F);
            this.lblTheme.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.lblTheme.Location = new System.Drawing.Point(33, 160);
            this.lblTheme.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTheme.Name = "lblTheme";
            this.lblTheme.Size = new System.Drawing.Size(163, 22);
            this.lblTheme.TabIndex = 4;
            this.lblTheme.Text = "Theme Selection:";
            // 
            // lblLogoutMinutes
            // 
            this.lblLogoutMinutes.AutoSize = true;
            this.lblLogoutMinutes.Font = new System.Drawing.Font("Century Gothic", 10F);
            this.lblLogoutMinutes.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.lblLogoutMinutes.Location = new System.Drawing.Point(480, 90);
            this.lblLogoutMinutes.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblLogoutMinutes.Name = "lblLogoutMinutes";
            this.lblLogoutMinutes.Size = new System.Drawing.Size(73, 21);
            this.lblLogoutMinutes.TabIndex = 3;
            this.lblLogoutMinutes.Text = "minutes";
            // 
            // numAutoLogout
            // 
            this.numAutoLogout.BackColor = System.Drawing.Color.Transparent;
            this.numAutoLogout.BorderRadius = 8;
            this.numAutoLogout.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.numAutoLogout.Font = new System.Drawing.Font("Century Gothic", 10F);
            this.numAutoLogout.Location = new System.Drawing.Point(333, 80);
            this.numAutoLogout.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
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
            this.numAutoLogout.Size = new System.Drawing.Size(133, 44);
            this.numAutoLogout.TabIndex = 2;
            this.numAutoLogout.Value = new decimal(new int[] {
            15,
            0,
            0,
            0});
            // 
            // lblAutoLogout
            // 
            this.lblAutoLogout.AutoSize = true;
            this.lblAutoLogout.Font = new System.Drawing.Font("Century Gothic", 11F);
            this.lblAutoLogout.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.lblAutoLogout.Location = new System.Drawing.Point(33, 86);
            this.lblAutoLogout.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblAutoLogout.Name = "lblAutoLogout";
            this.lblAutoLogout.Size = new System.Drawing.Size(183, 22);
            this.lblAutoLogout.TabIndex = 1;
            this.lblAutoLogout.Text = "Auto-Logout Timer:";
            // 
            // lblSystemConfig
            // 
            this.lblSystemConfig.AutoSize = true;
            this.lblSystemConfig.Font = new System.Drawing.Font("Century Gothic", 14F, System.Drawing.FontStyle.Bold);
            this.lblSystemConfig.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblSystemConfig.Location = new System.Drawing.Point(27, 25);
            this.lblSystemConfig.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSystemConfig.Name = "lblSystemConfig";
            this.lblSystemConfig.Size = new System.Drawing.Size(258, 28);
            this.lblSystemConfig.TabIndex = 0;
            this.lblSystemConfig.Text = "System Configuration";
            // 
            // pnlScannerConfig
            // 
            this.pnlScannerConfig.BackColor = System.Drawing.Color.Transparent;
            this.pnlScannerConfig.BorderRadius = 15;
            this.pnlScannerConfig.Controls.Add(this.toggleBeepOnScan);
            this.pnlScannerConfig.Controls.Add(this.lblBeepOnScan);
            this.pnlScannerConfig.Controls.Add(this.lblTimeoutSeconds);
            this.pnlScannerConfig.Controls.Add(this.numConnectionTimeout);
            this.pnlScannerConfig.Controls.Add(this.lblConnectionTimeout);
            this.pnlScannerConfig.Controls.Add(this.toggleQRScanner);
            this.pnlScannerConfig.Controls.Add(this.lblQRScanner);
            this.pnlScannerConfig.Controls.Add(this.lblScannerConfig);
            this.pnlScannerConfig.Location = new System.Drawing.Point(40, 37);
            this.pnlScannerConfig.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pnlScannerConfig.Name = "pnlScannerConfig";
            this.pnlScannerConfig.ShadowDecoration.Depth = 10;
            this.pnlScannerConfig.ShadowDecoration.Enabled = true;
            this.pnlScannerConfig.Size = new System.Drawing.Size(587, 345);
            this.pnlScannerConfig.TabIndex = 0;
            // 
            // toggleBeepOnScan
            // 
            this.toggleBeepOnScan.Checked = true;
            this.toggleBeepOnScan.CheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.toggleBeepOnScan.CheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.toggleBeepOnScan.CheckedState.InnerBorderColor = System.Drawing.Color.White;
            this.toggleBeepOnScan.CheckedState.InnerColor = System.Drawing.Color.White;
            this.toggleBeepOnScan.Location = new System.Drawing.Point(480, 240);
            this.toggleBeepOnScan.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.toggleBeepOnScan.Name = "toggleBeepOnScan";
            this.toggleBeepOnScan.Size = new System.Drawing.Size(67, 31);
            this.toggleBeepOnScan.TabIndex = 7;
            this.toggleBeepOnScan.UncheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
            this.toggleBeepOnScan.UncheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
            this.toggleBeepOnScan.UncheckedState.InnerBorderColor = System.Drawing.Color.White;
            this.toggleBeepOnScan.UncheckedState.InnerColor = System.Drawing.Color.White;
            // 
            // lblBeepOnScan
            // 
            this.lblBeepOnScan.AutoSize = true;
            this.lblBeepOnScan.Font = new System.Drawing.Font("Century Gothic", 11F);
            this.lblBeepOnScan.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.lblBeepOnScan.Location = new System.Drawing.Point(33, 240);
            this.lblBeepOnScan.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblBeepOnScan.Name = "lblBeepOnScan";
            this.lblBeepOnScan.Size = new System.Drawing.Size(138, 22);
            this.lblBeepOnScan.TabIndex = 6;
            this.lblBeepOnScan.Text = "Beep on Scan";
            // 
            // lblTimeoutSeconds
            // 
            this.lblTimeoutSeconds.AutoSize = true;
            this.lblTimeoutSeconds.Font = new System.Drawing.Font("Century Gothic", 10F);
            this.lblTimeoutSeconds.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.lblTimeoutSeconds.Location = new System.Drawing.Point(480, 164);
            this.lblTimeoutSeconds.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTimeoutSeconds.Name = "lblTimeoutSeconds";
            this.lblTimeoutSeconds.Size = new System.Drawing.Size(79, 21);
            this.lblTimeoutSeconds.TabIndex = 5;
            this.lblTimeoutSeconds.Text = "seconds";
            // 
            // numConnectionTimeout
            // 
            this.numConnectionTimeout.BackColor = System.Drawing.Color.Transparent;
            this.numConnectionTimeout.BorderRadius = 8;
            this.numConnectionTimeout.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.numConnectionTimeout.Font = new System.Drawing.Font("Century Gothic", 10F);
            this.numConnectionTimeout.Location = new System.Drawing.Point(333, 154);
            this.numConnectionTimeout.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
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
            this.numConnectionTimeout.Size = new System.Drawing.Size(133, 44);
            this.numConnectionTimeout.TabIndex = 4;
            this.numConnectionTimeout.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // lblConnectionTimeout
            // 
            this.lblConnectionTimeout.AutoSize = true;
            this.lblConnectionTimeout.Font = new System.Drawing.Font("Century Gothic", 11F);
            this.lblConnectionTimeout.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.lblConnectionTimeout.Location = new System.Drawing.Point(33, 160);
            this.lblConnectionTimeout.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblConnectionTimeout.Name = "lblConnectionTimeout";
            this.lblConnectionTimeout.Size = new System.Drawing.Size(200, 22);
            this.lblConnectionTimeout.TabIndex = 3;
            this.lblConnectionTimeout.Text = "Connection Timeout:";
            // 
            // toggleQRScanner
            // 
            this.toggleQRScanner.Checked = true;
            this.toggleQRScanner.CheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.toggleQRScanner.CheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.toggleQRScanner.CheckedState.InnerBorderColor = System.Drawing.Color.White;
            this.toggleQRScanner.CheckedState.InnerColor = System.Drawing.Color.White;
            this.toggleQRScanner.Location = new System.Drawing.Point(480, 86);
            this.toggleQRScanner.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.toggleQRScanner.Name = "toggleQRScanner";
            this.toggleQRScanner.Size = new System.Drawing.Size(67, 31);
            this.toggleQRScanner.TabIndex = 2;
            this.toggleQRScanner.UncheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
            this.toggleQRScanner.UncheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
            this.toggleQRScanner.UncheckedState.InnerBorderColor = System.Drawing.Color.White;
            this.toggleQRScanner.UncheckedState.InnerColor = System.Drawing.Color.White;
            // 
            // lblQRScanner
            // 
            this.lblQRScanner.AutoSize = true;
            this.lblQRScanner.Font = new System.Drawing.Font("Century Gothic", 11F);
            this.lblQRScanner.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.lblQRScanner.Location = new System.Drawing.Point(33, 86);
            this.lblQRScanner.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblQRScanner.Name = "lblQRScanner";
            this.lblQRScanner.Size = new System.Drawing.Size(188, 22);
            this.lblQRScanner.TabIndex = 1;
            this.lblQRScanner.Text = "Enable QR Scanner";
            // 
            // lblScannerConfig
            // 
            this.lblScannerConfig.AutoSize = true;
            this.lblScannerConfig.Font = new System.Drawing.Font("Century Gothic", 14F, System.Drawing.FontStyle.Bold);
            this.lblScannerConfig.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblScannerConfig.Location = new System.Drawing.Point(27, 25);
            this.lblScannerConfig.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblScannerConfig.Name = "lblScannerConfig";
            this.lblScannerConfig.Size = new System.Drawing.Size(270, 28);
            this.lblScannerConfig.TabIndex = 0;
            this.lblScannerConfig.Text = "Scanner Configuration";
            // 
            // pnlActions
            // 
            this.pnlActions.BackColor = System.Drawing.Color.White;
            this.pnlActions.Controls.Add(this.btnSaveSettings);
            this.pnlActions.Controls.Add(this.btnResetDefaults);
            this.pnlActions.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlActions.Location = new System.Drawing.Point(0, 862);
            this.pnlActions.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pnlActions.Name = "pnlActions";
            this.pnlActions.ShadowDecoration.Depth = 15;
            this.pnlActions.ShadowDecoration.Enabled = true;
            this.pnlActions.Size = new System.Drawing.Size(1333, 98);
            this.pnlActions.TabIndex = 2;
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
            this.btnSaveSettings.Font = new System.Drawing.Font("Century Gothic", 11F, System.Drawing.FontStyle.Bold);
            this.btnSaveSettings.ForeColor = System.Drawing.Color.White;
            this.btnSaveSettings.Location = new System.Drawing.Point(1147, 25);
            this.btnSaveSettings.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnSaveSettings.Name = "btnSaveSettings";
            this.btnSaveSettings.Size = new System.Drawing.Size(160, 55);
            this.btnSaveSettings.TabIndex = 1;
            this.btnSaveSettings.Text = "Save Settings";
            this.btnSaveSettings.Click += new System.EventHandler(this.btnSaveSettings_Click);
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
            this.btnResetDefaults.Font = new System.Drawing.Font("Century Gothic", 11F, System.Drawing.FontStyle.Bold);
            this.btnResetDefaults.ForeColor = System.Drawing.Color.White;
            this.btnResetDefaults.Location = new System.Drawing.Point(973, 25);
            this.btnResetDefaults.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnResetDefaults.Name = "btnResetDefaults";
            this.btnResetDefaults.Size = new System.Drawing.Size(160, 55);
            this.btnResetDefaults.TabIndex = 0;
            this.btnResetDefaults.Text = "Reset Defaults";
            this.btnResetDefaults.Click += new System.EventHandler(this.btnResetDefaults_Click);
            // 
            // guna2BorderlessForm1
            // 
            this.guna2BorderlessForm1.ContainerControl = this;
            this.guna2BorderlessForm1.DockIndicatorTransparencyValue = 0.6D;
            this.guna2BorderlessForm1.TransparentWhileDrag = true;
            // 
            // SettingsScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1333, 960);
            this.Controls.Add(this.pnlMain);
            this.Controls.Add(this.pnlActions);
            this.Controls.Add(this.pnlHeader);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.Name = "SettingsScreen";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Settings - Student Attendance System";
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.pnlMain.ResumeLayout(false);
            this.pnlDatabaseConfig.ResumeLayout(false);
            this.pnlDatabaseConfig.PerformLayout();
            this.pnlSystemConfig.ResumeLayout(false);
            this.pnlSystemConfig.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numAutoLogout)).EndInit();
            this.pnlScannerConfig.ResumeLayout(false);
            this.pnlScannerConfig.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numConnectionTimeout)).EndInit();
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
        private Guna.UI2.WinForms.Guna2BorderlessForm guna2BorderlessForm1;
    }
}

