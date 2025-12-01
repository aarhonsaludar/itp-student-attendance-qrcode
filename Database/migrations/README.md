# Database Migrations - Quick Reference

## Current Active Migrations

### Migration 004: Exclude for_review Scans

**File:** `004_exclude_for_review_from_recent_scans.sql`  
**Status:** ✅ Applied (Nov 30, 2025)  
**Purpose:** Filters `for_review` scans from Recent Scan Activity in MainDashboard  
**Runner:** `Database/run_migration_004.bat` or `Database/run_migration_004.ps1`

## Utility Scripts

### 1. Check Database Objects

**File:** `check_database_objects.sql`  
**Purpose:** Lists all procedures, views, tables, and triggers  
**Usage:** Debugging and verification

```bash
mysql -u root -padmin student_attendance_db < Database/migrations/check_database_objects.sql
```

### 2. Cleanup Unused Objects

**File:** `cleanup_unused_objects.sql`  
**Purpose:** One-time removal of deprecated DB objects  
**Status:** Reference only (already applied)

### 3. Reset Students (Full)

**File:** `reset_students.sql`  
**Purpose:** Delete all students, scan history, and tokens for testing  
**⚠️ DESTRUCTIVE:** Clears all data

```bash
mysql -u root -padmin student_attendance_db < Database/migrations/reset_students.sql
```

### 4. Reset Students Only

**File:** `reset_students_only.sql`  
**Purpose:** Delete students only, keeps scan history for audit  
**⚠️ DESTRUCTIVE:** Clears student data

```bash
mysql -u root -padmin student_attendance_db < Database/migrations/reset_students_only.sql
```

## Migration History (Already Applied)

These features are now part of `schema.sql` and don't need separate migration:

| Feature                 | Status     | Incorporated In    |
| ----------------------- | ---------- | ------------------ |
| `sex` column            | ✅ Applied | schema.sql line 45 |
| `time_out` column       | ✅ Applied | schema.sql line 87 |
| `home_address` column   | ✅ Applied | schema.sql line 49 |
| Time validation columns | ✅ Applied | Stored procedure   |
| for_review status       | ✅ Applied | Migration 004      |

## New Database Setup

For a **fresh installation**, simply run:

```bash
mysql -u root -padmin < Database/schema.sql
```

This creates the complete database with all features.

## Existing Database Updates

If your database already exists and you need to apply Migration 004:

```bash
cd Database
.\run_migration_004.ps1
```

## Next Migration Number

**Next migration should be:** `005_*.sql`

When creating new migrations:

1. Name format: `00X_descriptive_name.sql`
2. Create corresponding runner: `run_migration_00X.ps1/.bat`
3. Place in `Database/migrations/`
4. Update this reference guide

## File Structure

```
Database/
├── schema.sql                          # Complete DB schema
├── run_migration_004.bat/.ps1          # Migration 004 runner
├── migrations/
│   ├── 004_exclude_for_review...sql    # Active migration
│   ├── check_database_objects.sql      # Utility
│   ├── cleanup_unused_objects.sql      # Reference
│   ├── reset_students.sql              # Testing utility
│   ├── reset_students_only.sql         # Testing utility
│   └── TESTING_GUIDE.md                # Documentation
└── MIGRATIONS_CLEANUP_REPORT.md        # Cleanup history
```

## Migration Best Practices

1. **Idempotent**: Migrations should check if changes already exist
2. **Reversible**: Document how to undo if needed
3. **Tested**: Test on development database first
4. **Documented**: Add comments explaining purpose
5. **Atomic**: One logical change per migration
6. **Backed Up**: Always backup before running migrations

## Troubleshooting

### Check if Migration 004 Applied

```sql
SELECT TABLE_NAME, TABLE_TYPE
FROM information_schema.TABLES
WHERE TABLE_SCHEMA = 'student_attendance_db'
AND TABLE_NAME = 'vw_recent_scans';
```

### Verify Recent Scans View Filter

```sql
SHOW CREATE VIEW vw_recent_scans;
-- Should contain: WHERE ... AND sh.status != 'for_review'
```

### Check Current Database Version

```sql
SELECT * FROM vw_recent_scans LIMIT 1;
-- If this works and excludes for_review, Migration 004 is applied
```

---

**Last Updated:** November 30, 2025  
**Active Migrations:** 1 (Migration 004)  
**Pending Migrations:** 0
