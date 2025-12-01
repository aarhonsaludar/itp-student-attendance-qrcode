# Testing the Hybrid Offline Mode

## ✅ Migration Status: COMPLETED

The database has been successfully updated with:

- ✓ New columns added to `scan_history` table
- ✓ Stored procedure `sp_record_attendance_scan_secure` updated
- ✓ Views `vw_scans_pending_review` and `vw_daily_offline_scans` created

## Testing Steps

### 1. Test Online Mode (Normal)

**Steps:**

1. Make sure you have internet connection
2. Build and run your application
3. Scan a student QR code
4. Verify attendance is recorded

**Expected Result:**

- ✅ Attendance recorded successfully
- ✅ No warning messages
- ✅ `validation_status` = 'verified'
- ✅ `requires_review` = FALSE

**SQL Check:**

```sql
USE student_attendance_db;

SELECT
    scan_id,
    scan_datetime,
    validation_status,
    requires_review,
    client_time,
    server_time,
    time_drift_seconds
FROM scan_history
ORDER BY scan_id DESC
LIMIT 5;
```

---

### 2. Test Offline Mode

**Steps:**

1. **Disconnect from internet** (disable WiFi/unplug ethernet)
2. Build and run your application
3. Scan a student QR code
4. Check the message displayed

**Expected Result:**

- ✅ Attendance still recorded (not blocked)
- ⚠️ Warning message: "Offline Mode - No internet connection available"
- ⚠️ Message includes: "This record will be flagged for manual review"
- ✅ `validation_status` = 'offline_mode'
- ✅ `requires_review` = TRUE
- ✅ `client_time` = device time
- ✅ `server_time` = NULL

**SQL Check:**

```sql
USE student_attendance_db;

-- Check offline scans
SELECT * FROM vw_scans_pending_review
ORDER BY scan_datetime DESC;

-- Count offline scans
SELECT COUNT(*) as offline_count
FROM scan_history
WHERE validation_status = 'offline_mode';
```

---

### 3. Test Time Tampering Detection

**Steps:**

1. **Reconnect to internet**
2. **Change your PC date/time** (set it 1 day ahead or behind)
3. Build and run your application
4. Try to scan a student QR code

**Expected Result:**

- ❌ Attendance BLOCKED
- ❌ Error message about time tampering
- ❌ Shows client time vs server time
- ❌ No record created in database

**After Test:**

- **IMPORTANT:** Set your PC time back to automatic/correct time!

---

### 4. View Pending Reviews

**Query all scans needing review:**

```sql
USE student_attendance_db;

SELECT
    scan_id,
    scan_datetime,
    student_number,
    student_name,
    program,
    validation_status,
    client_time,
    server_time,
    notes
FROM vw_scans_pending_review
ORDER BY scan_datetime DESC;
```

---

### 5. Approve Offline Scans (Manual Review)

**After verifying legitimate offline scans:**

```sql
USE student_attendance_db;

-- Approve a specific scan
UPDATE scan_history
SET requires_review = FALSE,
    notes = CONCAT(IFNULL(notes, ''), '\nReviewed and approved by Admin on ', NOW())
WHERE scan_id = [SCAN_ID]
  AND validation_status = 'offline_mode';

-- Batch approve scans from a specific time period (e.g., known internet outage)
UPDATE scan_history
SET requires_review = FALSE,
    notes = CONCAT(IFNULL(notes, ''), '\nBatch approved - Known outage on ', NOW())
WHERE validation_status = 'offline_mode'
  AND scan_datetime BETWEEN '2025-11-29 08:00:00' AND '2025-11-29 10:00:00'
  AND requires_review = TRUE;
```

---

### 6. View Daily Offline Summary

```sql
USE student_attendance_db;

SELECT * FROM vw_daily_offline_scans
ORDER BY scan_date DESC
LIMIT 7;
```

---

## Troubleshooting

### Issue: "Column 'validation_status' doesn't exist"

**Solution:** Migration didn't run. Run it again:

```powershell
cd Database\migrations
.\run_migration_004.ps1
```

### Issue: "Procedure 'sp_record_attendance_scan_secure' doesn't exist"

**Solution:** Check which procedure is being used:

```sql
SHOW PROCEDURE STATUS WHERE Db = 'student_attendance_db';
```

### Issue: All scans showing as offline_mode even with internet

**Causes:**

1. Firewall blocking outbound connections to Google/TimeAPI
2. Proxy/corporate network blocking
3. Antivirus interfering

**Debug:**

- Test from PowerShell: `Test-NetConnection www.google.com -Port 443`
- Check TimeAPI: `Invoke-WebRequest https://timeapi.io/api/Time/current/zone?timeZone=Asia/Manila`

---

## Quick Reference

### Check System Status

```sql
-- Total scans by validation status
SELECT
    validation_status,
    COUNT(*) as count,
    COUNT(CASE WHEN requires_review THEN 1 END) as needs_review
FROM scan_history
GROUP BY validation_status;

-- Recent scans (last 24 hours)
SELECT
    scan_datetime,
    CONCAT(s.first_name, ' ', s.last_name) as student,
    sh.validation_status,
    sh.requires_review
FROM scan_history sh
JOIN students s ON sh.student_id = s.student_id
WHERE sh.scan_datetime >= DATE_SUB(NOW(), INTERVAL 24 HOUR)
ORDER BY sh.scan_datetime DESC;
```

---

## Next Steps After Testing

1. ✅ **Test all 3 scenarios** (online, offline, tampering)
2. ✅ **Verify database records** match expected behavior
3. ✅ **Document any issues** you encounter
4. ✅ **Train staff** on reviewing flagged scans
5. ✅ **Set up daily review process** for offline scans

---

**Migration Version:** 004  
**Date Completed:** November 29, 2025  
**Status:** ✅ READY FOR TESTING
