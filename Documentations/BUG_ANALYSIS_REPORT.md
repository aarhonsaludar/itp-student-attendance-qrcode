# Bug Analysis Report - Student Attendance QR System

**Date:** November 21, 2025  
**Project:** ITP104 Final Project - Student Attendance System

---

## 🔴 **CRITICAL BUGS** (Must Fix Immediately)

### **BUG #1: Password Authentication Not Using BCrypt (SECURITY ISSUE)**

**File:** `Data/UserRepository.cs` - Line 40  
**Severity:** CRITICAL  
**Risk:** Major Security Vulnerability

**Problem:**

```csharp
string query = "SELECT * FROM users WHERE username = @username AND password_hash = @password AND is_active = 1";
command.Parameters.AddWithValue("@password", password); // ❌ Plaintext password!
```

The code is comparing plaintext passwords directly against the `password_hash` column. However, the database stores BCrypt hashes, not plaintext passwords. This will NEVER authenticate successfully because:

- Plaintext `password` != BCrypt hash of `password`

**Impact:**

- ❌ All users cannot login (authentication always fails)
- ❌ Security vulnerability - passwords should never be compared as plaintext
- ❌ BCrypt library is imported but not used

**Solution:** Use BCrypt.Verify() instead of direct comparison:

```csharp
// Fetch user by username first
string selectQuery = "SELECT * FROM users WHERE username = @username AND is_active = 1";
// Then verify password with BCrypt
bool passwordValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
```

---

### **BUG #2: Missing Async/Await in LoadRecentScans**

**File:** `MainDashboard.cs` - Line 937  
**Severity:** HIGH

**Problem:**

```csharp
private void LoadRecentScans()  // ❌ Should be async Task
{
    try
    {
        var recentScans = await scanHistoryRepository.GetRecentScansAsync(limit: 10);
        // ❌ Cannot use 'await' in non-async method!
    }
}
```

The method calls `LoadRecentScansAsync()` but is defined as `void` instead of `async Task`.

**Impact:**

- ❌ Compilation error - cannot use 'await' in non-async context
- ❌ Application will not compile

**Solution:** Change method signature:

```csharp
private async Task LoadRecentScansAsync()  // ✓ Correct
{
    try
    {
        var recentScans = await scanHistoryRepository.GetRecentScansAsync(limit: 10);
```

---

### **BUG #3: Missing ScanHistoryRepository Initialization**

**File:** `MainDashboard.cs` - Line 18+  
**Severity:** HIGH

**Problem:**
The `MainDashboard` class uses `scanHistoryRepository` but it's never initialized:

```csharp
private readonly StudentRepository studentRepository;
// ❌ scanHistoryRepository is used but never declared or initialized!
```

**Impact:**

- ❌ Null reference exception when accessing `scanHistoryRepository.GetRecentScansAsync()`
- ❌ Dashboard crashes on load

**Solution:** Add initialization:

```csharp
private readonly StudentRepository studentRepository;
private readonly ScanHistoryRepository scanHistoryRepository;  // ✓ Add this

public MainDashboard()
{
    InitializeComponent();
    studentRepository = new StudentRepository();
    scanHistoryRepository = new ScanHistoryRepository();  // ✓ Add this
    InitializeDashboard();
}
```

---

## 🟠 **HIGH PRIORITY BUGS**

### **BUG #4: Hardcoded Dashboard Statistics**

**File:** `MainDashboard.cs` - Line 916  
**Severity:** MEDIUM

**Problem:**

```csharp
private void LoadDashboardStats()
{
    try
    {
        lblTotalStudentsValue.Text = "1,247";      // ❌ Hardcoded
        lblScansTodayValue.Text = "89";            // ❌ Hardcoded
        lblMostUsedScanValue.Text = "QR Code";     // ❌ Hardcoded
    }
    catch { }
}
```

Dashboard statistics are hardcoded instead of loaded from database.

**Impact:**

- ❌ Dashboard shows fake data
- ❌ Statistics don't reflect actual database state
- ❌ Misleading information to users

**Solution:** Fetch actual data from database using stored procedures or queries.

---

### **BUG #5: Mock Scanner Status (Not Actual)**

**File:** `MainDashboard.cs` - Line 895  
**Severity:** MEDIUM

**Problem:**

```csharp
private void UpdateSystemStatus()
{
    bool scannerActive = random.Next(0, 10) > 2; // ❌ Random value!
    lblScannerStatus.Text = scannerActive ? "● Scanner: Ready" : "● Scanner: Idle";
}
```

Scanner status is randomized, not based on actual device state.

**Impact:**

- ❌ Misleading status information
- ❌ Users don't know actual scanner state
- ❌ No way to verify if scanner is truly ready

**Solution:** Check actual scanner device status from database.

---

### **BUG #6: Missing Error Logging in Repositories**

**File:** Multiple repository files  
**Severity:** MEDIUM

**Problem:**
Some repository methods don't properly log errors:

```csharp
catch (Exception ex)
{
    throw new Exception($"Error updating student: {ex.Message}", ex);
    // ❌ No logging to ErrorLoggingService
}
```

**Impact:**

- ❌ Errors not recorded in system logs
- ❌ Difficult to troubleshoot production issues
- ❌ Inconsistent error handling

**Solution:** Use `ErrorLoggingService.LogErrorAsync()` in all catch blocks.

---

## 🟡 **MEDIUM PRIORITY BUGS**

### **BUG #7: Null Reference Potential in MapScanHistory**

**File:** `Data/ScanHistoryRepository.cs` - Line 475  
**Severity:** MEDIUM

**Problem:**

```csharp
DeviceId = deviceIdOrdinal >= 0 ? (int?)GetIntValue(deviceIdOrdinal) : null,
```

`DeviceId` is nullable but could cause issues if not handled properly upstream.

**Impact:**

- ⚠️ Potential null reference exceptions in UI binding
- ⚠️ Data validation issues

**Solution:** Ensure all downstream code handles nullable DeviceId properly.

---

### **BUG #8: Empty Event Handler**

**File:** `LoginScreen.cs` - Line 38  
**Severity:** LOW

**Problem:**

```csharp
private void label1_Click(object sender, EventArgs e)
{
    // ❌ Empty handler
}
```

Empty event handler serves no purpose and should be removed.

**Impact:**

- ⚠️ Code clutter
- ⚠️ Confusing to maintainers

---

### **BUG #9: Duplicate Code in LoadRecentScansAsync**

**File:** `MainDashboard.cs` - Lines 1040-1125  
**Severity:** LOW

**Problem:**
The exact same code is duplicated for InvokeRequired and non-InvokeRequired cases.

**Impact:**

- ⚠️ Maintenance nightmare
- ⚠️ Easy to have inconsistent updates

**Solution:** Extract common logic into a helper method.

---

## ✅ **RECOMMENDATIONS**

### **Immediate Actions (Today):**

1. ✓ Fix authentication to use BCrypt verification
2. ✓ Add ScanHistoryRepository initialization
3. ✓ Fix async/await in LoadRecentScansAsync

### **Short-term (This Week):**

4. ✓ Replace hardcoded dashboard statistics with database queries
5. ✓ Implement actual scanner status checking
6. ✓ Add ErrorLoggingService to all repository methods

### **Long-term (This Month):**

7. ✓ Refactor duplicate code in LoadRecentScansAsync
8. ✓ Add unit tests for repository methods
9. ✓ Add integration tests for authentication flow

---

## 📊 **Bug Summary**

| Severity    | Count  | Status                  |
| ----------- | ------ | ----------------------- |
| 🔴 CRITICAL | 3      | Must Fix Before Release |
| 🟠 HIGH     | 3      | Fix Before Testing      |
| 🟡 MEDIUM   | 2      | Fix After Critical Bugs |
| 🟢 LOW      | 2      | Nice to Fix             |
| **TOTAL**   | **10** | -                       |

---

## 🚀 **Next Steps**

1. Review this report with the development team
2. Create tasks for each bug
3. Prioritize critical bugs first
4. Test fixes thoroughly before deployment

**Report Generated:** 2025-11-21  
**Project Status:** ⚠️ Not Ready for Release (Critical bugs present)
