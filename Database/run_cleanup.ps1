# ============================================
# Run Database Cleanup Script
# PowerShell Script for Windows
# ============================================

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Database Cleanup - Remove Unused Data" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "WARNING: This will permanently delete:" -ForegroundColor Yellow
Write-Host "  - tokens table (unused)" -ForegroundColor White
Write-Host "  - 3 unused stored procedures" -ForegroundColor White
Write-Host "  - System logs older than 60 days" -ForegroundColor White
Write-Host ""

$confirm = Read-Host "Do you want to continue? (yes/no)"
if ($confirm -ne "yes") {
    Write-Host "Cleanup cancelled." -ForegroundColor Yellow
    Read-Host "Press Enter to exit"
    exit 0
}

Write-Host ""

# Get MySQL credentials
$mysqlUser = Read-Host "Enter MySQL username (default: root)"
if ([string]::IsNullOrWhiteSpace($mysqlUser)) {
    $mysqlUser = "root"
}

$mysqlPassword = Read-Host "Enter MySQL password" -AsSecureString
$BSTR = [System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($mysqlPassword)
$mysqlPasswordPlain = [System.Runtime.InteropServices.Marshal]::PtrToStringAuto($BSTR)

# MySQL connection details
$mysqlHost = "localhost"
$mysqlPort = "3306"

Write-Host ""
Write-Host "Connecting to MySQL..." -ForegroundColor Yellow

# Get the script directory
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$cleanupFile = Join-Path $scriptDir "cleanup_unused_data.sql"

# Check if cleanup file exists
if (-Not (Test-Path $cleanupFile)) {
    Write-Host "ERROR: Cleanup file not found: $cleanupFile" -ForegroundColor Red
    Read-Host "Press Enter to exit"
    exit 1
}

Write-Host "Cleanup file: $cleanupFile" -ForegroundColor Gray
Write-Host ""

# Run the cleanup
Write-Host "Running database cleanup..." -ForegroundColor Yellow

try {
    # Execute MySQL command
    $env:MYSQL_PWD = $mysqlPasswordPlain
    & mysql -h $mysqlHost -P $mysqlPort -u $mysqlUser -e "source `"$cleanupFile`""
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host ""
        Write-Host "========================================" -ForegroundColor Green
        Write-Host "Database cleanup completed successfully!" -ForegroundColor Green
        Write-Host "========================================" -ForegroundColor Green
        Write-Host ""
        Write-Host "What was removed:" -ForegroundColor Cyan
        Write-Host "  - tokens table (was never used)" -ForegroundColor White
        Write-Host "  - sp_get_student_by_qrcode procedure" -ForegroundColor White
        Write-Host "  - sp_register_student procedure" -ForegroundColor White
        Write-Host "  - sp_record_attendance_scan procedure (old version)" -ForegroundColor White
        Write-Host "  - Old system logs (moved to system_logs_archive)" -ForegroundColor White
        Write-Host ""
        Write-Host "Database optimized and cleaned!" -ForegroundColor Green
        Write-Host ""
    } else {
        Write-Host ""
        Write-Host "ERROR: Cleanup failed with exit code: $LASTEXITCODE" -ForegroundColor Red
        Write-Host "Please check the error messages above." -ForegroundColor Red
    }
} catch {
    Write-Host ""
    Write-Host ("ERROR: " + $_.Exception.Message) -ForegroundColor Red
} finally {
    # Clear the password from environment
    Remove-Item Env:\MYSQL_PWD -ErrorAction SilentlyContinue
}

Write-Host ""
Read-Host "Press Enter to exit"
