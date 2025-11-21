# Database Directory Structure
**Student Attendance System - QR Code Integration**

---

## 📁 Directory Overview

```
Database/
├── schema.sql                    ⭐ MAIN FILE - Use this for fresh installations
├── DEPLOYMENT_GUIDE.md           📖 Complete deployment instructions
├── SCHEMA_UPDATE_SUMMARY.md      📝 What's included in schema.sql
├── migrations/                   📦 Historical schema changes (optional)
│   ├── add_sex_column.sql
│   ├── add_timeout_column.sql
│   ├── time_in_out_update.sql
│   └── update_scan_history_sp.sql
└── archive/                      🗄️ Old/deprecated files (safe to delete)
    ├── dump.sql
    ├── create_admin_user.sql
    ├── fix_admin_password.sql
    ├── reset_admin_password.sql
    ├── update_admin_hash.sql
    └── recreate_admin.sql
```

---

## 🎯 Which File to Use?

### For New Device/Fresh Installation
**Use:** `schema.sql` ✅

This single file contains everything:
- All 7 tables with latest schema
- All 5 stored procedures (including Time In/Time Out system)
- All 4 views
- All 3 triggers
- Sample data for testing

**Command:**
```bash
mysql -u root -p < schema.sql
```

### For Existing Old Database
**Use:** Migration files in `migrations/` folder (in order)

Only if you have an old database that needs updating:
1. `add_sex_column.sql` - Adds sex field to students
2. `add_timeout_column.sql` - Adds time_out to scan_history
3. `time_in_out_update.sql` - Updates to Time In/Time Out system
4. `update_scan_history_sp.sql` - Updates stored procedures

---

## 📂 File Descriptions

### Main Files (Keep These)

| File | Purpose | When to Use |
|------|---------|-------------|
| **schema.sql** | Complete database schema | ⭐ Fresh installations |
| **DEPLOYMENT_GUIDE.md** | Deployment instructions | Reference for setup |
| **SCHEMA_UPDATE_SUMMARY.md** | What's in schema.sql | Quick reference |

### Migration Files (Optional - Keep for History)

| File | Purpose | Status |
|------|---------|--------|
| `add_sex_column.sql` | Adds sex field | ✅ In schema.sql |
| `add_timeout_column.sql` | Adds time_out field | ✅ In schema.sql |
| `time_in_out_update.sql` | Time In/Out system | ✅ In schema.sql |
| `update_scan_history_sp.sql` | Procedure updates | ✅ In schema.sql |

**Why keep them?**
- Historical record of changes
- Useful for updating existing databases
- Documentation of schema evolution

### Archive Files (Can Delete)

| File | Purpose | Recommendation |
|------|---------|----------------|
| `dump.sql` | Database backup/export | ❌ Delete (60KB, not needed) |
| `create_admin_user.sql` | Manual admin creation | ⚠️ Keep if you need manual admin setup |
| `fix_admin_password.sql` | Password hash fix | ❌ Delete (handled by C# app) |
| `reset_admin_password.sql` | Password reset | ❌ Delete (handled by C# app) |
| `update_admin_hash.sql` | Update admin hash | ❌ Delete (handled by C# app) |
| `recreate_admin.sql` | Recreate admin user | ❌ Delete (handled by C# app) |

---

## 🗑️ Safe to Delete

You can safely delete these files without affecting functionality:

### Definitely Delete:
- ❌ `dump.sql` - Old database dump (60KB)
- ❌ `fix_admin_password.sql` - Redundant
- ❌ `reset_admin_password.sql` - Redundant
- ❌ `update_admin_hash.sql` - Redundant
- ❌ `recreate_admin.sql` - Redundant

### Consider Keeping:
- ✅ `create_admin_user.sql` - Useful for manual admin creation if needed

### Recommended to Keep (for history):
- ✅ Migration files - Move to `migrations/` folder
- ✅ Documentation files - Keep in root

---

## 📋 Recommended Actions

### Option 1: Organize (Recommended)
```powershell
# Move migration files to migrations folder
Move-Item add_sex_column.sql migrations/
Move-Item add_timeout_column.sql migrations/
Move-Item time_in_out_update.sql migrations/
Move-Item update_scan_history_sp.sql migrations/

# Move archive files to archive folder
Move-Item dump.sql archive/
Move-Item fix_admin_password.sql archive/
Move-Item reset_admin_password.sql archive/
Move-Item update_admin_hash.sql archive/
Move-Item recreate_admin.sql archive/

# Keep create_admin_user.sql in root (might be useful)
```

### Option 2: Delete Archive Files
```powershell
# Delete redundant files
Remove-Item dump.sql
Remove-Item fix_admin_password.sql
Remove-Item reset_admin_password.sql
Remove-Item update_admin_hash.sql
Remove-Item recreate_admin.sql
```

---

## 🎯 Quick Reference

### "I want to set up on a new device"
→ Use `schema.sql` only

### "I have an old database to update"
→ Use migration files in order

### "I need to create an admin user manually"
→ Use `create_admin_user.sql`

### "I want to clean up my Database folder"
→ Delete files in archive/ folder or move migrations to migrations/

---

## 📊 Database Schema Version

**Current Version:** 1.0.0  
**Last Updated:** 2025-11-21  
**Compatible With:** MySQL 5.7+, MySQL 8.0+

---

## ✅ What's in schema.sql

- ✅ 7 Tables (users, students, devices, scan_history, tokens, system_settings, system_logs)
- ✅ 5 Stored Procedures (including Time In/Time Out system)
- ✅ 4 Views (with attendance status tracking)
- ✅ 3 Triggers (audit trail)
- ✅ Sample Data (2 admins, 2 devices, 5 students, 5 scans)
- ✅ All latest features (sex field, time_out field, duplicate prevention)

---

## 📞 Need Help?

See `DEPLOYMENT_GUIDE.md` for:
- Step-by-step installation
- Troubleshooting
- Verification steps
- Complete schema documentation

---

**Last Updated:** 2025-11-21  
**Maintained By:** Student Attendance System Development Team
