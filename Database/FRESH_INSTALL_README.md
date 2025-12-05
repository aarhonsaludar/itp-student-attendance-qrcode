# FRESH INSTALLATION GUIDE

## Student Attendance System - Complete Database Setup

This guide provides complete instructions for setting up the Student Attendance System database on a fresh device or new installation.

---

## 📋 PREREQUISITES

### Required Software

1. **MySQL Server 8.0 or higher**

   - Download from: https://dev.mysql.com/downloads/mysql/
   - Ensure MySQL is running as a service
   - Note your root password

2. **MySQL Client (mysql.exe)**

   - Usually installed with MySQL Server
   - Must be accessible from command line (added to PATH)

3. **PowerShell 5.1 or higher** (Windows)
   - Pre-installed on Windows 10/11
   - Check version: `powershell $PSVersionTable.PSVersion`

### Optional Software

- **MySQL Workbench** - For visual database management
- **.NET Framework 4.8** - Required to run the C# application

---

## 🚀 INSTALLATION METHODS

### Method 1: Automated Installation (Recommended)

**Step 1: Prepare Files**

Ensure these files are in the same directory:

- `FRESH_INSTALL_SETUP.sql` - Main installation script
- `run_fresh_install.ps1` - PowerShell automation script
- `run_fresh_install.bat` - Batch file launcher

**Step 2: Run Installation**

**Option A: Using Batch File (Easiest)**

1. Right-click `run_fresh_install.bat`
2. Select "Run as Administrator"
3. Follow the on-screen prompts

**Option B: Using PowerShell**

1. Right-click on the folder
2. Select "Open PowerShell window here"
3. Run: `.\run_fresh_install.ps1`
4. Follow the on-screen prompts

**Step 3: Follow Installation Prompts**

The script will:

1. Check MySQL installation
2. Test MySQL connection
3. Prompt for MySQL root password
4. Warn about database deletion (type YES to confirm)
5. Execute installation script
6. Verify installation
7. Display summary and next steps

**Step 4: Verify Installation**

The script automatically verifies:

- All tables created (6 tables)
- All procedures created (3 procedures)
- All views created (4 views)
- All triggers created (3 triggers)
- Default data inserted

---

### Method 2: Manual Installation

**Step 1: Open MySQL Command Line**

```bash
mysql -u root -p
```

Enter your MySQL root password when prompted.

**Step 2: Execute Installation Script**

From MySQL prompt:

```sql
source C:/path/to/FRESH_INSTALL_SETUP.sql
```

Or from command line:

```bash
mysql -u root -p < FRESH_INSTALL_SETUP.sql
```

**Step 3: Verify Installation**

```sql
USE student_attendance_db;
SHOW TABLES;
SHOW PROCEDURE STATUS WHERE Db = 'student_attendance_db';
SHOW TRIGGERS;
```

---

### Method 3: Using MySQL Workbench

**Step 1: Open MySQL Workbench**

1. Launch MySQL Workbench
2. Connect to your local MySQL instance
3. Enter root password

**Step 2: Open SQL Script**

1. Click File → Open SQL Script
2. Navigate to `FRESH_INSTALL_SETUP.sql`
3. Click Open

**Step 3: Execute Script**

1. Click the lightning bolt icon (Execute)
2. Wait for execution to complete (should take 10-30 seconds)
3. Check the Output panel for success messages

**Step 4: Verify Installation**

1. Refresh the Schemas panel
2. Expand `student_attendance_db`
3. Verify Tables, Views, and Stored Procedures are present

---

## 📦 WHAT GETS INSTALLED

### Database Structure

**Tables (6):**

1. `users` - Admin and staff accounts
2. `students` - Student information and QR codes
3. `devices` - QR scanner devices
4. `scan_history` - Attendance records with time validation
5. `system_settings` - Configuration parameters
6. `system_logs` - Audit trail

**Stored Procedures (3):**

1. `sp_record_attendance_scan_secure` - Main attendance recording with validation
2. `sp_get_daily_summary` - Daily statistics
3. `sp_get_student_attendance` - Student attendance history

**Views (4):**

1. `vw_recent_scans` - Last 24 hours of scans
2. `vw_student_scan_stats` - Student statistics
3. `vw_scans_pending_review` - Flagged/offline scans
4. `vw_daily_offline_scans` - Offline scan summary

**Triggers (3):**

1. `trg_student_update_log` - Log student updates
2. `trg_student_delete_log` - Log student deletions
3. `trg_update_device_active` - Update device last active time

### Default Data

**Users (2):**

- **Admin Account**
  - Username: `admin`
  - Password: `admin123` (MUST CHANGE IMMEDIATELY)
  - Role: admin
- **Staff Account**
  - Username: `staff1`
  - Password: `admin123` (MUST CHANGE IMMEDIATELY)
  - Role: staff

**Devices (3):**

- QR Scanner 01 - Main Building - Entrance
- QR Scanner 02 - Library - Front Desk
- QR Scanner 03 - Computer Lab - Room 301

**System Settings (15):**

- Scanner settings (enabled, timeout, beep, cooldown)
- Time validation settings (min/max duration, drift tolerance)
- OTP settings (expiration, resend limit)
- UI settings (theme, font)
- Database version tracking

**Sample Student (1):**

- Student Number: 2024-00001
- Name: Juan Santos Dela Cruz
- Program: BS Information Technology
- Status: Active
- _(For testing purposes - can be deleted)_

---

## 🔧 POST-INSTALLATION CONFIGURATION

### 1. Update C# Application Configuration

Edit `App.config` in your C# project:

```xml
<connectionStrings>
  <add name="MySqlConnection"
       connectionString="Server=localhost;
                         Port=3306;
                         Database=student_attendance_db;
                         Uid=root;
                         Pwd=YOUR_MYSQL_PASSWORD;
                         SslMode=None;
                         CharSet=utf8mb4;
                         AllowUserVariables=True;
                         ConnectionTimeout=30;"
       providerName="MySql.Data.MySqlClient" />
</connectionStrings>
```

Replace `YOUR_MYSQL_PASSWORD` with your actual MySQL root password.

### 2. Configure SMTP Email Settings

Add to `App.config`:

```xml
<appSettings>
  <add key="SmtpServer" value="smtp.gmail.com" />
  <add key="SmtpPort" value="587" />
  <add key="SmtpUsername" value="your-email@gmail.com" />
  <add key="SmtpPassword" value="your-app-password" />
  <add key="SmtpEnableSsl" value="true" />
  <add key="EmailFrom" value="your-email@gmail.com" />
  <add key="EmailFromName" value="Student Attendance System" />
</appSettings>
```

**For Gmail:**

1. Enable 2-Factor Authentication
2. Generate App Password: https://myaccount.google.com/apppasswords
3. Use App Password in SmtpPassword

### 3. First Login and Security

**IMPORTANT SECURITY STEPS:**

1. **Change Default Passwords**

   - Login with `admin` / `admin123`
   - Navigate to User Management
   - Change password immediately
   - Use strong password (min 8 chars, mixed case, numbers, symbols)

2. **Update Staff Account**

   - Change staff1 password
   - Update staff1 email address
   - Or create new staff accounts

3. **Verify Security Settings**
   - Check auto-logout timer (default: 15 minutes)
   - Verify OTP expiration (default: 5 minutes)
   - Review time validation settings

### 4. Add Students

**Via Application:**

1. Login to application
2. Navigate to Student Records
3. Click "Add New Student"
4. Fill in required information
5. Upload photo (optional)
6. System auto-generates QR code
7. Print QR code for student

**Via Database (Bulk Import):**

```sql
USE student_attendance_db;

INSERT INTO students (
    student_number, first_name, middle_name, last_name,
    email, phone, sex, year_level, program, section,
    home_address, qr_code_data, status, enrollment_date
) VALUES (
    '2024-00002', 'Maria', 'Santos', 'Garcia',
    'maria.garcia@students.plc.edu.ph', '09198765432',
    'Female', '3', 'BS Computer Science', 'CS-3A',
    'Calamba, Laguna', 'QR_2024-00002_MARIA_GARCIA',
    'Active', '2024-08-15'
);
```

### 5. Configure Additional Devices

If you have more QR scanners:

```sql
USE student_attendance_db;

INSERT INTO devices (device_name, device_type, location, status)
VALUES ('QR Scanner 04', 'QR_SCANNER', 'Gymnasium', 'active');
```

---

## ✅ VERIFICATION CHECKLIST

After installation, verify:

- [ ] Database `student_attendance_db` created
- [ ] All 6 tables present
- [ ] All 3 stored procedures present
- [ ] All 4 views present
- [ ] All 3 triggers present
- [ ] 2 default users created
- [ ] 3 default devices created
- [ ] 15+ system settings inserted
- [ ] Can login to C# application
- [ ] Can change admin password
- [ ] Can add new student
- [ ] QR code generates automatically
- [ ] Can scan QR code (Time-In)
- [ ] OTP email received
- [ ] Can complete Time-Out (after 15 min)

---

## 🔍 TROUBLESHOOTING

### Problem: MySQL not found in PATH

**Solution:**

1. Find MySQL installation directory (usually `C:\Program Files\MySQL\MySQL Server 8.0\bin`)
2. Add to System PATH:
   - Right-click This PC → Properties
   - Advanced System Settings
   - Environment Variables
   - Edit PATH variable
   - Add MySQL bin directory
   - Restart PowerShell

### Problem: Access denied for user 'root'

**Solution:**

- Verify MySQL root password
- Reset root password if forgotten:
  ```bash
  mysqld --skip-grant-tables
  mysql -u root
  ALTER USER 'root'@'localhost' IDENTIFIED BY 'new_password';
  FLUSH PRIVILEGES;
  ```

### Problem: Database already exists error

**Solution:**

- The script drops existing database
- If error persists:
  ```sql
  DROP DATABASE IF EXISTS student_attendance_db;
  ```
- Then re-run installation script

### Problem: C# application cannot connect

**Solution:**

1. Verify MySQL service is running:
   ```powershell
   Get-Service -Name MySQL*
   ```
2. Check connection string in App.config
3. Test connection manually:
   ```bash
   mysql -u root -p -h localhost -P 3306
   ```
4. Verify firewall allows port 3306

### Problem: OTP emails not sending

**Solution:**

1. Verify SMTP settings in App.config
2. For Gmail:
   - Enable "Less secure app access" OR
   - Use App Password with 2FA
3. Check spam/junk folder
4. Test SMTP connection separately
5. Verify internet connection

### Problem: Time validation errors

**Solution:**

1. Ensure device time is correct
2. Check internet connection
3. Verify time validation settings:
   ```sql
   SELECT * FROM system_settings
   WHERE setting_key LIKE '%time%' OR setting_key LIKE '%drift%';
   ```
4. Adjust tolerance if needed (default: 5 minutes)

---

## 📊 DATABASE SCHEMA DIAGRAM

```
┌─────────────┐
│    users    │
├─────────────┤
│ user_id (PK)│
│ username    │
│ password    │
│ email       │
│ role        │
└─────────────┘
       │
       │ (logs)
       ▼
┌─────────────────┐
│  system_logs    │
├─────────────────┤
│ log_id (PK)     │
│ user_id (FK)    │
│ action          │
│ timestamp       │
└─────────────────┘

┌──────────────┐         ┌─────────────────┐         ┌──────────────┐
│   students   │         │  scan_history   │         │   devices    │
├──────────────┤         ├─────────────────┤         ├──────────────┤
│student_id(PK)│────────▶│ scan_id (PK)    │◀────────│device_id (PK)│
│student_number│         │ student_id (FK) │         │device_name   │
│first_name    │         │ device_id (FK)  │         │location      │
│last_name     │         │ scan_datetime   │         │status        │
│email         │         │ time_out        │         └──────────────┘
│qr_code_data  │         │ validation_*    │
│photo_path    │         │ tick_count_*    │
│status        │         │ time_drift_*    │
└──────────────┘         └─────────────────┘

┌───────────────────┐
│  system_settings  │
├───────────────────┤
│ setting_id (PK)   │
│ setting_key       │
│ setting_value     │
│ setting_category  │
└───────────────────┘
```

---

## 📝 FEATURES INCLUDED

### ✅ Security Features

- ✓ BCrypt password hashing
- ✓ Online time validation (Google/TimeAPI/Microsoft)
- ✓ Offline TickCount tampering detection
- ✓ Duration enforcement (15 min - 18 hours)
- ✓ OTP email verification
- ✓ Comprehensive audit logging
- ✓ WiFi disconnect detection
- ✓ Manual review system for flagged scans

### ✅ Time Validation

- ✓ 5-minute time drift tolerance (online)
- ✓ 3-minute TickCount tolerance (offline)
- ✓ Minimum 15 minutes between Time-In/Time-Out
- ✓ Maximum 18 hours session duration
- ✓ 5-second scan cooldown (anti-spam)

### ✅ Data Management

- ✓ Complete student CRUD operations
- ✓ QR code auto-generation
- ✓ Photo management
- ✓ Device tracking
- ✓ System settings configuration
- ✓ Audit trail for all changes

### ✅ Reporting

- ✓ Daily attendance summary
- ✓ Student attendance history
- ✓ Recent scans view (24 hours)
- ✓ Offline scans monitoring
- ✓ Scans pending review
- ✓ Student statistics

---

## 📞 SUPPORT

If you encounter issues:

1. Check the installation log file: `installation_log_YYYYMMDD_HHMMSS.txt`
2. Review this README troubleshooting section
3. Check MySQL error logs
4. Verify all prerequisites are met
5. Contact system administrator

---

## 📄 LICENSE & VERSION

- **Version:** 1.0.0
- **Date:** December 5, 2025
- **Database:** MySQL 8.0+
- **Application:** .NET Framework 4.8
- **Institution:** Pamantasan ng Cabuyao

---

## ⚠️ IMPORTANT NOTES

1. **ALWAYS change default passwords immediately after installation**
2. **Backup database regularly** (recommended: daily backups)
3. **Keep MySQL updated** for security patches
4. **Secure database credentials** - never commit to version control
5. **Test in development environment** before production deployment
6. **Document any custom changes** to the schema

---

**END OF FRESH INSTALLATION GUIDE**
