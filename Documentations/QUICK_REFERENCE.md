# 🛡️ Anti-Tampering Quick Reference Card

## 🎯 What This Prevents

**Problem**: Student disconnects WiFi, changes system clock, and fakes attendance time.

**Solution**: Environment.TickCount64 tracks REAL elapsed time (tamper-proof!).

---

## 🚀 Quick Deployment (3 Steps)

```bash
# Step 1: Add database columns
cd Database
run_migration_007.bat

# Step 2: Update stored procedure
run_migration_008.bat

# Step 3: Rebuild application
# Visual Studio: Build > Rebuild Solution
```

---

## 🔍 How It Works (Simple Explanation)

```
TickCount64 = Milliseconds since computer boot

Time In:  TickCount = 1,000,000 (computer on for ~16 min)
Time Out: TickCount = 1,900,000 (computer on for ~31 min)

Real elapsed time = 900,000 ms = 15 minutes ✅

If student changes clock but TickCount only increased by 2 minutes:
System sees: Claimed 6 hours, Actual 2 minutes = TAMPERING! 🚨
```

---

## 📊 Validation Rules

| Check               | Limit      | Result if Violated |
| ------------------- | ---------- | ------------------ |
| Min Duration        | 15 minutes | ❌ Blocked         |
| Max Duration        | 12 hours   | ⚠️ Flagged         |
| TickCount Tolerance | 3 minutes  | 🚨 Tampering!      |
| Disconnections      | 3 drops    | ⚠️ Suspicious      |
| Offline Time        | 60 minutes | ⚠️ Review          |

---

## 🎨 Status Icons

| Icon                       | Meaning                                 |
| -------------------------- | --------------------------------------- |
| ✅ Completed (verified)    | Valid attendance with tamper-proof time |
| 🚨 Time Tampering Detected | Clock manipulation caught!              |
| ⚠️ Suspicious Activity     | Too many disconnections or long offline |
| ⚠️ For Review              | Offline mode or needs verification      |
| ⏳ Pending Time Out        | Waiting for checkout                    |

---

## 🧪 Testing

### Test 1: Normal (Should Pass ✅)

1. Time In
2. Wait 15+ minutes
3. Time Out
4. **Result**: ✅ Completed (verified)

### Test 2: Tampering (Should Catch 🚨)

1. Time In
2. Disconnect WiFi
3. Change clock forward 6 hours
4. Time Out immediately (2 min real time)
5. **Result**: 🚨 Time Tampering Detected!

Run: `mysql < test_anti_tampering.sql`

---

## 📁 Files Added

```
Database/
  migrations/
    007_add_tickcount_anti_tampering.sql
    008_update_stored_procedure_tickcount.sql
  run_migration_007.bat
  run_migration_008.bat
  test_anti_tampering.sql

Documentations/
  ANTI_TAMPERING_TICKCOUNT_SYSTEM.md (Full docs)
  IMPLEMENTATION_SUMMARY.md (Summary)
```

---

## 🔧 Troubleshooting

**Problem**: TickCount columns NULL  
**Fix**: Run `run_migration_007.bat`

**Problem**: Stored procedure error  
**Fix**: Run `run_migration_008.bat`

**Problem**: Everything flagged as tampering  
**Fix**: Check TICK_COUNT_TOLERANCE_MINUTES in ScanHistory.cs

---

## 💡 Key Code Snippets

### Check if tampering occurred:

```csharp
if (scanHistory.IsTimeOutTampered())
{
    // Caught! Flag for review
    scanHistory.RequiresReview = true;
    scanHistory.Notes = "Time tampering detected";
}
```

### Get real duration:

```csharp
double? realMinutes = scanHistory.GetRealElapsedTimeMinutes();
// Returns actual elapsed time from TickCount (tamper-proof)
```

### Validate before saving:

```csharp
string validation = scanHistory.GetTimeOutValidationMessage(timeOut);
if (validation != "✅ Valid")
{
    // Handle invalid/suspicious attendance
}
```

---

## 📊 Database Queries

### Check recent scans with tampering detection:

```sql
SELECT
    student_number,
    scan_datetime AS time_in,
    time_out,
    TIMESTAMPDIFF(MINUTE, scan_datetime, time_out) AS claimed_min,
    ROUND((time_out_tick_count - time_in_tick_count) / 60000.0, 1) AS actual_min,
    CASE
        WHEN ABS(TIMESTAMPDIFF(MINUTE, scan_datetime, time_out) -
             ((time_out_tick_count - time_in_tick_count) / 60000.0)) > 3
        THEN '🚨 TAMPERING'
        ELSE '✅ Valid'
    END AS status
FROM scan_history
WHERE DATE(scan_datetime) = CURDATE()
ORDER BY scan_datetime DESC;
```

---

## ✅ Pre-Deployment Checklist

- [ ] Migrations 007 & 008 completed
- [ ] Test script passes
- [ ] Normal attendance works
- [ ] Tampering detection works
- [ ] No compilation errors
- [ ] Status icons display
- [ ] Duration shows "(verified)"

---

## 📞 Need Help?

1. Read: `ANTI_TAMPERING_TICKCOUNT_SYSTEM.md`
2. Run: `test_anti_tampering.sql`
3. Check: scan_history table for TickCount values

---

**Implementation Date**: December 1, 2025  
**Status**: ✅ Ready for Deployment
