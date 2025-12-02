# 📚 Student Attendance QR Code System

A secure, modern Windows desktop application for managing student attendance using QR code technology with multi-layer security verification.

![.NET Framework](https://img.shields.io/badge/.NET%20Framework-4.7.2-purple)
![MySQL](https://img.shields.io/badge/MySQL-5.7%2B-blue)
![Platform](https://img.shields.io/badge/Platform-Windows-lightgrey)
![License](https://img.shields.io/badge/License-MIT-green)

---

## ✨ Features

### Core Functionality

- ✅ **QR Code-based Attendance Tracking** - Generate unique QR codes for each student
- ✅ **Time In/Time Out System** - Automatic tracking with duplicate prevention
- ✅ **Email OTP Verification** - Secure attendance confirmation via email
- ✅ **Time Tampering Detection** - Prevents system clock manipulation
- ✅ **Offline Mode Support** - Works offline with manual review flags
- ✅ **Real-time Dashboard Analytics** - Visual statistics and charts

### Security Features

- 🔐 BCrypt password hashing
- 🔐 Email-based OTP verification
- 🔐 Internet time synchronization validation
- 🔐 10-second scan cooldown to prevent accidental duplicates
- 🔐 Comprehensive audit trail

### Application Screens

| Screen                   | Description                               |
| ------------------------ | ----------------------------------------- |
| **Splash Screen**        | Application loading with branding         |
| **Login Screen**         | Secure user authentication                |
| **Main Dashboard**       | Central hub with real-time statistics     |
| **Student Registration** | Register students with QR code generation |
| **QR Scanner**           | Real-time webcam QR code scanning         |
| **Student Records**      | View and manage student information       |
| **Scan History**         | Browse and filter attendance records      |
| **Settings**             | Configure system preferences              |

---

## 🛠️ Technology Stack

| Component              | Technology                |
| ---------------------- | ------------------------- |
| **Language**           | C#                        |
| **Framework**          | .NET Framework 4.7.2      |
| **UI Framework**       | Windows Forms             |
| **UI Components**      | Guna.UI2.WinForms         |
| **Database**           | MySQL 5.7+ / MySQL 8.0+   |
| **Database Connector** | MySqlConnector            |
| **QR Code Generation** | QRCoder                   |
| **QR Code Scanning**   | ZXing.Net                 |
| **Email Service**      | MailKit / MimeKit         |
| **Password Hashing**   | BCrypt.Net-Next           |
| **Encryption**         | BouncyCastle.Cryptography |

---

## 📋 Prerequisites

Before you begin, ensure you have the following installed:

- **Windows 10/11** (64-bit recommended)
- **Visual Studio 2019/2022** with .NET desktop development workload
- **MySQL Server 5.7+** or **MySQL 8.0+**
- **MySQL Workbench** (optional, for database management)
- **.NET Framework 4.7.2** Runtime

---

## 🚀 Installation

### Step 1: Clone the Repository

```bash
git clone https://github.com/aarhonsaludar/itp-student-attendance-qrcode.git
cd itp-student-attendance-qrcode
```

### Step 2: Set Up the Database

#### Option A: Using MySQL Command Line

```bash
mysql -u root -p < Database/schema.sql
```

#### Option B: Using MySQL Workbench

1. Open MySQL Workbench
2. Connect to your MySQL server
3. File → Open SQL Script → Select `Database/schema.sql`
4. Execute the script (⚡ or `Ctrl+Shift+Enter`)

#### Verify Installation

After execution, you should have:

- ✅ Database `student_attendance_db` created
- ✅ 7 tables created
- ✅ 5 stored procedures created
- ✅ 4 views created
- ✅ 3 triggers created
- ✅ Sample data inserted

### Step 3: Configure Connection String

Update the connection string in `App.config`:

```xml
<connectionStrings>
    <add name="StudentAttendanceDB"
         connectionString="Server=localhost;Port=3306;Database=student_attendance_db;Uid=YOUR_USERNAME;Pwd=YOUR_PASSWORD;AllowUserVariables=true;CharSet=utf8mb4;SslMode=none;"
         providerName="MySqlConnector" />
</connectionStrings>
```

Replace `YOUR_USERNAME` and `YOUR_PASSWORD` with your MySQL credentials.

### Step 4: Restore NuGet Packages

Open the solution in Visual Studio and restore NuGet packages:

```
Right-click Solution → Restore NuGet Packages
```

Or via Package Manager Console:

```powershell
Update-Package -reinstall
```

### Step 5: Build and Run

1. Open `ITP104-FINAL-PROJECT.sln` in Visual Studio
2. Set the build configuration to `Debug` or `Release`
3. Press `F5` or click **Start** to run the application

---

## 📁 Project Structure

```
studentattendance/
├── 📂 Data/                    # Data access layer
│   ├── DatabaseHelper.cs       # Database connection management
│   ├── ScanHistoryRepository.cs
│   ├── SettingsRepository.cs
│   ├── StudentRepository.cs
│   └── UserRepository.cs
├── 📂 Database/                # SQL scripts and migrations
│   ├── schema.sql              # Main database schema
│   ├── migrations/             # Database migration files
│   └── *.sql                   # Utility SQL scripts
├── 📂 Models/                  # Data models
│   ├── Student.cs
│   ├── User.cs
│   ├── ScanHistory.cs
│   └── ...
├── 📂 Services/                # Business logic services
│   ├── OTPService.cs           # Email OTP service
│   ├── TimeValidationService.cs
│   ├── InputValidator.cs
│   └── ErrorLoggingService.cs
├── 📂 Resources/               # Images, sounds, assets
├── 📂 Documentations/          # Additional documentation
├── 📄 Program.cs               # Application entry point
├── 📄 LoginScreen.cs           # Login form
├── 📄 MainDashboard.cs         # Main dashboard
├── 📄 StudentRegistration.cs   # Student registration form
├── 📄 QRScannerForm.cs         # QR code scanner
├── 📄 StudentRecordScreen.cs   # Student records management
├── 📄 ScanHistoryScreen.cs     # Scan history viewer
├── 📄 SettingsScreen.cs        # System settings
└── 📄 App.config               # Application configuration
```

---

## 🗄️ Database Schema

### Tables

| Table             | Purpose                                  |
| ----------------- | ---------------------------------------- |
| `users`           | System administrators and staff accounts |
| `students`        | Student information with QR code data    |
| `devices`         | QR scanning device management            |
| `scan_history`    | Attendance records (Time In/Out)         |
| `tokens`          | QR code token management                 |
| `system_settings` | Application configuration                |
| `system_logs`     | Audit trail                              |

### Key Stored Procedures

| Procedure                   | Description                                   |
| --------------------------- | --------------------------------------------- |
| `sp_register_student`       | Register new students with QR codes           |
| `sp_record_attendance_scan` | Main attendance procedure (Time In/Out logic) |
| `sp_get_scan_history`       | Retrieve filtered scan history                |
| `sp_get_daily_summary`      | Daily attendance statistics                   |
| `sp_get_student_by_qrcode`  | Look up student by QR code                    |

---

## ⚙️ Configuration

### Email OTP Settings

To enable email OTP verification, configure SMTP settings in the Settings screen or directly in the database:

| Setting        | Description                              |
| -------------- | ---------------------------------------- |
| SMTP Server    | Your email server (e.g., smtp.gmail.com) |
| SMTP Port      | Port number (587 for TLS, 465 for SSL)   |
| Email Username | Your email address                       |
| Email Password | App-specific password (for Gmail)        |

### Default Admin Credentials

After initial setup, use these credentials to log in:

| Username | Password   |
| -------- | ---------- |
| `admin`  | `admin123` |

> ⚠️ **Important:** Change the default password immediately after first login!

---

## 📖 Usage

### Registering a Student

1. Navigate to **Register Student** from the dashboard
2. Fill in student details (ID, Name, Email, Course, etc.)
3. Optionally capture or upload a profile photo
4. Click **Register** to generate a unique QR code
5. Print or save the QR code for the student

### Recording Attendance

1. Navigate to **Scan QR Code** from the dashboard
2. Position the student's QR code in front of the webcam
3. The system will automatically detect and process the QR code
4. If OTP is enabled, the student will receive a verification code via email
5. Enter the OTP to confirm attendance

### Attendance Logic

- **First scan of the day** → Records as **Time In**
- **Second scan of the day** → Records as **Time Out**
- **Third scan onwards** → **Rejected** (attendance already complete)
- **10-second cooldown** prevents accidental double scans

---

## 🔧 Troubleshooting

### Database Connection Issues

1. Verify MySQL server is running
2. Check connection string credentials in `App.config`
3. Ensure the database `student_attendance_db` exists
4. Confirm firewall allows MySQL port (default: 3306)

### Camera Not Detected

1. Check webcam permissions in Windows Settings
2. Ensure no other application is using the camera
3. Try restarting the application

### NuGet Package Errors

```powershell
# Clear NuGet cache and restore
dotnet nuget locals all --clear
nuget restore ITP104-FINAL-PROJECT.sln
```

---

## 📚 Documentation

For more detailed documentation, refer to:

- [Comprehensive System Documentation](COMPREHENSIVE_SYSTEM_DOCUMENTATION.md)
- [System Features Documentation](SYSTEM_FEATURES_DOCUMENTATION.md)
- [Database Deployment Guide](Database/DEPLOYMENT_GUIDE.md)
- [Additional Documentation](Documentations/)

---

## 🤝 Contributing

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

---

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 👨‍💻 Author

**Jaycee**  
ITP104 Final Project - December 2025

---

## 🙏 Acknowledgments

- [Guna UI Framework](https://gunaui.com/) for modern Windows Forms components
- [ZXing.Net](https://github.com/micjahn/ZXing.Net) for QR code scanning
- [QRCoder](https://github.com/codebude/QRCoder) for QR code generation
- [MailKit](https://github.com/jstedfast/MailKit) for email functionality
