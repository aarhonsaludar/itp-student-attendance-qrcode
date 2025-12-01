# 🚀 Anti-Tampering System Deployment Checklist

## Pre-Deployment Preparation

### ✅ Backup Current System

- [ ] Backup database: `mysqldump -u root -p student_attendance_db > backup_$(date +%Y%m%d).sql`
- [ ] Backup application folder
- [ ] Document current version/commit hash
- [ ] Test backup restoration procedure

---

## Phase 1: Database Migration (10-15 minutes)

### Step 1: Migration 007 - Add TickCount Columns

```bash
cd Database
run_migration_007.bat
```

**Verification**:

```sql
DESCRIBE scan_history;
```

**Expected Output**:

- [x] `time_in_tick_count` BIGINT exists
- [x] `time_out_tick_count` BIGINT exists
- [x] `connection_drop_count` INT exists
- [x] `offline_duration_minutes` DOUBLE exists

**If Failed**:

- Check MySQL connection
- Verify database user permissions
- Review error messages
- Restore from backup if needed

---

### Step 2: Migration 008 - Update Stored Procedure

```bash
run_migration_008.bat
```

**Verification**:

```sql
SHOW CREATE PROCEDURE sp_record_attendance_scan_secure;
```

**Expected Output**:

- [x] Procedure accepts `p_tick_count BIGINT` parameter
- [x] Procedure accepts `p_connection_drop_count INT` parameter
- [x] Procedure stores TickCount values

**If Failed**:

- Check syntax errors in migration file
- Verify stored procedure dropped successfully
- Review error messages
- Restore from backup if needed

---

### Step 3: Test Database Changes

```bash
mysql -u root -p student_attendance_db < test_anti_tampering.sql
```

**Expected Results**:

- [x] Test Time In succeeds
- [x] Test Time Out succeeds
- [x] Tampering detection works (shows "🚨 TIME TAMPERING DETECTED!")
- [x] TickCount values are stored correctly

---

## Phase 2: Application Deployment (5-10 minutes)

### Step 1: Build Application

```bash
# Visual Studio
Build > Rebuild Solution

# Or command line
dotnet build --configuration Release
```

**Verification**:

- [x] Build succeeds with 0 errors
- [x] No warnings related to ScanHistory.cs or ScanHistoryRepository.cs
- [x] Output folder contains updated .exe

---

### Step 2: Deploy Application Files

- [ ] Stop running application instances
- [ ] Copy new .exe to production folder
- [ ] Copy any updated dependencies
- [ ] Verify file permissions

---

## Phase 3: Functional Testing (15-20 minutes)

### Test Case 1: Normal Attendance (MUST PASS)

**Steps**:

1. Launch application
2. Scan student QR code (Time In)
3. Wait 15+ minutes
4. Scan same QR code (Time Out)

**Expected Results**:

- [x] Time In successful
- [x] Time Out successful after 15+ minutes
- [x] Status shows: ✅ Completed (verified)
- [x] Duration displays correctly with "(verified)" tag
- [x] Database has TickCount values populated

**SQL Verification**:

```sql
SELECT
    student_number,
    scan_datetime,
    time_out,
    time_in_tick_count,
    time_out_tick_count,
    ((time_out_tick_count - time_in_tick_count) / 60000.0) AS actual_minutes
FROM scan_history
WHERE scan_id = (SELECT MAX(scan_id) FROM scan_history);
```

---

### Test Case 2: Too Quick Time Out (MUST FAIL)

**Steps**:

1. Scan student QR code (Time In)
2. Immediately scan again (Time Out) - less than 15 minutes

**Expected Results**:

- [x] Time In successful
- [x] Time Out BLOCKED or FLAGGED
- [x] Message shows: "Time Out too soon. Minimum: 15 min"
- [x] Status: ⚠️ For Review or blocked

---

### Test Case 3: Time Tampering Detection (MUST CATCH)

**Steps**:

1. Scan student QR code (Time In)
2. Disconnect WiFi/Internet
3. Change system clock forward 6 hours
4. Scan same QR code (Time Out) - only 2 minutes real time passed

**Expected Results**:

- [x] Time In successful with TickCount captured
- [x] Time Out recorded but FLAGGED
- [x] Status shows: 🚨 Time Tampering Detected
- [x] Notes field contains: "TIME TAMPERING DETECTED! Claimed: ~360 min, Actual: ~2 min"
- [x] RequiresReview = TRUE
- [x] Admin can see flagged record in Scan History

**SQL Verification**:

```sql
SELECT
    student_number,
    TIMESTAMPDIFF(MINUTE, scan_datetime, time_out) AS claimed_minutes,
    ROUND((time_out_tick_count - time_in_tick_count) / 60000.0, 1) AS actual_minutes,
    ABS(TIMESTAMPDIFF(MINUTE, scan_datetime, time_out) -
        ((time_out_tick_count - time_in_tick_count) / 60000.0)) AS difference,
    status,
    notes
FROM scan_history
WHERE scan_id = (SELECT MAX(scan_id) FROM scan_history);
```

**Expected SQL Results**:

- claimed_minutes: ~360 (6 hours)
- actual_minutes: ~2 (2 minutes)
- difference: ~358 (way over 3 min tolerance)
- status: 'for_review'
- notes: Contains "TIME TAMPERING DETECTED"

---

### Test Case 4: Offline Mode (Legitimate)

**Steps**:

1. Disconnect internet BEFORE Time In
2. Scan student QR code (Time In) - offline
3. Keep offline for 20 minutes
4. Scan same QR code (Time Out) - still offline

**Expected Results**:

- [x] Time In successful with offline validation mode
- [x] Time Out successful with offline validation mode
- [x] TickCount difference matches actual time (~20 minutes)
- [x] Status: ✅ Completed (verified) or ⚠️ For Review (legitimate offline)
- [x] No tampering detected (TickCount difference < 3 min tolerance)

---

### Test Case 5: Admin Review Process

**Steps**:

1. Create a flagged record (Test Case 3)
2. Open Scan History screen as admin
3. Filter by "For Review" status

**Expected Results**:

- [x] Flagged record appears in list
- [x] Status icon shows 🚨 or ⚠️
- [x] Duration shows actual vs claimed time
- [x] Notes field shows tampering details
- [x] Admin can approve/reject

---

## Phase 4: User Acceptance Testing (1-2 days)

### Day 1: Limited Rollout

- [ ] Deploy to 1-2 scanner devices only
- [ ] Monitor for false positives
- [ ] Check database TickCount values being stored
- [ ] Verify no performance issues

### Day 2: Full Rollout

- [ ] Deploy to all scanner devices
- [ ] Monitor flagged records
- [ ] Train admins on reviewing flagged scans
- [ ] Document any issues

---

## Phase 5: Monitoring & Validation (1 week)

### Daily Checks

- [ ] Review flagged scans for patterns
- [ ] Check for false positives
- [ ] Verify TickCount values are reasonable
- [ ] Monitor system performance
- [ ] Check database size growth

### Weekly Report

- [ ] Total scans processed
- [ ] Number of tampering attempts detected
- [ ] Number of legitimate offline scans
- [ ] False positive rate
- [ ] System stability

---

## Rollback Plan (If Issues Occur)

### Emergency Rollback Steps

1. **Stop application**
2. **Restore previous application version**
3. **Rollback database** (if needed):

   ```sql
   -- Remove new columns (optional, keeps old data)
   ALTER TABLE scan_history
   DROP COLUMN time_in_tick_count,
   DROP COLUMN time_out_tick_count,
   DROP COLUMN connection_drop_count,
   DROP COLUMN offline_duration_minutes;

   -- Restore old stored procedure from backup
   ```

4. **Test basic functionality**
5. **Document rollback reason**
6. **Schedule fix and redeployment**

---

## Post-Deployment Documentation

### Update Documentation

- [ ] Update system manual with new features
- [ ] Document tampering detection process
- [ ] Create admin guide for reviewing flagged scans
- [ ] Update troubleshooting guide

### Train Staff

- [ ] Admin training on tampering detection
- [ ] How to review flagged scans
- [ ] How to interpret TickCount values
- [ ] Escalation procedures for confirmed tampering

---

## Success Criteria

### Must Have (Critical)

- [x] Database migrations successful
- [x] Application builds without errors
- [x] Normal attendance works (Test Case 1)
- [x] Tampering detection works (Test Case 3)
- [x] No data loss
- [x] System performance acceptable

### Should Have (Important)

- [x] Duration limits enforced (Test Case 2)
- [x] Offline mode works (Test Case 4)
- [x] Admin review process works (Test Case 5)
- [x] TickCount values stored correctly
- [x] Status icons display correctly

### Nice to Have (Optional)

- [ ] Connection drop tracking functional
- [ ] Offline duration calculation accurate
- [ ] Detailed audit logs
- [ ] Performance metrics collected

---

## Sign-Off

### Technical Team

- [ ] Database Administrator: ********\_******** Date: **\_\_\_**
- [ ] Developer: ********\_******** Date: **\_\_\_**
- [ ] QA Tester: ********\_******** Date: **\_\_\_**

### Management

- [ ] IT Manager: ********\_******** Date: **\_\_\_**
- [ ] System Administrator: ********\_******** Date: **\_\_\_**

---

## Contact Information

**For Issues During Deployment**:

- Database Issues: [DBA Contact]
- Application Issues: [Developer Contact]
- Infrastructure Issues: [System Admin Contact]

**Emergency Rollback Authority**: [IT Manager]

---

**Deployment Date**: ******\_\_\_\_******  
**Deployed By**: ******\_\_\_\_******  
**Version**: Anti-Tampering v2.0 (TickCount64)  
**Status**: [ ] Successful [ ] Rolled Back [ ] Partial

---

## Notes & Observations

_Record any issues, observations, or deviations from the plan:_

```
Date: ________
Time: ________
Issue:




Resolution:




```
