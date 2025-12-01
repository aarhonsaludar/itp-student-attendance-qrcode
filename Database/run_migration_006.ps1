# ============================================
# PowerShell Script: Run Migration 006
# Update vw_recent_scans view with validation columns
# ============================================

Write-Host "============================================" -ForegroundColor Cyan
Write-Host "Running Migration 006: Update vw_recent_scans" -ForegroundColor Cyan
Write-Host "============================================" -ForegroundColor Cyan
Write-Host ""

# Check if MySQL is accessible
try {
    $mysqlVersion = mysql --version 2>&1
    Write-Host "MySQL found: $mysqlVersion" -ForegroundColor Green
    Write-Host ""
}
catch {
    Write-Host "ERROR: MySQL command not found in PATH" -ForegroundColor Red
    Write-Host "Please ensure MySQL is installed and added to system PATH" -ForegroundColor Yellow
    Read-Host "Press Enter to exit"
    exit 1
}

# Prompt for MySQL root password
$password = Read-Host "Enter MySQL root password" -AsSecureString
$BSTR = [System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($password)
$plainPassword = [System.Runtime.InteropServices.Marshal]::PtrToStringAuto($BSTR)

Write-Host "Running migration script..." -ForegroundColor Yellow

# Run the migration
$migrationFile = Join-Path $PSScriptRoot "migrations\006_update_vw_recent_scans_with_validation_columns.sql"

if (Test-Path $migrationFile) {
    try {
        Get-Content $migrationFile | mysql -u root -p"$plainPassword" 2>&1 | Out-String | Write-Host
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host ""
            Write-Host "============================================" -ForegroundColor Green
            Write-Host "Migration 006 completed successfully!" -ForegroundColor Green
            Write-Host "============================================" -ForegroundColor Green
        }
        else {
            Write-Host ""
            Write-Host "============================================" -ForegroundColor Red
            Write-Host "ERROR: Migration 006 failed!" -ForegroundColor Red
            Write-Host "============================================" -ForegroundColor Red
        }
    }
    catch {
        Write-Host ""
        Write-Host "ERROR: $($_.Exception.Message)" -ForegroundColor Red
    }
}
else {
    Write-Host "ERROR: Migration file not found: $migrationFile" -ForegroundColor Red
}

Write-Host ""
Read-Host "Press Enter to exit"
