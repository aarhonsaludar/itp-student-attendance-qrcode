# Migration 005: Add Validation Mode Tracking
# Solution 1: Time-Out Validation Against Time-In

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Migration 005: Add Validation Mode Tracking" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check if MySQL is available
$mysqlPath = Get-Command mysql -ErrorAction SilentlyContinue
if (-not $mysqlPath) {
    Write-Host "ERROR: MySQL command-line client not found!" -ForegroundColor Red
    Write-Host "Please ensure MySQL is installed and added to PATH." -ForegroundColor Yellow
    pause
    exit 1
}

Write-Host "Running migration 005..." -ForegroundColor Yellow
Write-Host ""

# Get script directory
$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
$migrationFile = Join-Path $scriptPath "migrations\005_add_validation_mode_tracking.sql"

# Run the migration script
$processInfo = New-Object System.Diagnostics.ProcessStartInfo
$processInfo.FileName = "mysql"
$processInfo.Arguments = "-u root -padmin student_attendance_db"
$processInfo.RedirectStandardInput = $true
$processInfo.RedirectStandardOutput = $true
$processInfo.RedirectStandardError = $true
$processInfo.UseShellExecute = $false
$processInfo.CreateNoWindow = $true

$process = New-Object System.Diagnostics.Process
$process.StartInfo = $processInfo
$process.Start() | Out-Null

# Read and send the SQL file
$sqlContent = Get-Content $migrationFile -Raw
$process.StandardInput.WriteLine($sqlContent)
$process.StandardInput.Close()

$output = $process.StandardOutput.ReadToEnd()
$errors = $process.StandardError.ReadToEnd()
$process.WaitForExit()

if ($process.ExitCode -eq 0) {
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Green
    Write-Host "Migration 005 completed successfully!" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "Validation mode tracking columns added." -ForegroundColor White
    Write-Host "System can now detect time-in/time-out mode mismatches." -ForegroundColor White
    Write-Host ""
    Write-Host "New features:" -ForegroundColor Cyan
    Write-Host "  • Tracks if time-in was online or offline" -ForegroundColor White
    Write-Host "  • Tracks if time-out was online or offline" -ForegroundColor White
    Write-Host "  • Detects WiFi disconnect + time tampering attacks" -ForegroundColor White
    Write-Host ""
}
else {
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Red
    Write-Host "ERROR: Migration 005 failed!" -ForegroundColor Red
    Write-Host "========================================" -ForegroundColor Red
    Write-Host ""
    Write-Host "Error details:" -ForegroundColor Yellow
    Write-Host $errors -ForegroundColor Red
    Write-Host ""
    Write-Host "Please check:" -ForegroundColor Yellow
    Write-Host "1. Database connection is working" -ForegroundColor White
    Write-Host "2. You have permissions to alter tables" -ForegroundColor White
    Write-Host "3. Migration file exists" -ForegroundColor White
    Write-Host ""
}

pause
