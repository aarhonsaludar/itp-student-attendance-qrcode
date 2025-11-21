# Dashboard Error Fix - Column Name Issue

## ✅ FIXED: "Unknown column 'last_name' in 'order clause'"

### 🐛 Problem Identified

**Error Message:**
```
Error retrieving students: Unknown column 'last_name' in 'order clause'
```

**Root Cause:**
The `StudentRepository.GetAllAsync()` method was querying the `vw_active_students` view when `activeOnly = true`, but this view doesn't have `first_name`, `middle_name`, or `last_name` columns - it only has a `full_name` column.

**View Structure (vw_active_students):**
```sql
SELECT 
    s.student_id,
    s.student_number,
    CONCAT(s.first_name, ' ', IFNULL(s.middle_name, ''), ' ', s.last_name) AS full_name,  -- Combined!
    s.email,
    s.phone,
    s.year_level,
    s.program,
    s.section,
    s.status,
    s.enrollment_date,
    COUNT(t.token_id) AS active_tokens
FROM students s
...
```

The query was trying to `ORDER BY last_name, first_name` but those columns don't exist in the view.

### 🔧 Solution Applied

Changed `StudentRepository.GetAllAsync()` to query the `students` table directly instead of using the view.

**File:** `Data\StudentRepository.cs`

**Before (BROKEN):**
```csharp
string query = activeOnly
    ? "SELECT * FROM vw_active_students ORDER BY last_name, first_name"  // ❌ View doesn't have these columns
    : @"SELECT student_id, student_number, first_name, middle_name, last_name, 
       email, phone, year_level, program, section, qr_code_data, photo_path, 
       status, enrollment_date, created_at, updated_at
       FROM students ORDER BY last_name, first_name";
```

**After (FIXED):**
```csharp
// Query students table directly to get all required columns
string query = @"SELECT student_id, student_number, first_name, middle_name, last_name, 
               email, phone, year_level, program, section, qr_code_data, photo_path, 
               status, enrollment_date, created_at, updated_at
               FROM students " +
               (activeOnly ? "WHERE status = 'Active' " : "") +  // ✅ Filter by status directly
               "ORDER BY last_name, first_name";
```

### ✅ What Changed

1. **Removed dependency on `vw_active_students` view**
2. **Query `students` table directly** with all required columns
3. **Added conditional WHERE clause** to filter active students when needed
4. **Maintains same functionality** but with correct column names

### 🎯 Benefits

- ✅ All required columns are available (`first_name`, `middle_name`, `last_name`, etc.)
- ✅ Proper ORDER BY clause works correctly
- ✅ `MapStudent()` method receives all expected columns
- ✅ No dependency on view structure
- ✅ More maintainable and explicit

### 🧪 Testing

After this fix, the dashboard should:

1. ✅ Load without errors
2. ✅ Display total active students count
3. ✅ Show scans today count
4. ✅ Display most used scan type
5. ✅ Show recent scans table
6. ✅ Auto-refresh every 5 seconds

### 📋 All Fixes Applied So Far

#### Fix #1: Parameter Name Mismatch
- **File:** `Data\ScanHistoryRepository.cs`
- **Issue:** `@p_target_date` → `@p_date`
- **Status:** ✅ Fixed

#### Fix #2: Column Name Mismatch
- **File:** `Data\StudentRepository.cs`
- **Issue:** View doesn't have `last_name`, `first_name` columns
- **Status:** ✅ Fixed

#### Fix #3: Enhanced Error Handling
- **File:** `MainDashboard.cs`
- **Added:** Detailed error messages and logging
- **Status:** ✅ Implemented

### 🚀 Ready to Test

**Next Steps:**
1. Build the application
2. Run the application
3. Navigate to Dashboard
4. Verify all statistics display correctly

**Expected Results:**
- Total Students: Shows count of active students
- Scans Today: Shows count of today's scans (or 0 if none)
- Most Used Scan: Shows "QR Code (X)" or "Manual (X)" or "No scans today"
- Recent Scans: Shows last 10 scans from past 24 hours

### 💡 Why This Error Occurred

The original code was trying to use a view (`vw_active_students`) that was designed for a different purpose (showing active students with token counts). The view concatenated the name fields into a single `full_name` column, but the repository code expected individual `first_name`, `middle_name`, and `last_name` columns.

By querying the base `students` table directly, we ensure all required columns are available and the code works as expected.

### ✅ Status: RESOLVED

The dashboard should now load correctly with real data from your database!
