# ============================================
# Migration 003 - Secure Timestamp Attendance
# PowerShell Script for Database Migration
# ============================================

Write-Host ""
Write-Host "================================================" -ForegroundColor Cyan
Write-Host " Student Attendance System - Database Migration" -ForegroundColor Cyan
Write-Host " Migration 003: Secure Timestamp Attendance" -ForegroundColor Cyan
Write-Host "================================================" -ForegroundColor Cyan
Write-Host ""

# Get the script directory
$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
$migrationFile = Join-Path $scriptPath "migrations\003_secure_timestamp_attendance.sql"

# Check if migration file exists
if (-not (Test-Path $migrationFile)) {
    Write-Host "❌ Error: Migration file not found!" -ForegroundColor Red
    Write-Host "Expected location: $migrationFile" -ForegroundColor Yellow
    Write-Host ""
    pause
    exit 1
}

# Prompt for MySQL credentials
$mysqlUser = Read-Host "Enter MySQL username (default: root)"
if ([string]::IsNullOrWhiteSpace($mysqlUser)) {
    $mysqlUser = "root"
}

$database = Read-Host "Enter database name (default: student_attendance_db)"
if ([string]::IsNullOrWhiteSpace($database)) {
    $database = "student_attendance_db"
}

Write-Host ""
Write-Host "Running migration script..." -ForegroundColor Yellow
Write-Host ""

# Build the MySQL command
$mysqlCommand = "mysql -u $mysqlUser -p < `"$migrationFile`""

# Execute the migration
try {
    $process = Start-Process -FilePath "mysql" -ArgumentList "-u", $mysqlUser, "-p" -RedirectStandardInput $migrationFile -NoNewWindow -Wait -PassThru
    
    if ($process.ExitCode -eq 0) {
        Write-Host ""
        Write-Host "================================================" -ForegroundColor Green
        Write-Host " ✅ Migration completed successfully!" -ForegroundColor Green
        Write-Host "================================================" -ForegroundColor Green
        Write-Host ""
        Write-Host "The following has been created:" -ForegroundColor White
        Write-Host "  - sp_record_attendance_scan_secure" -ForegroundColor Cyan
        Write-Host ""
        Write-Host "This procedure returns database-generated" -ForegroundColor White
        Write-Host "timestamps to prevent time manipulation." -ForegroundColor White
        Write-Host ""
        
        # Verify the procedure was created
        Write-Host "Verifying installation..." -ForegroundColor Yellow
        Write-Host ""
        Write-Host "You can verify the procedure by running:" -ForegroundColor White
        Write-Host "  SHOW PROCEDURE STATUS WHERE Name = 'sp_record_attendance_scan_secure';" -ForegroundColor Cyan
        Write-Host ""
    } else {
        throw "MySQL command failed with exit code: $($process.ExitCode)"
    }
} catch {
    Write-Host ""
    Write-Host "================================================" -ForegroundColor Red
    Write-Host " ❌ Migration failed!" -ForegroundColor Red
    Write-Host "================================================" -ForegroundColor Red
    Write-Host ""
    Write-Host "Error: $_" -ForegroundColor Red
    Write-Host ""
    Write-Host "Please check:" -ForegroundColor Yellow
    Write-Host "  1. MySQL is running and accessible" -ForegroundColor White
    Write-Host "  2. Username and password are correct" -ForegroundColor White
    Write-Host "  3. Database '$database' exists" -ForegroundColor White
    Write-Host "  4. MySQL bin directory is in your PATH" -ForegroundColor White
    Write-Host ""
    Write-Host "Manual execution command:" -ForegroundColor Yellow
    Write-Host "  mysql -u $mysqlUser -p $database < `"$migrationFile`"" -ForegroundColor Cyan
    Write-Host ""
}

pause
