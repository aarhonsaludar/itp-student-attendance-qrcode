# ============================================
# Reset Students Database Script
# PowerShell Helper
# ============================================

Write-Host ""
Write-Host "================================================" -ForegroundColor Red
Write-Host " ⚠️  RESET STUDENTS DATABASE" -ForegroundColor Red
Write-Host "================================================" -ForegroundColor Red
Write-Host ""
Write-Host "WARNING: This will:" -ForegroundColor Yellow
Write-Host "  • Delete ALL student records" -ForegroundColor Red
Write-Host "  • Delete ALL scan history" -ForegroundColor Red
Write-Host "  • Delete ALL tokens" -ForegroundColor Red
Write-Host "  • Reset auto-increment IDs to 1" -ForegroundColor Red
Write-Host ""

$confirmation = Read-Host "Are you sure you want to continue? Type 'YES' to confirm"

if ($confirmation -ne "YES") {
    Write-Host ""
    Write-Host "Operation cancelled." -ForegroundColor Yellow
    Write-Host ""
    pause
    exit 0
}

Write-Host ""
Write-Host "Enter your MySQL credentials..." -ForegroundColor Cyan
Write-Host ""

$mysqlUser = Read-Host "MySQL username (default: root)"
if ([string]::IsNullOrWhiteSpace($mysqlUser)) {
    $mysqlUser = "root"
}

Write-Host ""
Write-Host "Executing reset script..." -ForegroundColor Yellow
Write-Host ""

$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
$sqlFile = Join-Path $scriptPath "migrations\reset_students.sql"

if (-not (Test-Path $sqlFile)) {
    Write-Host "❌ Error: SQL file not found at $sqlFile" -ForegroundColor Red
    pause
    exit 1
}

try {
    Get-Content $sqlFile | mysql -u $mysqlUser -p student_attendance_db
    
    Write-Host ""
    Write-Host "================================================" -ForegroundColor Green
    Write-Host " ✅ DATABASE RESET SUCCESSFUL" -ForegroundColor Green
    Write-Host "================================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "Results:" -ForegroundColor White
    Write-Host "  ✓ All students deleted" -ForegroundColor Green
    Write-Host "  ✓ All scan history deleted" -ForegroundColor Green
    Write-Host "  ✓ All tokens deleted" -ForegroundColor Green
    Write-Host "  ✓ Auto-increment IDs reset to 1" -ForegroundColor Green
    Write-Host ""
    Write-Host "Next student registered will have ID = 1" -ForegroundColor Cyan
    Write-Host ""
} catch {
    Write-Host ""
    Write-Host "================================================" -ForegroundColor Red
    Write-Host " ❌ RESET FAILED" -ForegroundColor Red
    Write-Host "================================================" -ForegroundColor Red
    Write-Host ""
    Write-Host "Error: $_" -ForegroundColor Red
    Write-Host ""
    Write-Host "Please check:" -ForegroundColor Yellow
    Write-Host "  1. MySQL is running" -ForegroundColor White
    Write-Host "  2. Username and password are correct" -ForegroundColor White
    Write-Host "  3. Database 'student_attendance_db' exists" -ForegroundColor White
    Write-Host ""
}

pause
