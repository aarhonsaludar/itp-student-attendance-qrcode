# Bug Fix: IndexOutOfRangeException in MySqlConnector

**Date:** December 1, 2025  
**Issue:** Multiple `System.IndexOutOfRangeException` errors during QR scanning with OTP verification

## Root Cause

The application was attempting to read database columns that didn't exist, causing `IndexOutOfRangeException` errors:

1. **Missing Column in Model:** `ReviewStatus` property existed in the `ScanHistory` model but had no corresponding database column
2. **Incorrect Column Name:** Code was looking for `time_in` column which doesn't exist (actual column is `scan_datetime`)
3. **View Missing Columns:** The `vw_recent_scans` database view was missing validation-related columns that were added in migrations 004 and 005

## Symptoms

- Multiple `IndexOutOfRangeException` errors thrown during QR scanning
- Errors occurred when loading scan history data
- Particularly affected the main dashboard's "Recent Scan Activity" display

## Files Modified

### 1. `Models\ScanHistory.cs`

**Change:** Removed the non-existent `ReviewStatus` property

```csharp
// REMOVED:
public string ReviewStatus { get; set; }
```

### 2. `Data\ScanHistoryRepository.cs`

**Change:** Updated `MapScanHistory` method to only reference existing columns

**Removed:**

- `reviewStatusOrdinal` variable
- `timeInOrdinal` variable (replaced with using only `scanDatetimeOrdinal`)
- Reference to `ReviewStatus` in return statement

### 3. Database Migration 006

**New File:** `Database\migrations\006_update_vw_recent_scans_with_validation_columns.sql`

**Purpose:** Updated `vw_recent_scans` view to include validation columns:

- `validation_status`
- `requires_review`
- `client_time`
- `server_time`
- `time_drift_seconds`
- `time_in_validation_mode`
- `time_out_validation_mode`

**Also removed:** The `time_in` alias column (was causing confusion since it's just an alias for `scan_datetime`)

## Database Schema Reference

The `scan_history` table actual columns:

```sql
- scan_id
- student_id
- device_id
- scan_type
- scan_data
- scan_datetime          -- (NOT time_in)
- time_out
- scan_purpose
- location
- status                 -- (values: 'success', 'failed', 'duplicate', 'for_review')
- notes
- created_at
- validation_status
- time_in_validation_mode
- time_out_validation_mode
- requires_review
- client_time
- server_time
- time_drift_seconds
```

**Note:** There is NO `time_in` column and NO `review_status` column in the database.

## How to Apply Fix

### If Already Applied Code Changes:

The code changes have been automatically applied. You need to run the database migration:

```powershell
# Option 1: Run PowerShell script
.\Database\run_migration_006.ps1

# Option 2: Run batch file
.\Database\run_migration_006.bat

# Option 3: Run manually
Get-Content .\Database\migrations\006_update_vw_recent_scans_with_validation_columns.sql | mysql -u root -padmin
```

### Verify Fix:

After applying the migration, restart the application and test QR scanning with OTP verification. The `IndexOutOfRangeException` errors should no longer appear.

## Testing Checklist

- [x] Code changes applied to `ScanHistory.cs`
- [x] Code changes applied to `ScanHistoryRepository.cs`
- [x] Migration 006 created
- [x] Migration 006 executed on database
- [ ] Application restarted
- [ ] QR scan with OTP verification tested
- [ ] No `IndexOutOfRangeException` errors in debug output
- [ ] Recent scan activity displays correctly on dashboard

## Additional Notes

This issue was introduced when:

1. Migration 004 and 005 added new validation columns to the `scan_history` table
2. The `vw_recent_scans` view was not updated to include these new columns
3. The C# model had a `ReviewStatus` property that never had a corresponding database column

The fix ensures that:

- The C# model only contains properties that map to actual database columns
- Database views include all columns that the application expects to read
- Column name mapping is correct (`scan_datetime` not `time_in`)
