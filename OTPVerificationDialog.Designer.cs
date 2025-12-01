namespace ITP104_FINAL_PROJECT
{
    partial class OTPVerificationDialog
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
            this.panelMain = new System.Windows.Forms.Panel();
            this.btnCancel = new Guna.UI2.WinForms.Guna2ControlBox();
            this.btnResend = new System.Windows.Forms.Button();
            this.btnVerify = new System.Windows.Forms.Button();
            this.lblError = new System.Windows.Forms.Label();
            this.lblTimer = new System.Windows.Forms.Label();
            this.txtOTP = new System.Windows.Forms.TextBox();
            this.lblInstruction = new System.Windows.Forms.Label();
            this.lblAttendanceType = new System.Windows.Forms.Label();
            this.lblStudentInfo = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.timerCountdown = new System.Windows.Forms.Timer(this.components);
            this.guna2BorderlessForm1 = new Guna.UI2.WinForms.Guna2BorderlessForm(this.components);
            this.panelMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelMain
            // 
            this.panelMain.BackColor = System.Drawing.Color.White;
            this.panelMain.Controls.Add(this.btnCancel);
            this.panelMain.Controls.Add(this.btnResend);
            this.panelMain.Controls.Add(this.btnVerify);
            this.panelMain.Controls.Add(this.lblError);
            this.panelMain.Controls.Add(this.lblTimer);
            this.panelMain.Controls.Add(this.txtOTP);
            this.panelMain.Controls.Add(this.lblInstruction);
            this.panelMain.Controls.Add(this.lblAttendanceType);
            this.panelMain.Controls.Add(this.lblStudentInfo);
            this.panelMain.Controls.Add(this.lblTitle);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(0, 0);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(500, 433);
            this.panelMain.TabIndex = 0;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.FillColor = System.Drawing.Color.Red;
            this.btnCancel.Font = new System.Drawing.Font("Century Gothic", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.IconColor = System.Drawing.Color.White;
            this.btnCancel.Location = new System.Drawing.Point(447, -2);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(53, 34);
            this.btnCancel.TabIndex = 9;
            // 
            // btnResend
            // 
            this.btnResend.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(193)))), ((int)(((byte)(7)))));
            this.btnResend.FlatAppearance.BorderSize = 0;
            this.btnResend.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnResend.Font = new System.Drawing.Font("Century Gothic", 11F, System.Drawing.FontStyle.Bold);
            this.btnResend.ForeColor = System.Drawing.Color.Black;
            this.btnResend.Location = new System.Drawing.Point(260, 342);
            this.btnResend.Name = "btnResend";
            this.btnResend.Size = new System.Drawing.Size(140, 45);
            this.btnResend.TabIndex = 8;
            this.btnResend.Text = "Resend OTP";
            this.btnResend.UseVisualStyleBackColor = false;
            this.btnResend.Click += new System.EventHandler(this.btnResend_Click);
            // 
            // btnVerify
            // 
            this.btnVerify.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(167)))), ((int)(((byte)(69)))));
            this.btnVerify.FlatAppearance.BorderSize = 0;
            this.btnVerify.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnVerify.Font = new System.Drawing.Font("Century Gothic", 11F, System.Drawing.FontStyle.Bold);
            this.btnVerify.ForeColor = System.Drawing.Color.White;
            this.btnVerify.Location = new System.Drawing.Point(100, 342);
            this.btnVerify.Name = "btnVerify";
            this.btnVerify.Size = new System.Drawing.Size(140, 45);
            this.btnVerify.TabIndex = 7;
            this.btnVerify.Text = "Verify";
            this.btnVerify.UseVisualStyleBackColor = false;
            this.btnVerify.Click += new System.EventHandler(this.btnVerify_Click);
            // 
            // lblError
            // 
            this.lblError.Font = new System.Drawing.Font("Century Gothic", 9F, System.Drawing.FontStyle.Bold);
            this.lblError.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(53)))), ((int)(((byte)(69)))));
            this.lblError.Location = new System.Drawing.Point(20, 290);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(460, 20);
            this.lblError.TabIndex = 6;
            this.lblError.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblError.Visible = false;
            // 
            // lblTimer
            // 
            this.lblTimer.Font = new System.Drawing.Font("Century Gothic", 10F, System.Drawing.FontStyle.Bold);
            this.lblTimer.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(53)))), ((int)(((byte)(69)))));
            this.lblTimer.Location = new System.Drawing.Point(21, 290);
            this.lblTimer.Name = "lblTimer";
            this.lblTimer.Size = new System.Drawing.Size(460, 25);
            this.lblTimer.TabIndex = 5;
            this.lblTimer.Text = "⏱️ Expires in: 5:00";
            this.lblTimer.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtOTP
            // 
            this.txtOTP.Font = new System.Drawing.Font("Courier New", 32F, System.Drawing.FontStyle.Bold);
            this.txtOTP.Location = new System.Drawing.Point(100, 219);
            this.txtOTP.MaxLength = 6;
            this.txtOTP.Name = "txtOTP";
            this.txtOTP.Size = new System.Drawing.Size(300, 68);
            this.txtOTP.TabIndex = 4;
            this.txtOTP.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtOTP.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtOTP_KeyPress);
            // 
            // lblInstruction
            // 
            this.lblInstruction.Font = new System.Drawing.Font("Century Gothic", 10F);
            this.lblInstruction.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(117)))), ((int)(((byte)(125)))));
            this.lblInstruction.Location = new System.Drawing.Point(20, 145);
            this.lblInstruction.Name = "lblInstruction";
            this.lblInstruction.Size = new System.Drawing.Size(460, 61);
            this.lblInstruction.TabIndex = 3;
            this.lblInstruction.Text = "A 6-digit OTP has been sent to your registered email.\r\nPlease enter the code belo" +
    "w to verify.";
            this.lblInstruction.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblAttendanceType
            // 
            this.lblAttendanceType.Font = new System.Drawing.Font("Century Gothic", 14F, System.Drawing.FontStyle.Bold);
            this.lblAttendanceType.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(167)))), ((int)(((byte)(69)))));
            this.lblAttendanceType.Location = new System.Drawing.Point(20, 105);
            this.lblAttendanceType.Name = "lblAttendanceType";
            this.lblAttendanceType.Size = new System.Drawing.Size(460, 30);
            this.lblAttendanceType.TabIndex = 2;
            this.lblAttendanceType.Text = "⏰ TIME IN";
            this.lblAttendanceType.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblStudentInfo
            // 
            this.lblStudentInfo.Font = new System.Drawing.Font("Century Gothic", 11F, System.Drawing.FontStyle.Bold);
            this.lblStudentInfo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(37)))), ((int)(((byte)(41)))));
            this.lblStudentInfo.Location = new System.Drawing.Point(20, 75);
            this.lblStudentInfo.Name = "lblStudentInfo";
            this.lblStudentInfo.Size = new System.Drawing.Size(460, 25);
            this.lblStudentInfo.TabIndex = 1;
            this.lblStudentInfo.Text = "Student: John Doe (2021-12345)";
            this.lblStudentInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTitle
            // 
            this.lblTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(123)))), ((int)(((byte)(255)))));
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblTitle.Font = new System.Drawing.Font("Century Gothic", 18F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(0, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(500, 60);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Email OTP Verification";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblTitle.Click += new System.EventHandler(this.lblTitle_Click);
            // 
            // timerCountdown
            // 
            this.timerCountdown.Interval = 1000;
            this.timerCountdown.Tick += new System.EventHandler(this.timerCountdown_Tick);
            // 
            // guna2BorderlessForm1
            // 
            this.guna2BorderlessForm1.ContainerControl = this;
            this.guna2BorderlessForm1.DockIndicatorTransparencyValue = 0.6D;
            this.guna2BorderlessForm1.TransparentWhileDrag = true;
            // 
            // OTPVerificationDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(500, 433);
            this.Controls.Add(this.panelMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OTPVerificationDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "OTP Verification";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OTPVerificationDialog_FormClosing);
            this.panelMain.ResumeLayout(false);
            this.panelMain.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblStudentInfo;
        private System.Windows.Forms.Label lblAttendanceType;
        private System.Windows.Forms.Label lblInstruction;
        private System.Windows.Forms.TextBox txtOTP;
        private System.Windows.Forms.Label lblTimer;
        private System.Windows.Forms.Label lblError;
        private System.Windows.Forms.Button btnVerify;
        private System.Windows.Forms.Button btnResend;
        private System.Windows.Forms.Timer timerCountdown;
        private Guna.UI2.WinForms.Guna2BorderlessForm guna2BorderlessForm1;
        private Guna.UI2.WinForms.Guna2ControlBox btnCancel;
    }
}

