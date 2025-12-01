# ============================================
# Test Solution 1: WiFi Disconnect Detection
# Run this to test without opening the app
# ============================================

Write-Host ""
Write-Host "================================================" -ForegroundColor Cyan
Write-Host " Testing Solution 1: Attack Detection" -ForegroundColor Cyan
Write-Host " WiFi Disconnect + Time Tampering Test" -ForegroundColor Cyan
Write-Host "================================================" -ForegroundColor Cyan
Write-Host ""

# Get script path
$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
$testFile = Join-Path $scriptPath "test_attack_scenario.sql"

# Check if test file exists
if (-not (Test-Path $testFile)) {
    Write-Host "❌ Error: Test file not found!" -ForegroundColor Red
    Write-Host "Expected: $testFile" -ForegroundColor Yellow
    pause
    exit 1
}

# Prompt for MySQL credentials
Write-Host "Enter MySQL credentials:" -ForegroundColor Yellow
$mysqlUser = Read-Host "MySQL username (default: root)"
if ([string]::IsNullOrWhiteSpace($mysqlUser)) {
    $mysqlUser = "root"
}

$database = Read-Host "Database name (default: student_attendance_db)"
if ([string]::IsNullOrWhiteSpace($database)) {
    $database = "student_attendance_db"
}

Write-Host ""
Write-Host "Running attack simulation test..." -ForegroundColor Yellow
Write-Host ""

# Run the test
try {
    $output = Get-Content $testFile | mysql -u $mysqlUser -p $database 2>&1
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "================================================" -ForegroundColor Green
        Write-Host " ✅ Test completed successfully!" -ForegroundColor Green
        Write-Host "================================================" -ForegroundColor Green
        Write-Host ""
        Write-Host "Test Results:" -ForegroundColor Cyan
        Write-Host $output
        Write-Host ""
        Write-Host "================================================" -ForegroundColor Green
        Write-Host " Check the output above for:" -ForegroundColor Yellow
        Write-Host " 1. Time-In validation mode = 'online'" -ForegroundColor White
        Write-Host " 2. Time-Out validation mode = 'offline'" -ForegroundColor White
        Write-Host " 3. Status = 'for_review'" -ForegroundColor White
        Write-Host " 4. Notes containing 'CRITICAL' warning" -ForegroundColor White
        Write-Host "================================================" -ForegroundColor Green
    }
    else {
        Write-Host "================================================" -ForegroundColor Red
        Write-Host " ❌ Test failed!" -ForegroundColor Red
        Write-Host "================================================" -ForegroundColor Red
        Write-Host ""
        Write-Host "Error output:" -ForegroundColor Yellow
        Write-Host $output
        Write-Host ""
    }
}
catch {
    Write-Host "❌ Error running test: $_" -ForegroundColor Red
}

Write-Host ""
Write-Host "Press Enter to exit..."
Read-Host
