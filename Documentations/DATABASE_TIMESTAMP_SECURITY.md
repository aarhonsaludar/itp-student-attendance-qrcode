# Secure QR-Code Based Attendance System with Database Timestamp Control

## 🔒 Security Overview

This system implements a **tamper-proof attendance tracking mechanism** where timestamps are generated **EXCLUSIVELY by the database server**, not by client devices or external APIs. This prevents students from manipulating attendance records by changing their device's system clock.

---

## 🎯 Key Security Principles

### ✅ What is Secure

- ✓ Database server generates ALL timestamps using `NOW()` or `CURRENT_TIMESTAMP`
- ✓ Client only sends student ID (extracted from QR code)
- ✓ Database returns the generated timestamp to client for display only
- ✓ No client-side time influence on attendance records

### ❌ What is Prevented

- ✗ Client cannot submit timestamps
- ✗ Client cannot manipulate time by changing device clock
- ✗ External APIs are not needed for time synchronization
- ✗ No reliance on client system time for attendance logging

---

## 📊 System Architecture

```
┌─────────────────────────────────────────────────────────────────────┐
│                    SECURE ATTENDANCE WORKFLOW                       │
└─────────────────────────────────────────────────────────────────────┘

┌──────────────┐     ┌──────────────┐     ┌──────────────┐     ┌──────────────┐
│   Student    │     │    Client    │     │   Database   │     │  Attendance  │
│ Scans QR Code│────▶│ Application  │────▶│    Server    │────▶│   Record     │
└──────────────┘     └──────────────┘     └──────────────┘     └──────────────┘
                            │                      │
                            │  1. Decode QR        │
                            │  2. Extract ID       │
                            │  3. Send ID ONLY     │
                            │                      │
                            │                      │  4. Generate NOW()
                            │                      │  5. Insert Record
                            │                      │  6. Return Timestamp
                            │                      │
                            │  7. Display Time     │
                            │◀─────────────────────│
                            │  (Read-Only)         │
```

### Detailed Flow

```
┌─────────────────────────────────────────────────────────────────────────────┐
│  STEP-BY-STEP SECURE TIMESTAMP FLOW                                        │
└─────────────────────────────────────────────────────────────────────────────┘

1️⃣ QR CODE SCAN
   Student Device: Scans QR code containing student ID
   ↓

2️⃣ DECODE QR DATA
   Client App: Extracts student ID from QR code
   Example: "ID:2024-STU-0001|Name:John Smith|..."
   ↓

3️⃣ SEND TO API (ID ONLY)
   HTTP POST /api/attendance
   {
     "student_id": "2024-STU-0001",
     "device_id": 1,
     "location": "Building A"
   }
   ❌ NO timestamp parameter sent!
   ↓

4️⃣ DATABASE GENERATES TIMESTAMP
   Stored Procedure: sp_record_attendance_scan_secure

   INSERT INTO scan_history (
     student_id,
     scan_datetime,     ← NOW() - Database time!
     time_out,
     device_id,
     location
   ) VALUES (
     @student_id,
     NOW(),             ← CRITICAL: Database server time
     NULL,
     @device_id,
     @location
   );
   ↓

5️⃣ RETURN DATABASE TIMESTAMP
   Database → API Response:
   {
     "success": true,
     "scan_type": "TIME_IN",
     "timestamp": "2025-11-28 14:30:45",    ← From database
     "time_in": "2025-11-28 14:30:45",
     "student_name": "John Smith",
     "message": "Time In recorded successfully"
   }
   ↓

6️⃣ DISPLAY CONFIRMATION
   Client App: Shows database timestamp to user
   "✓ Time In recorded at 14:30:45 (Database Server Time)"

   ✓ User can see the time
   ❌ User cannot influence the time
```

---

## 🛠️ Implementation Details

### 1. Database Schema

The `scan_history` table stores attendance records with server-generated timestamps:

```sql
CREATE TABLE scan_history (
    scan_id INT AUTO_INCREMENT PRIMARY KEY,
    student_id INT NOT NULL,
    device_id INT,
    scan_type ENUM('QR', 'MANUAL') DEFAULT 'QR',
    scan_data TEXT NOT NULL,
    scan_datetime DATETIME DEFAULT CURRENT_TIMESTAMP,  -- ← Server time
    time_out DATETIME NULL,                            -- ← Server time
    scan_purpose ENUM('attendance', 'identification', 'verification') DEFAULT 'attendance',
    location VARCHAR(100),
    status ENUM('success', 'failed', 'duplicate') DEFAULT 'success',
    notes TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,    -- ← Server time
    FOREIGN KEY (student_id) REFERENCES students(student_id) ON DELETE CASCADE,
    FOREIGN KEY (device_id) REFERENCES devices(device_id) ON DELETE SET NULL,
    INDEX idx_student_scan (student_id, scan_datetime),
    INDEX idx_scan_date (scan_datetime)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
```

### 2. Secure Stored Procedure

Location: `Database/migrations/003_secure_timestamp_attendance.sql`

```sql
CREATE PROCEDURE sp_record_attendance_scan_secure(
    IN p_scan_data TEXT,
    IN p_device_id INT,
    IN p_location VARCHAR(100),
    OUT p_result VARCHAR(200),
    OUT p_student_name VARCHAR(200),
    OUT p_student_number VARCHAR(50),
    OUT p_scan_type VARCHAR(20),
    OUT p_timestamp DATETIME,      -- ← Returns DB timestamp
    OUT p_time_in DATETIME,        -- ← Returns DB timestamp
    OUT p_time_out DATETIME        -- ← Returns DB timestamp
)
BEGIN
    DECLARE v_current_timestamp DATETIME;

    -- ✓ CRITICAL: Capture database server time
    SET v_current_timestamp = NOW();

    -- Find student by QR code
    -- Check for duplicate scans
    -- Determine if TIME_IN or TIME_OUT

    -- Insert with database timestamp
    INSERT INTO scan_history (
        student_id, device_id, scan_data,
        scan_datetime,          -- ← Database time
        time_out,
        location, status
    ) VALUES (
        v_student_id, p_device_id, p_scan_data,
        v_current_timestamp,    -- ← Database NOW()
        NULL,
        p_location, 'success'
    );

    -- Return the database-generated timestamp
    SET p_timestamp = v_current_timestamp;
    SET p_time_in = v_current_timestamp;
    SET p_time_out = NULL;
END
```

### 3. Repository Layer (C#)

Location: `Data/ScanHistoryRepository.cs`

```csharp
/// <summary>
/// CRITICAL: Client sends ONLY student ID, database generates timestamp
/// </summary>
public async Task<(bool success, string message, string scanType,
    DateTime? timestamp, DateTime? timeIn, DateTime? timeOut)>
    RecordAttendanceScanAsync(string qrData, int deviceId, string location = null)
{
    using (var command = new MySqlCommand("sp_record_attendance_scan_secure", connection))
    {
        command.CommandType = CommandType.StoredProcedure;

        // ✓ INPUT: Only student data, NO timestamp
        command.Parameters.AddWithValue("@p_scan_data", qrData);
        command.Parameters.AddWithValue("@p_device_id", deviceId);
        command.Parameters.AddWithValue("@p_location", location ?? DBNull.Value);

        // ✓ OUTPUT: Database returns its generated timestamps
        var timestampParam = new MySqlParameter("@p_timestamp", MySqlDbType.DateTime)
            { Direction = ParameterDirection.Output };
        var timeInParam = new MySqlParameter("@p_time_in", MySqlDbType.DateTime)
            { Direction = ParameterDirection.Output };
        var timeOutParam = new MySqlParameter("@p_time_out", MySqlDbType.DateTime)
            { Direction = ParameterDirection.Output };

        command.Parameters.Add(timestampParam);
        command.Parameters.Add(timeInParam);
        command.Parameters.Add(timeOutParam);

        await command.ExecuteNonQueryAsync();

        // ✓ Extract database timestamps
        DateTime? timestamp = timestampParam.Value != DBNull.Value
            ? (DateTime?)timestampParam.Value : null;
        DateTime? timeIn = timeInParam.Value != DBNull.Value
            ? (DateTime?)timeInParam.Value : null;
        DateTime? timeOut = timeOutParam.Value != DBNull.Value
            ? (DateTime?)timeOutParam.Value : null;

        return (success, message, scanType, timestamp, timeIn, timeOut);
    }
}
```

### 4. Client Application (QRScannerForm.cs)

```csharp
private async Task ProcessQRScanAsync(string qrData)
{
    // ✓ Send only QR data to database
    var (success, message, scanType, timestamp, timeIn, timeOut) =
        await scanHistoryRepository.RecordAttendanceScanAsync(
            qrData: qrData,
            deviceId: DEFAULT_DEVICE_ID,
            location: DEFAULT_LOCATION
        );

    if (success)
    {
        // ✓ CRITICAL: Display ONLY database timestamp, never DateTime.Now
        string dbTime = timestamp.HasValue
            ? timestamp.Value.ToString("HH:mm:ss")
            : "Unknown";

        MessageBox.Show(
            $"Time In Successfully Recorded\n\n" +
            $"Database Server Time: {dbTime}\n" +
            $"⚠️ Timestamp generated by database server (tamper-proof)",
            "✓ Time In Success",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information
        );
    }
}
```

---

## 🌐 Backend Implementation Examples

### ASP.NET Core Web API

```csharp
[ApiController]
[Route("api/[controller]")]
public class AttendanceController : ControllerBase
{
    private readonly IAttendanceRepository _repository;

    [HttpPost("scan")]
    public async Task<IActionResult> RecordScan([FromBody] ScanRequest request)
    {
        // ✓ Client sends only student ID
        var result = await _repository.RecordAttendanceAsync(
            studentId: request.StudentId,
            deviceId: request.DeviceId,
            location: request.Location
        );

        // ✓ Return database-generated timestamp
        return Ok(new {
            success = result.Success,
            timestamp = result.Timestamp,  // From database NOW()
            scanType = result.ScanType,
            message = result.Message
        });
    }
}

// Repository implementation
public async Task<AttendanceResult> RecordAttendanceAsync(
    string studentId, int deviceId, string location)
{
    using var connection = new MySqlConnection(_connectionString);
    using var command = new MySqlCommand("sp_record_attendance_scan_secure", connection);

    command.CommandType = CommandType.StoredProcedure;

    // Input: Student ID only
    command.Parameters.AddWithValue("@p_student_id", studentId);
    command.Parameters.AddWithValue("@p_device_id", deviceId);
    command.Parameters.AddWithValue("@p_location", location);

    // Output: Database timestamp
    var timestampParam = command.Parameters.Add("@p_timestamp", MySqlDbType.DateTime);
    timestampParam.Direction = ParameterDirection.Output;

    await connection.OpenAsync();
    await command.ExecuteNonQueryAsync();

    return new AttendanceResult {
        Timestamp = (DateTime)timestampParam.Value  // Database time
    };
}
```

### PHP Laravel

```php
// Controller
public function recordScan(Request $request)
{
    $validated = $request->validate([
        'student_id' => 'required|string',
        'device_id' => 'required|integer',
        'location' => 'nullable|string'
    ]);

    // Call stored procedure
    $result = DB::select('CALL sp_record_attendance_scan_secure(?, ?, ?, @result, @timestamp)', [
        $validated['student_id'],
        $validated['device_id'],
        $validated['location'] ?? null
    ]);

    // Get output parameters
    $output = DB::select('SELECT @result as result, @timestamp as timestamp')[0];

    return response()->json([
        'success' => str_starts_with($output->result, 'SUCCESS'),
        'timestamp' => $output->timestamp,  // Database time
        'message' => $output->result
    ]);
}

// Raw PDO approach
$stmt = $pdo->prepare("CALL sp_record_attendance_scan_secure(?, ?, ?, @result, @timestamp)");
$stmt->execute([$studentId, $deviceId, $location]);

$result = $pdo->query("SELECT @result, @timestamp")->fetch();
```

### Node.js with MySQL2

```javascript
const mysql = require("mysql2/promise");

app.post("/api/attendance/scan", async (req, res) => {
  const { student_id, device_id, location } = req.body;

  const connection = await mysql.createConnection(dbConfig);

  try {
    // Call stored procedure
    const [rows] = await connection.execute(
      "CALL sp_record_attendance_scan_secure(?, ?, ?, @result, @timestamp, @time_in, @time_out)",
      [student_id, device_id, location || null]
    );

    // Get output parameters
    const [output] = await connection.execute(
      "SELECT @result as result, @timestamp as timestamp, @time_in as timeIn, @time_out as timeOut"
    );

    res.json({
      success: output[0].result.startsWith("SUCCESS"),
      timestamp: output[0].timestamp, // Database server time
      timeIn: output[0].timeIn,
      timeOut: output[0].timeOut,
      message: output[0].result,
    });
  } finally {
    await connection.end();
  }
});
```

### Python Flask with MySQL

```python
from flask import Flask, request, jsonify
import mysql.connector

@app.route('/api/attendance/scan', methods=['POST'])
def record_scan():
    data = request.get_json()
    student_id = data['student_id']
    device_id = data['device_id']
    location = data.get('location')

    conn = mysql.connector.connect(**db_config)
    cursor = conn.cursor()

    # Call stored procedure
    args = (student_id, device_id, location, None, None, None, None)
    result = cursor.callproc('sp_record_attendance_scan_secure', args)

    # Get output parameters
    cursor.execute("SELECT @_sp_record_attendance_scan_secure_3 as result, "
                   "@_sp_record_attendance_scan_secure_4 as timestamp")
    output = cursor.fetchone()

    cursor.close()
    conn.close()

    return jsonify({
        'success': output[0].startswith('SUCCESS'),
        'timestamp': output[1].isoformat(),  # Database server time
        'message': output[0]
    })
```

---

## 🔐 Security Features

### 1. Time Manipulation Prevention

| Attack Vector           | Prevention Method                 |
| ----------------------- | --------------------------------- |
| Change device clock     | ✓ Client time never used          |
| Backdated attendance    | ✓ Database uses NOW() only        |
| Future-dated attendance | ✓ Database validates date         |
| API timestamp injection | ✓ No timestamp parameter accepted |
| Replay attacks          | ✓ 10-second cooldown enforced     |

### 2. Database-Level Security

```sql
-- All timestamps use database server time
SET v_current_timestamp = NOW();

-- Duplicate prevention (10-second cooldown)
SELECT scan_datetime INTO v_recent_scan
FROM scan_history
WHERE student_id = v_student_id
  AND scan_datetime > DATE_SUB(NOW(), INTERVAL 10 SECOND);

-- Today's attendance check
SET v_today_start = DATE(NOW());
SET v_today_end = DATE_ADD(v_today_start, INTERVAL 1 DAY);
```

### 3. Audit Trail

Every scan is logged with:

- ✓ Database-generated timestamp
- ✓ Device ID
- ✓ Location
- ✓ Student ID
- ✓ Scan type (TIME_IN, TIME_OUT)
- ✓ Status (success, duplicate, failed)

---

## 📱 Client Responsibilities

### What Client MUST Do:

1. ✓ Scan QR code
2. ✓ Extract student ID
3. ✓ Send ID to server
4. ✓ Display database timestamp returned by server

### What Client MUST NOT Do:

1. ❌ Generate timestamps
2. ❌ Send timestamps to server
3. ❌ Use DateTime.Now for attendance
4. ❌ Allow manual time entry

---

## 🧪 Testing the System

### Test 1: Normal Time In

```
Client Time: 2025-11-28 14:30:00
Database Time: 2025-11-28 14:30:00
Expected: Record created with 14:30:00 (database time)
```

### Test 2: Client Clock Ahead (Manipulation Attempt)

```
Client Time: 2025-11-29 09:00:00  ← Changed ahead
Database Time: 2025-11-28 14:30:00
Expected: Record created with 14:30:00 (database time, not client)
✓ Manipulation prevented!
```

### Test 3: Client Clock Behind (Manipulation Attempt)

```
Client Time: 2025-11-27 10:00:00  ← Changed back
Database Time: 2025-11-28 14:30:00
Expected: Record created with 14:30:00 (database time, not client)
✓ Manipulation prevented!
```

### Test 4: Verify Tamper-Proof Storage

```sql
-- Check actual stored timestamp
SELECT student_id, scan_datetime, created_at
FROM scan_history
WHERE student_id = 123
ORDER BY scan_datetime DESC
LIMIT 1;

-- Result should show database server time, NOT client time
```

---

## 📋 Deployment Checklist

- [ ] Deploy database migration `003_secure_timestamp_attendance.sql`
- [ ] Verify stored procedure created: `sp_record_attendance_scan_secure`
- [ ] Update repository to use new procedure
- [ ] Update client app to display database timestamps
- [ ] Remove all `DateTime.Now` usage for attendance logging
- [ ] Test with client clock manipulation
- [ ] Configure database server time zone (UTC or local)
- [ ] Enable database audit logging
- [ ] Set up monitoring for timestamp anomalies

---

## 🎯 Best Practices

### Database Time Synchronization

```sql
-- Ensure MySQL server time is correct
SELECT NOW() as server_time, @@global.time_zone, @@session.time_zone;

-- Set timezone (if needed)
SET GLOBAL time_zone = '+08:00';  -- Philippines
SET SESSION time_zone = '+08:00';
```

### Error Handling

```csharp
// Always check if timestamp was returned
if (!timestamp.HasValue)
{
    logger.LogWarning("Database did not return timestamp - possible DB issue");
    return "Attendance recorded but timestamp unavailable";
}

// Display database time clearly
MessageBox.Show(
    $"Recorded at: {timestamp:yyyy-MM-dd HH:mm:ss}\n" +
    $"(Database Server Time - Tamper-Proof)",
    "Success"
);
```

---

## 📊 System Monitoring

### Monitor for Anomalies

```sql
-- Check for suspicious patterns
SELECT
    DATE(scan_datetime) as date,
    HOUR(scan_datetime) as hour,
    COUNT(*) as scan_count
FROM scan_history
WHERE scan_datetime >= CURDATE() - INTERVAL 7 DAY
GROUP BY DATE(scan_datetime), HOUR(scan_datetime)
HAVING scan_count > 100  -- Unusual activity
ORDER BY date DESC, hour DESC;

-- Verify all timestamps are recent
SELECT
    COUNT(*) as future_scans
FROM scan_history
WHERE scan_datetime > NOW() + INTERVAL 1 MINUTE;
-- Should return 0
```

---

## 🔄 Migration Guide

### From Old System (Client Timestamps) to New System (Database Timestamps)

1. **Backup existing data**

```sql
CREATE TABLE scan_history_backup AS SELECT * FROM scan_history;
```

2. **Deploy new stored procedure**

```sql
SOURCE Database/migrations/003_secure_timestamp_attendance.sql;
```

3. **Update client code**

- Replace `RecordAttendanceScanAsync` calls
- Update UI to use returned timestamps

4. **Verify**

```sql
-- Check new records use database time
SELECT * FROM scan_history
WHERE created_at > NOW() - INTERVAL 1 HOUR
LIMIT 10;
```

---

## ✅ Conclusion

This system provides **tamper-proof attendance tracking** by:

1. ✓ **Database-only timestamp generation** using `NOW()`
2. ✓ **No client time influence** - client sends ID only
3. ✓ **Returned timestamps** for display purposes only
4. ✓ **Complete audit trail** with server-generated times
5. ✓ **Prevents clock manipulation** - client time is irrelevant

**Security Status: 🔒 MAXIMUM - Client cannot manipulate attendance timestamps**

---

## 📞 Support

For issues or questions about the secure timestamp system:

- Check database server time: `SELECT NOW();`
- Review stored procedure: `SHOW CREATE PROCEDURE sp_record_attendance_scan_secure;`
- Verify migration applied: Check `Database/migrations/` folder
- Test with manipulated client clock to confirm security

**Document Version:** 1.0  
**Last Updated:** November 28, 2025  
**Security Level:** Maximum (Database-Only Timestamps)
