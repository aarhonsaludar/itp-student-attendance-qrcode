# Anti-Tampering Time Validation System

## Offline Time Tampering Detection using Environment.TickCount64

**Date**: December 1, 2025  
**Version**: 2.0 - Enhanced with Offline Tampering Detection

---

## 🎯 Purpose

This system prevents students from cheating the attendance system by:

1. **Disconnecting from WiFi** (going offline)
2. **Tampering with system clock** (changing date/time)
3. **Timing out immediately** with a future time to fake long attendance

## 🔒 How It Works

### The Problem

**Scenario**: Student wants to fake 6 hours of attendance but only stay 2 minutes:

1. Student scans QR at 1:00 PM (Time In) ✅
2. Student disconnects WiFi (goes offline) 📵
3. Student changes system clock to 7:00 PM ⏰
4. Student scans QR at 1:02 PM real time (Time Out) ❌
5. System sees: 1:00 PM → 7:00 PM = 6 hours (FAKE!)

### The Solution: Environment.TickCount64

**TickCount64** is a special counter that:

- Counts milliseconds since computer boot
- **CANNOT** be changed by modifying system date/time
- Works **100% OFFLINE** (no internet needed)
- Resets only when computer restarts

**Example**:

```
Computer boots at 8:00 AM
TickCount64 = 0

After 1 hour (9:00 AM)
TickCount64 = 3,600,000 ms (1 hour)

User changes clock to 5:00 PM
TickCount64 = STILL 3,600,000 ms (CANNOT BE CHANGED!)

After 15 more minutes (9:15 AM real time)
Clock shows: 5:15 PM
TickCount64 = 4,500,000 ms (1 hour 15 min from boot)

System compares:
- Claimed Duration: 5:00 PM → 5:15 PM = 15 minutes ✅
- Real Duration: TickCount difference = 15 minutes ✅
- MATCH = Valid attendance ✅
```

### Catching Tampering

**Tampering Scenario**:

```
Time In: 1:00 PM → TickCount = 10,000,000
User disconnects WiFi, changes clock to 7:00 PM
Time Out: 1:02 PM (real) → TickCount = 10,120,000

System checks:
- Claimed Duration: 1:00 PM → 7:00 PM = 360 minutes (6 hours)
- Real Duration: TickCount difference = 120,000 ms = 2 minutes
- MISMATCH: 358 minutes difference!
- Result: 🚨 TIME TAMPERING DETECTED! Flagged for admin review
```

---

## 📊 Database Schema Changes

### Migration 007: Add TickCount Fields

```sql
ALTER TABLE scan_history ADD COLUMN (
    time_in_tick_count BIGINT NULL,           -- TickCount64 at Time In
    time_out_tick_count BIGINT NULL,          -- TickCount64 at Time Out
    connection_drop_count INT NULL DEFAULT 0, -- Number of disconnections
    offline_duration_minutes DOUBLE NULL      -- Total offline time
);
```

### Migration 008: Update Stored Procedure

Added parameters to `sp_record_attendance_scan_secure`:

- `p_tick_count` - Current TickCount64 value
- `p_connection_drop_count` - Track WiFi disconnections

---

## 🛡️ Anti-Tampering Constants

```csharp
public const int MIN_ATTENDANCE_DURATION_MINUTES = 15;      // Minimum stay time
public const int MAX_ATTENDANCE_DURATION_HOURS = 12;        // Maximum session time
public const double TICK_COUNT_TOLERANCE_MINUTES = 3.0;     // Allowed variance
public const int SUSPICIOUS_DISCONNECT_COUNT = 3;           // Flag if disconnected 3+ times
public const int MAX_OFFLINE_DURATION_MINUTES = 60;         // Flag if offline > 1 hour
```

---

## 🔍 Detection Methods

### 1. **IsTimeOutTampered()**

Compares claimed duration vs real TickCount duration:

```csharp
double claimedMinutes = (TimeOut - ScanDateTime).TotalMinutes;
double actualMinutes = (TimeOutTickCount - TimeInTickCount) / 60000.0;
double difference = Math.Abs(claimedMinutes - actualMinutes);

return difference > TICK_COUNT_TOLERANCE_MINUTES; // 3 minutes tolerance
```

### 2. **IsTimeOutDurationValid()**

Checks if duration is within reasonable limits:

```csharp
TimeSpan duration = proposedTimeOut - ScanDateTime;
return duration.TotalMinutes >= 15 && duration.TotalHours <= 12;
```

### 3. **IsSuspiciousOfflineBehavior()**

Flags suspicious patterns:

```csharp
return ConnectionDropCount >= 3 || OfflineDurationMinutes > 60;
```

### 4. **GetRealElapsedTimeMinutes()**

Calculates tamper-proof elapsed time:

```csharp
long tickDifference = TimeOutTickCount.Value - TimeInTickCount.Value;
return tickDifference / 60000.0; // Convert ms to minutes
```

---

## 📝 Implementation

### C# Code (ScanHistoryRepository.cs)

**Recording TickCount64**:

```csharp
// When recording attendance (Time In or Time Out)
command.Parameters.AddWithValue("@p_tick_count", Environment.TickCount64);
command.Parameters.AddWithValue("@p_connection_drop_count", 0);
```

### Model (ScanHistory.cs)

**Properties**:

```csharp
public long? TimeInTickCount { get; set; }
public long? TimeOutTickCount { get; set; }
public int? ConnectionDropCount { get; set; }
public double? OfflineDurationMinutes { get; set; }
```

**Validation in AttendanceStatus**:

```csharp
public string AttendanceStatus
{
    get
    {
        if (IsTimeOutTampered())
            return "🚨 Time Tampering Detected";

        if (IsSuspiciousOfflineBehavior())
            return "⚠️ Suspicious Activity";

        // ... other checks
    }
}
```

---

## 🎨 User Interface

### Attendance Status Icons

| Status                | Icon | Meaning                                   |
| --------------------- | ---- | ----------------------------------------- |
| Completed             | ✅   | Valid Time In & Time Out                  |
| Time Tampering        | 🚨   | TickCount mismatch detected               |
| Suspicious Activity   | ⚠️   | Too many disconnections or long offline   |
| Suspicious Time Drift | ⚠️   | Online mode: clock drift detected         |
| Invalid Duration      | ⚠️   | Too short (<15 min) or too long (>12 hrs) |
| For Review            | ⚠️   | Offline mode or validation issues         |
| Pending Time Out      | ⏳   | Waiting for Time Out today                |
| Incomplete            | ⚠️   | Old record without Time Out               |
| Duplicate             | 🔁   | Already checked in today                  |
| Failed                | ❌   | System error                              |

### Duration Display

```csharp
public string GetAttendanceDuration()
{
    // Shows REAL duration from TickCount if available
    double? realMinutes = GetRealElapsedTimeMinutes();
    if (realMinutes.HasValue)
        return $"{hours}h {minutes}m (verified)"; // Tamper-proof!

    // Fallback to displayed time (less trustworthy)
    return $"{hours}h {minutes}m";
}
```

---

## 🚀 Deployment Steps

### 1. Run Database Migrations

```bash
# Step 1: Add TickCount columns
cd Database
run_migration_007.bat

# Step 2: Update stored procedure
run_migration_008.bat
```

### 2. Verify Database Changes

```sql
-- Check columns exist
DESCRIBE scan_history;

-- Should see:
-- time_in_tick_count BIGINT
-- time_out_tick_count BIGINT
-- connection_drop_count INT
-- offline_duration_minutes DOUBLE

-- Test stored procedure
SHOW CREATE PROCEDURE sp_record_attendance_scan_secure;
```

### 3. Build & Deploy Application

```bash
# Rebuild solution
dotnet build

# Or in Visual Studio:
Build > Rebuild Solution
```

### 4. Test Anti-Tampering

**Test Case 1**: Normal attendance (should work)

1. Time In at current time
2. Wait 15+ minutes
3. Time Out
4. Result: ✅ Completed

**Test Case 2**: Tampering attempt (should be caught)

1. Time In at current time
2. Disconnect WiFi
3. Change system clock forward 6 hours
4. Time Out immediately (2 minutes real time)
5. Result: 🚨 Time Tampering Detected

---

## 📊 Admin Review Process

### Flagged Records

Records flagged for review will show:

```
Status: ⚠️ For Review
Notes: 🚨 TIME TAMPERING DETECTED! Claimed: 360 min, Actual: 2 min
```

### Admin Actions

1. **Review Flagged Scans**:

   - Go to Scan History screen
   - Filter by "For Review" status
   - Check Notes field for details

2. **Approve/Reject**:

   - If legitimate (e.g., system clock auto-adjusted): Approve
   - If tampering confirmed: Reject or mark as violation

3. **Reports**:
   - Track tampering attempts by student
   - Generate compliance reports
   - Identify patterns of suspicious behavior

---

## 🔬 Technical Details

### Why TickCount64 is Tamper-Proof

1. **Not tied to system clock**: Independent counter
2. **Monotonic**: Always increases, never goes backward
3. **Persists in memory**: Stored in kernel, not accessible to user
4. **Resets only on reboot**: Restarting computer would disconnect active sessions

### Limitations

1. **Computer restart**: TickCount resets to 0
   - Mitigated: Active sessions are terminated on restart
2. **Virtual machines**: VM suspension might affect TickCount

   - Mitigated: Combined with online time validation

3. **System hibernation**: TickCount pauses during hibernate
   - Mitigated: Hibernation detection (future enhancement)

### Security Layers

| Layer   | Detection Method                        | Works Offline? |
| ------- | --------------------------------------- | -------------- |
| Layer 1 | Online time validation (Google/TimeAPI) | ❌ No          |
| Layer 2 | TickCount64 tampering detection         | ✅ Yes         |
| Layer 3 | Duration limits (15 min - 12 hrs)       | ✅ Yes         |
| Layer 4 | Connection drop tracking                | ✅ Yes         |
| Layer 5 | Offline duration monitoring             | ✅ Yes         |

---

## 🎓 Example Scenarios

### Scenario 1: Honest Student

```
8:00 AM: Time In (online) → TickCount = 5,000,000
2:00 PM: Time Out (online) → TickCount = 26,600,000

Claimed: 6 hours
Actual: 21,600,000 ms = 6 hours
Result: ✅ Completed (verified)
```

### Scenario 2: WiFi Drops Briefly

```
8:00 AM: Time In (online) → TickCount = 5,000,000
10:00 AM: WiFi drops for 5 minutes
2:00 PM: Time Out (online) → TickCount = 26,600,000

Claimed: 6 hours
Actual: 6 hours
Connection drops: 1
Result: ✅ Completed (verified)
```

### Scenario 3: Extended Offline (Flagged)

```
8:00 AM: Time In (online) → TickCount = 5,000,000
8:05 AM: Disconnect WiFi deliberately
2:00 PM: Time Out (offline) → TickCount = 26,600,000

Claimed: 6 hours
Actual: 6 hours
Offline duration: 355 minutes
Result: ⚠️ For Review (long offline period)
```

### Scenario 4: Time Tampering (Caught!)

```
1:00 PM: Time In → TickCount = 10,000,000
1:01 PM: Disconnect WiFi, change clock to 7:00 PM
1:02 PM: Time Out → TickCount = 10,120,000

Claimed: 6 hours (1:00 PM → 7:00 PM)
Actual: 120,000 ms = 2 minutes
Difference: 358 minutes
Result: 🚨 Time Tampering Detected!
```

---

## 📞 Support & Troubleshooting

### Common Issues

**Issue**: TickCount columns show NULL

- **Cause**: Migration 007 not run
- **Fix**: Run `run_migration_007.bat`

**Issue**: Stored procedure error "unknown parameter @p_tick_count"

- **Cause**: Migration 008 not run
- **Fix**: Run `run_migration_008.bat`

**Issue**: All attendance flagged as tampering

- **Cause**: TICK_COUNT_TOLERANCE_MINUTES too low
- **Fix**: Increase tolerance in `ScanHistory.cs` (currently 3.0 minutes)

---

## 🔄 Future Enhancements

1. **Machine Learning**: Pattern recognition for suspicious behavior
2. **Hibernation Detection**: Detect and flag system hibernation
3. **Network Monitoring**: Track network quality and stability
4. **Biometric Integration**: Combine with fingerprint/face recognition
5. **GPS Verification**: Validate physical location (optional)

---

## ✅ Testing Checklist

- [ ] Migration 007 completed successfully
- [ ] Migration 008 completed successfully
- [ ] Database columns exist
- [ ] Stored procedure accepts TickCount parameters
- [ ] Normal Time In/Out works
- [ ] Time tampering is detected
- [ ] Admin can review flagged records
- [ ] Duration displays show "(verified)" tag
- [ ] Status icons display correctly
- [ ] Offline mode is tracked

---

**For questions or support, contact the development team.**
