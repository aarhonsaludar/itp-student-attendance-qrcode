# Recent Scan Activity Approval Workflow

## Overview

This document describes the approval workflow for offline/for_review scans before they appear in the Recent Scan Activity section of the MainDashboard.

## Feature Description

### Problem

Previously, all scans (including those flagged for review due to offline mode) were immediately displayed in the "Recent Scan Activity" section of the MainDashboard, regardless of their approval status.

### Solution

Scans with `for_review` status are now excluded from the Recent Scan Activity until they are approved by an administrator through the ScanDetailsDialog.

## Implementation Details

### 1. Database View Update

**File:** `Database/schema.sql` and `Database/migrations/004_exclude_for_review_from_recent_scans.sql`

The `vw_recent_scans` view has been updated to filter out scans with `for_review` status:

```sql
CREATE VIEW vw_recent_scans AS
SELECT
    -- ... columns ...
FROM scan_history sh
JOIN students s ON sh.student_id = s.student_id
LEFT JOIN devices d ON sh.device_id = d.device_id
WHERE sh.scan_datetime >= DATE_SUB(NOW(), INTERVAL 24 HOUR)
    AND sh.status != 'for_review'  -- NEW: Exclude pending approval scans
ORDER BY sh.scan_datetime DESC;
```

### 2. ScanDetailsDialog Updates

**File:** `ScanDetailsDialog.cs`

Added `ReviewCompleted` event to notify parent forms when a review action is taken:

```csharp
public event EventHandler ReviewCompleted;
```

The event is triggered after successful approval or decline:

- `btnAccept_Click` - Triggers event after approving scan
- `btnDecline_Click` - Triggers event after declining scan

### 3. MainDashboard Updates

**File:** `MainDashboard.cs`

Updated the following methods to refresh Recent Scan Activity after ScanHistoryScreen closes:

- `BtnReports_Click` - Changed to async and refreshes after ShowDialog
- `btnNavReports_Click` - Changed to async and refreshes after ShowDialog

```csharp
private async void BtnReports_Click(object sender, EventArgs e)
{
    ScanHistoryScreen historyScreen = new ScanHistoryScreen();
    historyScreen.ShowDialog();

    // Refresh dashboard after closing in case reviews were processed
    await LoadRecentScansAsync();
}
```

## Workflow

### Scan Creation (Offline Mode)

1. Student scans QR code while system is offline
2. Scan is recorded with `status = 'for_review'`
3. Scan **does NOT** appear in Recent Scan Activity
4. Scan is visible in Scan History with "Pending Review" status

### Review Process

1. Admin opens Scan History
2. Admin clicks on scan with "Pending Review" status
3. ScanDetailsDialog opens showing review buttons
4. Admin clicks **Accept** or **Decline**:
   - **Accept**: Status changes to `success`, scan appears in Recent Scan Activity
   - **Decline**: Status changes to `failed`, scan does NOT appear in Recent Scan Activity
5. ScanHistoryScreen automatically refreshes
6. When ScanHistoryScreen closes, MainDashboard refreshes Recent Scan Activity

### Automatic Display

After approval, the scan will automatically appear in Recent Scan Activity on the next refresh because:

1. Status is no longer `for_review`
2. The `vw_recent_scans` view includes it
3. MainDashboard refreshes when returning from Scan History

## Files Modified

### Database

- `Database/schema.sql` - Updated `vw_recent_scans` view
- `Database/migrations/004_exclude_for_review_from_recent_scans.sql` - Migration script
- `Database/run_migration_004.bat` - Batch migration runner
- `Database/run_migration_004.ps1` - PowerShell migration runner

### Application Code

- `ScanDetailsDialog.cs` - Added `ReviewCompleted` event
- `MainDashboard.cs` - Updated to refresh after ScanHistoryScreen closes

## Migration Instructions

Run the migration to update the database view:

```batch
cd Database
run_migration_004.bat
```

Or using PowerShell:

```powershell
cd Database
.\run_migration_004.ps1
```

## Testing

### Test Case 1: Offline Scan Creation

1. Disconnect from internet
2. Scan a student QR code
3. Verify scan is saved with `for_review` status
4. Check MainDashboard - scan should NOT appear in Recent Scan Activity
5. Check Scan History - scan should appear with "Pending Review" status

### Test Case 2: Approval Workflow

1. Open Scan History
2. Click on a scan with "Pending Review" status
3. Click "Accept" button
4. Close ScanDetailsDialog
5. Verify Scan History refreshes
6. Close Scan History
7. Verify Recent Scan Activity now shows the approved scan

### Test Case 3: Decline Workflow

1. Open Scan History
2. Click on a scan with "Pending Review" status
3. Click "Decline" button
4. Close ScanDetailsDialog
5. Verify Scan History refreshes (scan status updated to "Failed")
6. Close Scan History
7. Verify Recent Scan Activity does NOT show the declined scan

## Benefits

1. **Data Integrity**: Only approved scans appear in Recent Scan Activity
2. **Admin Control**: Administrators can review offline scans before they become visible
3. **Security**: Prevents potentially tampered scans from appearing as valid
4. **User Experience**: Clear visual distinction between approved and pending scans
5. **Audit Trail**: All scans remain in Scan History regardless of approval status

## Notes

- Scans with `success` status (online, verified scans) appear immediately in Recent Scan Activity
- The 24-hour time window still applies - only recent scans are shown
- Declined scans remain in the database with `failed` status for audit purposes
- The Recent Scan Activity automatically refreshes when returning from Scan History
