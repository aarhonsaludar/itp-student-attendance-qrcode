# 🚀 Student Attendance System - Deployment Instructions

**Complete guide for deploying the Student Attendance QR Code System to a new device**

---

## 📋 Table of Contents

1. [Prerequisites](#prerequisites)
2. [Database Setup](#database-setup)
3. [Application Setup](#application-setup)
4. [Post-Deployment Verification](#post-deployment-verification)
5. [Troubleshooting](#troubleshooting)

---

## 🔧 Prerequisites

### Software Requirements

- **MySQL Server 8.0+** (with root or admin access)
- **Windows OS** (Windows 10/11)
- **.NET Framework 4.7.2+** (for C# WinForms application)
- **MySQL Workbench** (optional, for database management)

### Files to Transfer

Copy the entire project folder containing:

```
studentattendance/
├── Database/
│   ├── schema.sql                           ⭐ Main schema
│   ├── migrations/
│   │   ├── migration_004_add_time_validation.sql    ⭐ Required
│   │   └── migration_004_patch_duplicate_fix.sql    ⭐ Required
├── bin/Debug/ or bin/Release/               ⭐ Compiled application
├── App.config
└── (All other project files)
```

---

## 💾 Database Setup

### Step 1: Install MySQL Server

1. Download MySQL Installer from [mysql.com](https://dev.mysql.com/downloads/installer/)
2. Run installer and select **"Server Only"** or **"Full"** installation
3. During configuration:
   - Set root password: `admin` (or your preferred password)
   - Port: `3306` (default)
   - Start MySQL Server as Windows Service: ✅ Enabled

### Step 2: Verify MySQL Installation

Open PowerShell and test connection:

```powershell
mysql -u root -p
# Enter password when prompted
```

If connected successfully, you'll see:

```
mysql>
```

Type `exit` to close.

### Step 3: Create Database from Schema

**Option A: Using MySQL Command Line**

```powershell
# Navigate to Database folder
cd "C:\path\to\studentattendance\Database"

# Import schema (creates database and base structure)
mysql -u root -padmin < schema.sql
```

**Option B: Using MySQL Workbench**

1. Open MySQL Workbench
2. Connect to MySQL Server (localhost, root, password: admin)
3. File → Open SQL Script → Select `schema.sql`
4. Click **⚡ Execute** (lightning bolt icon)
5. Verify success message

### Step 4: Apply Migration 004 (Time Validation Support)

```powershell
# Still in Database folder
cd migrations

# Apply migration 004
mysql -u root -padmin student_attendance_db < migration_004_add_time_validation.sql
```

**Expected Output:**

```
Migration 004 completed: Time validation tracking added
```

### Step 5: Apply Duplicate Fix Patch

```powershell
# Apply the patch to fix offline mode duplicate scans
mysql -u root -padmin student_attendance_db < migration_004_patch_duplicate_fix.sql
```

**Expected Output:**

```
Patch applied successfully - Duplicate Time In issue fixed
```

### Step 6: Verify Database Structure

Connect to MySQL and verify tables and procedures:

```powershell
mysql -u root -padmin -D student_attendance_db
```

Then run these verification commands:

```sql
-- 1. Check tables (should show 7 tables)
SHOW TABLES;

-- Expected output:
-- devices
-- scan_history
-- students
-- system_logs
-- system_logs_archive (if cleanup was run)
-- system_settings
-- users

-- 2. Check stored procedures (should show 2)
SHOW PROCEDURE STATUS WHERE Db = 'student_attendance_db';

-- Expected output:
-- sp_get_daily_summary
-- sp_record_attendance_scan_secure

-- 3. Check views (should show 4)
SHOW FULL TABLES WHERE Table_type = 'VIEW';

-- Expected output:
-- vw_daily_offline_scans
-- vw_recent_scans
-- vw_scans_pending_review
-- vw_student_scan_stats

-- 4. Verify scan_history has validation columns
DESCRIBE scan_history;

-- Should include these columns:
-- validation_status
-- requires_review
-- client_time
-- server_time
-- time_drift_seconds

-- 5. Exit MySQL
exit
```

---

## 🖥️ Application Setup

### Step 1: Configure Database Connection

Edit `App.config` in the project root:

```xml
<connectionStrings>
  <add name="StudentAttendanceDB"
       connectionString="Server=localhost;Database=student_attendance_db;Uid=root;Pwd=admin;SslMode=none;"
       providerName="MySql.Data.MySqlClient" />
</connectionStrings>
```

⚠️ **Update password** if you used a different MySQL root password.

### Step 2: Install Required NuGet Packages

If rebuilding from source, ensure these packages are installed:

- **MySqlConnector** (v2.x or higher)
- **BCrypt.Net-Next** (for password hashing)
- **ZXing.Net** (for QR code generation/scanning)

### Step 3: Build the Application

**Option A: Using Visual Studio**

1. Open `ITP104-FINAL-PROJECT.sln`
2. Build → Rebuild Solution
3. Check for errors in Output window

**Option B: Using MSBuild (Command Line)**

```powershell
cd "C:\path\to\studentattendance"
msbuild ITP104-FINAL-PROJECT.sln /p:Configuration=Release
```

### Step 4: Run the Application

**From Visual Studio:**

- Press F5 (Debug) or Ctrl+F5 (Run without debugging)

**From Executable:**

```powershell
cd bin\Release  # or bin\Debug
.\ITP104-FINAL-PROJECT.exe
```

### Step 5: First-Time Login

**Default Credentials:**

- Username: `admin`
- Password: `admin123`

⚠️ **IMPORTANT:** Change the default admin password after first login via Settings → User Management.

---

## ✅ Post-Deployment Verification

### 1. Test Database Connection

**In the application:**

1. Launch the application
2. Login screen should appear (no connection errors)
3. Login with default credentials
4. You should see the Main Dashboard

**If connection fails:**

- Verify MySQL service is running: `Get-Service MySQL*`
- Check `App.config` connection string
- Test MySQL connection: `mysql -u root -padmin -D student_attendance_db`

### 2. Verify Sample Data

Check that sample students were created:

**In MySQL:**

```sql
USE student_attendance_db;

-- Should return 5 sample students
SELECT COUNT(*) FROM students;

-- View sample students
SELECT student_number, first_name, last_name, program, year_level
FROM students
LIMIT 5;
```

**In Application:**

1. Navigate to **Student Records** screen
2. You should see 5 pre-populated students:
   - 2024-STU-0001 (John Smith)
   - 2024-STU-0002 (Emily Johnson)
   - 2024-STU-0003 (Michael Brown)
   - 2024-STU-0004 (Sarah Davis)
   - 2024-STU-0005 (David Wilson)

### 3. Test QR Code Scanning

1. Go to **Student Records** → Select a student → Click **View QR Code**
2. QR code should display properly
3. Go to **QR Scanner** screen
4. Test scan using webcam or upload QR image
5. Verify successful attendance recording

### 4. Test Offline Mode (Time Validation)

1. **Disconnect internet** on the device
2. Go to QR Scanner screen
3. Scan a student QR code
4. Should see: "⚠️ Warning: Offline mode - Attendance flagged for review"
5. Check database:
   ```sql
   SELECT * FROM vw_scans_pending_review;
   -- Should show the offline scan with requires_review = TRUE
   ```

### 5. Verify Dashboard Charts

1. Open Main Dashboard
2. Check that charts display:
   - **Daily Attendance Trends** (line chart)
   - **Attendance by Program** (pie chart)
   - **Recent Scans** (data grid - last 10 scans)
   - **Summary Cards** (total students, today's attendance, etc.)

---

## 🐛 Troubleshooting

### Issue 1: "Unable to connect to MySQL server"

**Solution:**

```powershell
# Check if MySQL service is running
Get-Service MySQL*

# If not running, start it
Start-Service MySQL80  # or MySQL version number

# Verify connection
mysql -u root -padmin
```

### Issue 2: "Table 'scan_history' doesn't exist"

**Cause:** Schema not imported correctly

**Solution:**

```powershell
cd Database
mysql -u root -padmin < schema.sql
```

### Issue 3: "Procedure 'sp_record_attendance_scan_secure' doesn't exist"

**Cause:** Migration 004 not applied

**Solution:**

```powershell
cd Database\migrations
mysql -u root -padmin student_attendance_db < migration_004_add_time_validation.sql
mysql -u root -padmin student_attendance_db < migration_004_patch_duplicate_fix.sql
```

### Issue 4: Duplicate Time In records in offline mode

**Cause:** Patch not applied

**Solution:**

```powershell
cd Database\migrations
mysql -u root -padmin student_attendance_db < migration_004_patch_duplicate_fix.sql
```

### Issue 5: Login fails with "Invalid username or password"

**Cause:** Default users not created or password not hashed properly

**Solution:**

```sql
-- Connect to MySQL
mysql -u root -padmin -D student_attendance_db

-- Check if users exist
SELECT username, full_name FROM users;

-- If empty or passwords are TEMP_HASH_REPLACE_ON_FIRST_RUN
-- The application will hash them on first run, just restart the app
```

### Issue 6: QR Code scanner not working

**Possible causes:**

1. **Webcam permission denied** → Allow camera access in Windows Settings
2. **ZXing.Net library missing** → Rebuild solution to restore NuGet packages
3. **Invalid QR data** → Ensure student has `qr_code_data` field populated

### Issue 7: Charts not displaying on Dashboard

**Solution:**

1. Ensure sample scan history exists:
   ```sql
   SELECT COUNT(*) FROM scan_history;
   -- Should have at least 5 sample records
   ```
2. If empty, the chart data may be insufficient
3. Perform a test scan to generate data

---

## 📊 Database Object Reference

### Tables (7)

| Table                 | Purpose                                   |
| --------------------- | ----------------------------------------- |
| `users`               | System administrators/staff accounts      |
| `students`            | Student information with QR codes         |
| `devices`             | Scanning devices (QR scanners)            |
| `scan_history`        | All attendance scan records               |
| `system_settings`     | Application configuration                 |
| `system_logs`         | Audit trail for changes                   |
| `system_logs_archive` | Archived old logs (created after cleanup) |

### Stored Procedures (2)

| Procedure                          | Purpose                                        |
| ---------------------------------- | ---------------------------------------------- |
| `sp_record_attendance_scan_secure` | Main attendance recording with time validation |
| `sp_get_daily_summary`             | Daily attendance statistics                    |

### Views (4)

| View                      | Purpose                               |
| ------------------------- | ------------------------------------- |
| `vw_recent_scans`         | Last 24 hours of scans                |
| `vw_student_scan_stats`   | Per-student attendance statistics     |
| `vw_scans_pending_review` | Offline scans requiring manual review |
| `vw_daily_offline_scans`  | Daily summary of offline scans        |

### Triggers (3)

| Trigger                    | Purpose                             |
| -------------------------- | ----------------------------------- |
| `trg_student_update_log`   | Log student status changes          |
| `trg_student_delete_log`   | Log student deletions               |
| `trg_update_device_active` | Update device last_active timestamp |

---

## 🔐 Security Recommendations

1. **Change default passwords** immediately after deployment
2. **Use strong MySQL root password** (not 'admin' in production)
3. **Restrict MySQL network access** if not needed remotely
4. **Enable Windows Firewall** rules for MySQL (port 3306) only if remote access required
5. **Regular backups**:
   ```powershell
   # Create backup
   mysqldump -u root -padmin student_attendance_db > backup_$(Get-Date -Format 'yyyyMMdd_HHmmss').sql
   ```

---

## 📝 Additional Notes

### System Requirements

- **RAM:** 4GB minimum, 8GB recommended
- **Storage:** 500MB for application + database
- **Network:** Internet connection required for time validation (hybrid offline mode supported)
- **Webcam:** Required for QR code scanning

### Default System Settings

- QR Scanner: Enabled
- Connection timeout: 30 seconds
- Beep on scan: Enabled
- Auto logout timer: 15 minutes
- Theme: Light

### Support & Documentation

- Full documentation in `Documentations/` folder
- Database schema details: `Database/README.md`
- Offline mode guide: `Documentations/HYBRID_OFFLINE_MODE.md`
- Security guide: `Documentations/DATABASE_TIMESTAMP_SECURITY.md`

---

## ✨ Deployment Complete!

Your Student Attendance System is now ready for use.

**Quick Start Checklist:**

- ✅ MySQL Server running
- ✅ Database schema created
- ✅ Migration 004 applied
- ✅ Patch applied
- ✅ Application running
- ✅ Default login successful
- ✅ Sample data verified
- ✅ QR scanning tested

**Next Steps:**

1. Add your actual students via Student Registration screen
2. Configure devices/scanners
3. Test offline mode functionality
4. Review flagged scans in Scan History
5. Change default admin password

---

**Last Updated:** November 29, 2025  
**Database Version:** 1.0.0 + Migration 004  
**Application Version:** ITP104 Final Project
