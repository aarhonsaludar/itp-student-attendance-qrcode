# Database Deployment Guide
## Student Attendance System - QR Code Integration

---

## 📋 Table of Contents
1. [Fresh Installation (New Device)](#fresh-installation-new-device)
2. [Database Schema Overview](#database-schema-overview)
3. [Migration Files (Reference Only)](#migration-files-reference-only)
4. [Verification Steps](#verification-steps)
5. [Troubleshooting](#troubleshooting)

---

## 🆕 Fresh Installation (New Device)

### Prerequisites
- MySQL Server 5.7+ or MySQL 8.0+
- MySQL client or MySQL Workbench
- Database user with CREATE DATABASE privileges

### Step-by-Step Installation

#### 1. **Execute the Schema File**
Simply run the main schema file to create the complete database:

```bash
mysql -u root -p < Database/schema.sql
```

**OR** using MySQL Workbench:
1. Open MySQL Workbench
2. Connect to your MySQL server
3. File → Open SQL Script → Select `schema.sql`
4. Execute the script (⚡ lightning icon or Ctrl+Shift+Enter)

#### 2. **Verify Installation**
After execution, you should see:
- ✅ Database `student_attendance_db` created
- ✅ 7 tables created
- ✅ 5 stored procedures created
- ✅ 4 views created
- ✅ 3 triggers created
- ✅ Sample data inserted

#### 3. **Update Application Connection String**
Update your C# application's connection string in `appsettings.json` or configuration file:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=student_attendance_db;User=root;Password=your_password;"
  }
}
```

---

## 📊 Database Schema Overview

### Tables Created

| Table Name | Purpose | Key Features |
|------------|---------|--------------|
| `users` | System administrators and staff | BCrypt password hashing, role-based access |
| `students` | Student information | QR code data, sex field, enrollment tracking |
| `devices` | QR scanning devices | Device management and location tracking |
| `scan_history` | Attendance records | **Time In/Time Out tracking**, duplicate prevention |
| `tokens` | QR code token management | Token lifecycle and revocation |
| `system_settings` | Application configuration | Scanner settings, UI preferences |
| `system_logs` | Audit trail | User actions and data changes |

### Key Features Included

#### ✅ **Time In/Time Out System**
- First scan of the day = **Time In**
- Second scan of the day = **Time Out**
- Third scan = **Rejected** (attendance already complete)
- 10-second cooldown to prevent accidental double scans

#### ✅ **Student Management**
- Complete student profile (name, email, phone, program, year level)
- Sex/Gender field (Male/Female/NULL)
- QR code data storage
- Status tracking (Active/Inactive/Suspended)

#### ✅ **Stored Procedures**

1. **`sp_register_student`** - Register new students with QR codes
2. **`sp_record_attendance_scan`** - ⭐ **Main attendance procedure** (Time In/Time Out logic)
3. **`sp_get_scan_history`** - Retrieve filtered scan history
4. **`sp_get_daily_summary`** - Daily attendance statistics
5. **`sp_get_student_by_qrcode`** - Look up student by QR code

#### ✅ **Views for Reporting**

1. **`vw_active_students`** - All active students with token counts
2. **`vw_recent_scans`** - Last 24 hours of scans with attendance status
3. **`vw_student_scan_stats`** - Per-student scan statistics
4. **`vw_device_stats`** - Device usage statistics

#### ✅ **Triggers for Audit Trail**

1. **`trg_student_update_log`** - Logs student status changes
2. **`trg_student_delete_log`** - Logs student deletions
3. **`trg_update_device_active`** - Updates device last_active timestamp

### Default Data Inserted

- **2 Admin Users** (username: `admin` and `staff1`)
  - ⚠️ Default password hash is placeholder - will be set by C# application on first run
- **2 QR Scanner Devices** (Main Building and Library)
- **7 System Settings** (scanner config, UI theme, etc.)
- **5 Sample Students** (for testing)
- **5 Sample Scan Records** (for testing)

---

## 📁 Migration Files (Reference Only)

### ⚠️ **DO NOT RUN THESE ON FRESH INSTALLATIONS**

These files were used during development to update existing databases. They are **already incorporated** into `schema.sql`:

| File | Purpose | Status |
|------|---------|--------|
| `add_sex_column.sql` | Adds sex field to students | ✅ Already in schema.sql |
| `add_timeout_column.sql` | Adds time_out to scan_history | ✅ Already in schema.sql |
| `time_in_out_update.sql` | Updates to Time In/Out system | ✅ Already in schema.sql |
| `create_admin_user.sql` | Admin user creation | ⚠️ Use only for manual admin creation |
| `fix_admin_password.sql` | Password hash fixes | ⚠️ Use only if needed |
| `reset_admin_password.sql` | Password reset | ⚠️ Use only if needed |
| `update_admin_hash.sql` | Update admin password | ⚠️ Use only if needed |
| `recreate_admin.sql` | Recreate admin user | ⚠️ Use only if needed |
| `update_scan_history_sp.sql` | Stored procedure updates | ✅ Already in schema.sql |

### When to Use Migration Files

**Only use migration files if:**
- You have an **existing old database** that needs updating
- You need to manually fix admin user passwords
- You're debugging specific database issues

**For new installations:** Just use `schema.sql` ✅

---

## ✅ Verification Steps

### 1. Check Database Creation
```sql
SHOW DATABASES LIKE 'student_attendance_db';
```

### 2. Check Tables
```sql
USE student_attendance_db;
SHOW TABLES;
```
Expected output: 7 tables

### 3. Check Stored Procedures
```sql
SHOW PROCEDURE STATUS WHERE Db = 'student_attendance_db';
```
Expected output: 5 procedures

### 4. Check Views
```sql
SHOW FULL TABLES WHERE Table_type = 'VIEW';
```
Expected output: 4 views

### 5. Test Sample Data
```sql
-- Check students
SELECT COUNT(*) FROM students;  -- Should return 5

-- Check scan history
SELECT COUNT(*) FROM scan_history;  -- Should return 5

-- Check devices
SELECT COUNT(*) FROM devices;  -- Should return 2
```

### 6. Test Time In/Time Out Procedure
```sql
-- Test attendance scan
CALL sp_record_attendance_scan(
    'ID:2024-STU-0001|Name:John M. Smith|Email:john.smith@school.edu|Course:Computer Science|Year:3',
    1,
    'Test Location',
    @result,
    @name,
    @number,
    @type
);

SELECT @result AS Result, @name AS StudentName, @number AS StudentNumber, @type AS ScanType;
```

---

## 🔧 Troubleshooting

### Issue: "Database already exists" Error
**Solution:** Drop the existing database first (⚠️ **WARNING: This deletes all data!**)
```sql
DROP DATABASE IF EXISTS student_attendance_db;
```
Then re-run `schema.sql`

### Issue: "Access denied" Error
**Solution:** Ensure your MySQL user has proper privileges:
```sql
GRANT ALL PRIVILEGES ON student_attendance_db.* TO 'your_user'@'localhost';
FLUSH PRIVILEGES;
```

### Issue: Stored Procedure Not Found
**Solution:** Check if procedures were created:
```sql
SHOW PROCEDURE STATUS WHERE Db = 'student_attendance_db';
```
If missing, re-run the stored procedure section of `schema.sql`

### Issue: Character Encoding Problems
**Solution:** Ensure your MySQL client uses UTF-8:
```bash
mysql -u root -p --default-character-set=utf8mb4 < Database/schema.sql
```

### Issue: Admin Login Not Working
**Solution:** The default password hashes are placeholders. Your C# application should:
1. Detect the placeholder hash on first run
2. Create a proper BCrypt hash
3. Update the database

Or manually create an admin user using `create_admin_user.sql`

---

## 📝 Notes

### Database Version
- Current Schema Version: **1.0.0**
- Last Updated: **2025-11-21**
- Compatible with: MySQL 5.7+, MySQL 8.0+

### Character Set
- Database: `utf8mb4_unicode_ci`
- All tables: `utf8mb4_unicode_ci`
- Supports international characters and emojis

### Storage Engine
- All tables use **InnoDB** for:
  - ACID compliance
  - Foreign key support
  - Transaction support
  - Better crash recovery

---

## 🚀 Quick Start Checklist

- [ ] MySQL Server installed and running
- [ ] Execute `schema.sql`
- [ ] Verify 7 tables created
- [ ] Verify 5 stored procedures created
- [ ] Verify 4 views created
- [ ] Test sample data exists
- [ ] Update C# application connection string
- [ ] Test application connectivity
- [ ] Set up admin user password (via C# app)
- [ ] Test QR code scanning functionality

---

## 📞 Support

If you encounter issues:
1. Check the [Troubleshooting](#troubleshooting) section
2. Verify MySQL version compatibility
3. Check MySQL error logs
4. Review C# application logs

---

**Ready to deploy!** 🎉

Your `schema.sql` file contains everything needed for a complete fresh installation.
