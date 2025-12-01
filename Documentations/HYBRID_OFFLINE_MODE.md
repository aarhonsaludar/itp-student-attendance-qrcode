# Hybrid Offline Mode - Time Validation Implementation

## Overview

The system now supports **hybrid offline mode** that balances security with availability. Students can still record attendance during internet outages, but these records are **flagged for manual review**.

## How It Works

### Online Mode (Normal Operation) ✅

1. Scanner validates device time against **internet sources** (Google, TimeAPI, Microsoft)
2. If time is synchronized (±2 minutes), attendance is recorded as **verified**
3. Record is immediately trusted - no review needed

### Offline Mode (No Internet) ⚠️

1. Scanner cannot reach internet time sources
2. **Attendance is still allowed** (device time is used)
3. Record is **flagged for manual review**
4. Database fields track:
   - `validation_status` = `offline_mode`
   - `requires_review` = `TRUE`
   - `client_time` = Device timestamp
   - `server_time` = `NULL` (no internet)

### Tampered Time (Blocked) ❌

1. Device time differs significantly from internet time
2. **Attendance is BLOCKED**
3. User sees error message explaining the issue

## Database Schema Changes

### New Columns in `scan_history` Table

```sql
validation_status VARCHAR(30) DEFAULT 'verified'
  -- Values: 'verified', 'offline_mode', 'tampered', 'network_error'

requires_review BOOLEAN DEFAULT FALSE
  -- TRUE = Needs manual verification

client_time DATETIME NULL
  -- Device/client system time at scan

server_time DATETIME NULL
  -- Internet/server time used for validation

time_drift_seconds INT NULL
  -- Difference between client and server (seconds)
```

## Viewing Flagged Records

### View Pending Reviews

```sql
SELECT * FROM vw_scans_pending_review
ORDER BY scan_datetime DESC;
```

Returns:

- Student details
- Scan timestamp
- Client vs server time comparison
- Validation status
- Notes

### Daily Offline Summary

```sql
SELECT * FROM vw_daily_offline_scans;
```

Returns:

- Date
- Total offline scans
- Unique students affected
- First/last offline scan times

## Manual Review Process

### 1. Check Offline Scans

```sql
SELECT
    scan_id,
    scan_datetime,
    student_number,
    student_name,
    client_time,
    notes
FROM vw_scans_pending_review
WHERE validation_status = 'offline_mode';
```

### 2. Verify Legitimacy

Check if:

- ✅ Known internet outage at that time
- ✅ Multiple students affected (system-wide issue)
- ✅ Time seems reasonable (during class hours)
- ❌ Only one student (suspicious)
- ❌ Unusual time (late night/weekend)

### 3. Approve or Flag

```sql
-- Approve legitimate offline scans
UPDATE scan_history
SET requires_review = FALSE,
    notes = CONCAT(notes, '\nReviewed and approved by [Admin Name] on ', NOW())
WHERE scan_id = [ID]
  AND validation_status = 'offline_mode';

-- Flag suspicious scans
UPDATE scan_history
SET notes = CONCAT(notes, '\n⚠️ FLAGGED: Potential time manipulation - Review with student')
WHERE scan_id = [ID];
```

## Migration Instructions

### Run the Migration

**Windows (PowerShell):**

```powershell
cd Database\migrations
.\run_migration_004.ps1
```

**Manual (MySQL Command Line):**

```bash
mysql -u root -p < migration_004_add_time_validation.sql
```

### Verify Installation

```sql
-- Check columns were added
DESCRIBE scan_history;

-- Check procedure was updated
SHOW CREATE PROCEDURE sp_record_attendance_scan_secure;

-- Check views were created
SELECT * FROM vw_scans_pending_review LIMIT 1;
```

## Code Changes

### TimeValidationService.cs

- Added `OfflineMode` status to `TimeValidationStatus` enum
- Added `RequiresManualReview` property to `TimeValidationResult`
- Modified `ValidateClientTimeAsync()` to allow offline scans with warnings

### ScanHistoryRepository.cs

- Updated `RecordAttendanceScanAsync()` to accept offline mode
- Added validation parameter passing to stored procedure
- Enhanced logging for offline scans

## Security Features Maintained

### ✅ Still Prevents Tampering

- **Online**: Full validation against internet time sources
- **Offline**: Flagged for review (can't bypass unnoticed)
- **Tampered**: Blocked entirely (date/time manipulation detected)

### ✅ Audit Trail

- All offline scans are logged
- Review status tracked in database
- Administrator actions recorded

### ✅ Transparent to Users

- Clear messaging about offline mode
- Warning displayed during offline scans
- No silent acceptance of unverified times

## Benefits

### For Students

- ✅ Attendance still works during internet outages
- ✅ No false rejections due to network issues
- ✅ Fair treatment (legitimate scans accepted)

### For Administrators

- ✅ Security maintained through review process
- ✅ Easy identification of suspicious patterns
- ✅ Audit trail for all scans
- ✅ Flexible approval workflow

### For the System

- ✅ Improved availability
- ✅ Better user experience
- ✅ Maintains data integrity
- ✅ Automated flagging reduces manual work

## Reports and Monitoring

### Generate Weekly Review Report

```sql
SELECT
    DATE(scan_datetime) as date,
    COUNT(*) as offline_scans,
    COUNT(CASE WHEN requires_review = TRUE THEN 1 END) as pending_review,
    COUNT(CASE WHEN requires_review = FALSE THEN 1 END) as approved
FROM scan_history
WHERE validation_status = 'offline_mode'
    AND scan_datetime >= DATE_SUB(CURDATE(), INTERVAL 7 DAY)
GROUP BY DATE(scan_datetime)
ORDER BY date DESC;
```

### Find Suspicious Patterns

```sql
-- Students with multiple offline scans (potential manipulation)
SELECT
    s.student_number,
    CONCAT(s.first_name, ' ', s.last_name) as name,
    COUNT(*) as offline_count,
    MIN(sh.scan_datetime) as first_offline,
    MAX(sh.scan_datetime) as last_offline
FROM scan_history sh
JOIN students s ON sh.student_id = s.student_id
WHERE sh.validation_status = 'offline_mode'
    AND sh.requires_review = TRUE
    AND sh.scan_datetime >= DATE_SUB(CURDATE(), INTERVAL 30 DAY)
GROUP BY s.student_id
HAVING offline_count > 3
ORDER BY offline_count DESC;
```

## Troubleshooting

### Issue: All scans showing as offline_mode

**Cause**: Internet connectivity problem on server  
**Solution**:

1. Check internet connection
2. Test: `ping www.google.com`
3. Verify firewall allows outbound HTTPS

### Issue: Legitimate scans being flagged

**Cause**: Known internet outage or network issue  
**Solution**:

1. Batch approve scans from that time period
2. Document the outage in notes

### Issue: Old scans not showing validation status

**Cause**: Migration only affects new scans  
**Solution**: This is normal - old scans didn't track validation

## Best Practices

1. **Review daily** - Check `vw_scans_pending_review` at end of each day
2. **Batch approve** - Approve multiple scans from same outage together
3. **Document decisions** - Always add notes when approving/flagging
4. **Monitor patterns** - Watch for repeated offline scans by same student
5. **Keep logs** - Export flagged scans before archiving

## Support

For issues or questions about the hybrid offline mode, contact the development team or refer to:

- `TimeValidationService.cs` - Validation logic
- `ScanHistoryRepository.cs` - Database integration
- `migration_004_add_time_validation.sql` - Database schema

---

**Version:** 1.0  
**Date:** November 29, 2025  
**Migration:** 004
