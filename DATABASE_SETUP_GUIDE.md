# Database Setup Guide - Student Attendance QR System

## Prerequisites

Before starting, ensure you have:

- **MySQL Server 8.0+** installed and running
- **MySQL Client** (mysql command-line tool or MySQL Workbench)
- Admin/root access to MySQL
- PowerShell (for Windows commands)

---

## Step 1: Install MySQL Server (if not installed)

### Download and Install:

1. Go to: https://dev.mysql.com/downloads/mysql/
2. Download MySQL Community Server 8.0.43 or higher
3. Run the installer
4. During installation:
   - Choose "Developer Default" or "Server only"
   - Set root password to: `admin` (or your preferred password)
   - Port: `3306` (default)
   - Start MySQL Server as Windows Service

### Verify Installation:

```powershell
mysql --version
```

You should see: `mysql  Ver 8.0.43 for Win64`

---

## Step 2: Create the Database from Scratch

### Option A: Using PowerShell (Recommended)

Open PowerShell in the project directory:

```powershell
# Navigate to project folder
cd "c:\Users\Jaycee\source\repos\itp-student-attendance-qrcode"

# Step 1: Create the main database schema
Get-Content "Database\schema.sql" | mysql -u root -padmin

# Step 2: Apply Time In/Time Out updates
Get-Content "Database\time_in_out_update.sql" | mysql -u root -padmin -D student_attendance_db

# Step 2.5: Update stored procedure to include time_out
Get-Content "Database\update_scan_history_sp.sql" | mysql -u root -padmin -D student_attendance_db

# Step 3: Verify database was created
mysql -u root -padmin -D student_attendance_db -e "SHOW TABLES;"
```

### Option B: Using MySQL Workbench (GUI)

1. Open MySQL Workbench
2. Connect to your local MySQL server (root/admin)
3. Click **File** → **Open SQL Script**
4. Select `Database\schema.sql`
5. Click the ⚡ **Execute** button
6. Repeat for `Database\time_in_out_update.sql`

---

## Step 3: Verify Database Setup

Run these commands to verify everything is working:

```powershell
# Check tables
mysql -u root -padmin -D student_attendance_db -e "SHOW TABLES;"
```

**Expected Output:**

```
+----------------------------------+
| Tables_in_student_attendance_db |
+----------------------------------+
| devices                          |
| scan_history                     |
| students                         |
| system_logs                      |
| system_settings                  |
| tokens                           |
| users                            |
+----------------------------------+
```

```powershell
# Check stored procedures
mysql -u root -padmin -D student_attendance_db -e "SHOW PROCEDURE STATUS WHERE Db = 'student_attendance_db';"
```

**Expected Procedures:**

- `sp_register_student`
- `sp_record_scan`
- `sp_get_scan_history`
- `sp_get_daily_summary`
- `sp_get_student_by_qrcode`
- `sp_record_attendance_scan` ⭐ (Time In/Time Out)

```powershell
# Check if time_out column exists
mysql -u root -padmin -D student_attendance_db -e "DESCRIBE scan_history;"
```

**Expected Columns (including time_out):**

```
+---------------+---------------+------+-----+-------------------+
| Field         | Type          | Null | Key | Default           |
+---------------+---------------+------+-----+-------------------+
| scan_id       | int           | NO   | PRI | NULL              |
| student_id    | int           | NO   | MUL | NULL              |
| device_id     | int           | YES  | MUL | NULL              |
| scan_type     | enum(...)     | YES  |     | QR                |
| scan_data     | text          | NO   |     | NULL              |
| scan_datetime | datetime      | YES  | MUL | CURRENT_TIMESTAMP |
| time_out      | datetime      | YES  |     | NULL              | ⭐
| scan_purpose  | enum(...)     | YES  |     | attendance        |
| location      | varchar(100)  | YES  |     | NULL              |
| status        | enum(...)     | YES  | MUL | success           |
| notes         | text          | YES  |     | NULL              |
| created_at    | timestamp     | YES  |     | CURRENT_TIMESTAMP |
+---------------+---------------+------+-----+-------------------+
```

---

## Step 4: Set Up Admin User

The database comes with a default admin user, but the password needs to be properly hashed using BCrypt.

### Using the C# Application (Recommended):

1. Open the project in Visual Studio
2. Build the solution
3. Run the application
4. On the login screen, use:
   - **Username:** `admin`
   - **Password:** `admin123`

The application will automatically hash the password on first login.

### Manual Password Setup:

If you need to reset the admin password manually:

```powershell
# Run this SQL to set password to "admin123" (BCrypt hash)
mysql -u root -padmin -D student_attendance_db -e "UPDATE users SET password_hash = '$2a$11$Xvz.kHGlXk0N5WKVvA8oDOvJk9vQ3Vl8P8yKJ9qN9wXnL8mP8nP8m' WHERE username = 'admin';"
```

---

## Step 5: Add Sample Students (Optional)

The schema already includes 5 sample students. To add your own:

```sql
INSERT INTO students (
    student_number, first_name, middle_name, last_name,
    email, phone, year_level, program, section,
    qr_code_data, enrollment_date, status
) VALUES (
    '2300401',           -- Your student number
    'Jaycee',            -- First name
    NULL,                -- Middle name (or NULL)
    'Aguilan',           -- Last name
    'jaycee@school.edu', -- Email
    '09171234567',       -- Phone
    '3',                 -- Year level (1-4, Graduate)
    'Computer Science',  -- Program
    'CS-3A',             -- Section
    'STUDENT-2300401',   -- QR code data (format: STUDENT-{number})
    '2023-08-15',        -- Enrollment date
    'Active'             -- Status
);
```

**Important:** The `qr_code_data` must match the format: `STUDENT-{student_number}`

---

## Step 6: Test the System

### Test Time In/Time Out Logic:

```powershell
# First scan (Time In)
mysql -u root -padmin -D student_attendance_db -e "CALL sp_record_attendance_scan('STUDENT-2300401', 1, 'Main Entrance', @r, @n, @num, @t); SELECT @r as Result, @n as Name, @num as Number, @t as Type;"
```

**Expected:** `SUCCESS: Time In recorded at HH:MM AM/PM`

```powershell
# Second scan (Time Out)
mysql -u root -padmin -D student_attendance_db -e "CALL sp_record_attendance_scan('STUDENT-2300401', 1, 'Main Entrance', @r, @n, @num, @t); SELECT @r as Result, @n as Name, @num as Number, @t as Type;"
```

**Expected:** `SUCCESS: Time Out recorded at HH:MM AM/PM`

```powershell
# Third scan (Rejected)
mysql -u root -padmin -D student_attendance_db -e "CALL sp_record_attendance_scan('STUDENT-2300401', 1, 'Main Entrance', @r, @n, @num, @t); SELECT @r as Result, @n as Name, @num as Number, @t as Type;"
```

**Expected:** `ERROR: Attendance already completed for today (Time In: ..., Time Out: ...)`

### View Scan History:

```powershell
mysql -u root -padmin -D student_attendance_db -e "SELECT * FROM vw_recent_scans LIMIT 10;"
```

---

## Step 7: Update Connection String (if needed)

If you used a different password or settings, update the connection string in:

**File:** `Data\DatabaseHelper.cs`

```csharp
private static readonly string connectionString =
    "Server=localhost;Port=3306;Database=student_attendance_db;User ID=root;Password=admin;";
```

Change `Password=admin` to your actual MySQL root password.

---

## Troubleshooting

### Problem: "Access denied for user 'root'@'localhost'"

**Solution:**

```powershell
# Reset MySQL root password
mysql -u root -p
# Enter old password, then run:
ALTER USER 'root'@'localhost' IDENTIFIED BY 'admin';
FLUSH PRIVILEGES;
```

### Problem: "Database 'student_attendance_db' doesn't exist"

**Solution:**

```powershell
# Recreate database manually
mysql -u root -padmin -e "CREATE DATABASE student_attendance_db CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;"
# Then run schema.sql again
Get-Content "Database\schema.sql" | mysql -u root -padmin
```

### Problem: "Column 'time_out' doesn't exist"

**Solution:**

```powershell
# Apply the Time In/Time Out update
Get-Content "Database\time_in_out_update.sql" | mysql -u root -padmin -D student_attendance_db
```

### Problem: "Stored procedure 'sp_record_attendance_scan' not found"

**Solution:**

```powershell
# Check if procedure exists
mysql -u root -padmin -D student_attendance_db -e "SHOW PROCEDURE STATUS WHERE Name = 'sp_record_attendance_scan';"

# If not found, recreate it
Get-Content "Database\time_in_out_update.sql" | mysql -u root -padmin -D student_attendance_db
```

### Problem: MySQL not starting

**Solution:**

```powershell
# Check MySQL service status
Get-Service MySQL80

# Start MySQL service
Start-Service MySQL80

# Or restart
Restart-Service MySQL80
```

---

## Database Schema Overview

### Tables:

1. **users** - Admin/staff accounts
2. **students** - Student information with QR codes
3. **devices** - QR scanner devices
4. **scan_history** - All scan records (with time_in and time_out)
5. **tokens** - QR code token management
6. **system_settings** - Application configuration
7. **system_logs** - Audit trail

### Key Stored Procedures:

- `sp_record_attendance_scan` - Main Time In/Time Out logic ⭐
- `sp_get_student_by_qrcode` - Look up student by QR code
- `sp_get_scan_history` - Retrieve scan history with filters
- `sp_get_daily_summary` - Get daily attendance statistics

### Views:

- `vw_recent_scans` - Recent scans with attendance status
- `vw_active_students` - All active students
- `vw_student_scan_stats` - Student scan statistics

---

## Clearing Registered Students

### Option 1: Delete All Students (Keep Structure)

```powershell
# Delete all students and their related data
mysql -u root -padmin -D student_attendance_db -e "DELETE FROM students;"
```

This will automatically delete:

- ✅ All student records
- ✅ All associated tokens (QR codes)
- ✅ All scan history (due to CASCADE)
- ✅ Auto-increments are preserved (next student_id continues from where it left off)

### Option 2: Delete All Students + Reset IDs

```powershell
# Delete all students and reset auto-increment
mysql -u root -padmin -D student_attendance_db -e "TRUNCATE TABLE tokens; TRUNCATE TABLE scan_history; TRUNCATE TABLE students;"
```

This will:

- ✅ Delete all student data
- ✅ Reset student_id back to 1
- ⚠️ Must truncate tokens and scan_history first (foreign key constraints)

### Option 3: Delete Specific Student

```powershell
# Delete by student number
mysql -u root -padmin -D student_attendance_db -e "DELETE FROM students WHERE student_number = '2300401';"

# Delete by name
mysql -u root -padmin -D student_attendance_db -e "DELETE FROM students WHERE first_name = 'Jaycee' AND last_name = 'Aguilan';"

# Delete by student ID
mysql -u root -padmin -D student_attendance_db -e "DELETE FROM students WHERE student_id = 1;"
```

### Option 4: Delete All Except Sample Students

```powershell
# Keep only the 5 default sample students (student_id 1-5)
mysql -u root -padmin -D student_attendance_db -e "DELETE FROM students WHERE student_id > 5;"
```

### Option 5: Delete Sample Students Only

```powershell
# Remove the 5 default sample students, keep your registered students
mysql -u root -padmin -D student_attendance_db -e "DELETE FROM students WHERE student_id <= 5;"
```

### Option 6: Clear Scan History Only (Keep Students)

```powershell
# Delete all scan records but keep students
mysql -u root -padmin -D student_attendance_db -e "DELETE FROM scan_history;"

# Or delete scans for specific student
mysql -u root -padmin -D student_attendance_db -e "DELETE FROM scan_history WHERE student_id = 6;"

# Or delete scans older than a specific date
mysql -u root -padmin -D student_attendance_db -e "DELETE FROM scan_history WHERE scan_datetime < '2025-11-01';"
```

### Verify Deletion

```powershell
# Check remaining students
mysql -u root -padmin -D student_attendance_db -e "SELECT student_id, student_number, first_name, last_name, status FROM students;"

# Count students
mysql -u root -padmin -D student_attendance_db -e "SELECT COUNT(*) as total_students FROM students;"

# Check scan history count
mysql -u root -padmin -D student_attendance_db -e "SELECT COUNT(*) as total_scans FROM scan_history;"
```

### Batch Delete Examples

```powershell
# Delete all inactive students
mysql -u root -padmin -D student_attendance_db -e "DELETE FROM students WHERE status = 'Inactive';"

# Delete all suspended students
mysql -u root -padmin -D student_attendance_db -e "DELETE FROM students WHERE status = 'Suspended';"

# Delete students from specific program
mysql -u root -padmin -D student_attendance_db -e "DELETE FROM students WHERE program = 'Computer Science';"

# Delete students from specific year level
mysql -u root -padmin -D student_attendance_db -e "DELETE FROM students WHERE year_level = '4';"
```

### Safety Tips

⚠️ **Always backup before mass deletion:**

```powershell
# Export students to backup file
mysql -u root -padmin -D student_attendance_db -e "SELECT * FROM students;" > students_backup.csv

# Or backup entire database
mysqldump -u root -padmin student_attendance_db > backup_$(Get-Date -Format 'yyyyMMdd_HHmmss').sql
```

✅ **Check count before deletion:**

```powershell
# See how many will be deleted
mysql -u root -padmin -D student_attendance_db -e "SELECT COUNT(*) as will_be_deleted FROM students WHERE status = 'Inactive';"
```

---

## Quick Reset (Start Over)

If you need to completely reset the database:

```powershell
# WARNING: This will delete ALL data!

# Drop and recreate database
mysql -u root -padmin -e "DROP DATABASE IF EXISTS student_attendance_db; CREATE DATABASE student_attendance_db CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;"

# Recreate schema
Get-Content "Database\schema.sql" | mysql -u root -padmin

# Apply Time In/Time Out update
Get-Content "Database\time_in_out_update.sql" | mysql -u root -padmin -D student_attendance_db
```

---

## Next Steps

Once the database is set up:

1. ✅ Run the C# application
2. ✅ Login with `admin` / `admin123`
3. ✅ Register students (or use sample students)
4. ✅ Start QR Scanner
5. ✅ Test Time In/Time Out by scanning student QR codes
6. ✅ View scan history

**For generating student QR codes:** Use the Student Registration screen in the application. It will automatically generate QR codes in the format `STUDENT-{student_number}`.

---

## Support

If you encounter issues:

1. Check the **Troubleshooting** section above
2. Verify MySQL service is running
3. Check connection string in `DatabaseHelper.cs`
4. Review error messages in Visual Studio Output window
5. Check MySQL error log: `C:\ProgramData\MySQL\MySQL Server 8.0\Data\*.err`

---

**Database Setup Complete! 🎉**

Your attendance system is now ready with full Time In/Time Out functionality.
