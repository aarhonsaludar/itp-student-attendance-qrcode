# Reset Students Database - Quick Guide

## 📁 Files Created

### 1. `reset_students.sql` (FULL RESET)

**Location:** `Database/migrations/reset_students.sql`

**What it does:**

- ❌ Deletes ALL student records
- ❌ Deletes ALL scan history
- ❌ Deletes ALL tokens
- ✅ Resets auto-increment IDs to 1

**Use when:** Starting completely fresh

---

### 2. `reset_students_only.sql` (PARTIAL RESET)

**Location:** `Database/migrations/reset_students_only.sql`

**What it does:**

- ❌ Deletes ALL student records
- ✅ KEEPS scan history (for audit purposes)
- ✅ Creates backup of scan history
- ✅ Resets student IDs to 1

**Use when:** You want to remove students but keep historical data

---

### 3. `reset_students.ps1` (EASY RUNNER)

**Location:** `Database/reset_students.ps1`

**What it does:**

- Interactive PowerShell script
- Runs `reset_students.sql` with safety confirmation
- Shows results and verification

**Use when:** You want the easiest way to reset

---

## 🚀 How to Use

### Method 1: PowerShell Script (EASIEST)

```powershell
cd c:\Users\Jaycee\source\repos\studentattendance\Database
.\reset_students.ps1
```

- Will prompt for confirmation (type `YES`)
- Will ask for MySQL password
- Shows before/after counts

---

### Method 2: Direct MySQL Command

```powershell
cd c:\Users\Jaycee\source\repos\studentattendance\Database
Get-Content migrations\reset_students.sql | mysql -u root -p student_attendance_db
```

---

### Method 3: MySQL Workbench (GUI)

1. Open MySQL Workbench
2. Connect to your database
3. File → Open SQL Script
4. Select `Database/migrations/reset_students.sql`
5. Click Execute (⚡ icon)

---

## ⚠️ Important Notes

### Before Running:

- ⚠️ **BACKUP YOUR DATA** if you might need it later
- ⚠️ This action **CANNOT BE UNDONE**
- ⚠️ Close your application before running

### After Running:

- ✅ Next student will have `student_id = 1`
- ✅ Next scan will have `scan_id = 1`
- ✅ Database is clean and ready for fresh data

---

## 🔍 Verification

After reset, you can verify with:

```sql
SELECT COUNT(*) FROM students;  -- Should be 0
SELECT COUNT(*) FROM scan_history;  -- Should be 0 (full reset) or > 0 (students only)
SHOW TABLE STATUS LIKE 'students';  -- Auto_increment should be 1
```

---

## 📊 What Each Option Deletes

| Item               | reset_students.sql | reset_students_only.sql |
| ------------------ | ------------------ | ----------------------- |
| Student Records    | ❌ Deleted         | ❌ Deleted              |
| Scan History       | ❌ Deleted         | ✅ Kept                 |
| Tokens             | ❌ Deleted         | ❌ Deleted              |
| Student ID Counter | ✅ Reset to 1      | ✅ Reset to 1           |
| Scan ID Counter    | ✅ Reset to 1      | ⚠️ Unchanged            |

---

## 🆘 Troubleshooting

### Error: "Cannot delete or update a parent row"

- Make sure you're using the provided scripts (they disable foreign key checks)

### Error: "Access denied"

- Check your MySQL username/password
- Make sure you have DELETE privileges

### Error: "Database not found"

- Make sure `student_attendance_db` exists
- Check you're connected to the right server

---

## 💡 Use Cases

### Scenario 1: Testing

**Use:** `reset_students.sql` (full reset)

- Clean slate for testing
- No leftover data

### Scenario 2: New Semester

**Use:** `reset_students_only.sql` (keep history)

- Archive old students
- Keep attendance records for reports
- Fresh student roster

### Scenario 3: Production Cleanup

**Recommended:** Manual deletion via application

- Safer than mass deletion
- Better audit trail
- Can be selective

---

## 🔐 Security Note

These scripts require:

- MySQL administrator access
- `DELETE` privilege
- `ALTER` privilege (to reset auto-increment)

Do not share these scripts with regular users.

---

**Created:** 2025-11-28  
**Version:** 1.0  
**Author:** Database Management System
