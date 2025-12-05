# ============================================
# Student Attendance System - Fresh Install Script
# PowerShell Script for Windows
# ============================================
# This script automates the complete database installation
# for fresh devices/installations
# ============================================

Write-Host "============================================" -ForegroundColor Cyan
Write-Host "STUDENT ATTENDANCE SYSTEM" -ForegroundColor Cyan
Write-Host "Fresh Installation Setup" -ForegroundColor Cyan
Write-Host "============================================" -ForegroundColor Cyan
Write-Host ""

# Configuration
$MYSQL_USER = "root"
$MYSQL_HOST = "localhost"
$MYSQL_PORT = "3306"
$SCRIPT_PATH = "FRESH_INSTALL_SETUP.sql"
$LOG_FILE = "installation_log_$(Get-Date -Format 'yyyyMMdd_HHmmss').txt"

# Function to log messages
function Write-Log {
    param($Message, $Color = "White")
    $timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    $logMessage = "[$timestamp] $Message"
    Write-Host $logMessage -ForegroundColor $Color
    Add-Content -Path $LOG_FILE -Value $logMessage
}

# Start installation
Write-Log "Starting fresh installation..." "Green"
Write-Log "Log file: $LOG_FILE" "Gray"
Write-Host ""

# Check if MySQL is installed
Write-Log "Checking MySQL installation..." "Yellow"
$mysqlPath = Get-Command mysql -ErrorAction SilentlyContinue

if (-not $mysqlPath) {
    Write-Log "ERROR: MySQL not found in PATH" "Red"
    Write-Log "Please install MySQL or add it to your system PATH" "Red"
    Write-Host ""
    Read-Host "Press Enter to exit"
    exit 1
}

Write-Log "MySQL found: $($mysqlPath.Source)" "Green"
Write-Host ""

# Check if script file exists
if (-not (Test-Path $SCRIPT_PATH)) {
    Write-Log "ERROR: Installation script not found: $SCRIPT_PATH" "Red"
    Write-Log "Please ensure $SCRIPT_PATH is in the same directory" "Red"
    Write-Host ""
    Read-Host "Press Enter to exit"
    exit 1
}

Write-Log "Installation script found: $SCRIPT_PATH" "Green"
Write-Host ""

# Prompt for MySQL password
Write-Host "Please enter MySQL root password:" -ForegroundColor Yellow
$MYSQL_PASSWORD = Read-Host -AsSecureString
$MYSQL_PASSWORD_PLAIN = [Runtime.InteropServices.Marshal]::PtrToStringAuto(
    [Runtime.InteropServices.Marshal]::SecureStringToBSTR($MYSQL_PASSWORD)
)

Write-Host ""

# Test MySQL connection
Write-Log "Testing MySQL connection..." "Yellow"
$testQuery = "SELECT VERSION();"
$testResult = $testQuery | mysql -u $MYSQL_USER -p"$MYSQL_PASSWORD_PLAIN" -h $MYSQL_HOST -P $MYSQL_PORT 2>&1

if ($LASTEXITCODE -ne 0) {
    Write-Log "ERROR: Cannot connect to MySQL" "Red"
    Write-Log "Error: $testResult" "Red"
    Write-Host ""
    Read-Host "Press Enter to exit"
    exit 1
}

Write-Log "MySQL connection successful!" "Green"
Write-Log "MySQL Version: $($testResult | Select-Object -Last 1)" "Gray"
Write-Host ""

# Warning about existing database
Write-Host "============================================" -ForegroundColor Yellow
Write-Host "WARNING: This will DELETE the existing database!" -ForegroundColor Red
Write-Host "Database name: student_attendance_db" -ForegroundColor Yellow
Write-Host "All existing data will be PERMANENTLY LOST!" -ForegroundColor Red
Write-Host "============================================" -ForegroundColor Yellow
Write-Host ""

$confirmation = Read-Host "Type 'YES' to continue or anything else to cancel"

if ($confirmation -ne "YES") {
    Write-Log "Installation cancelled by user" "Yellow"
    Write-Host ""
    Read-Host "Press Enter to exit"
    exit 0
}

Write-Host ""
Write-Log "User confirmed. Proceeding with installation..." "Green"
Write-Host ""

# Execute installation script
Write-Log "Executing installation script..." "Yellow"
Write-Log "This may take a few moments..." "Gray"
Write-Host ""

$installStart = Get-Date

# Run the SQL script
$scriptResult = Get-Content $SCRIPT_PATH | mysql -u $MYSQL_USER -p"$MYSQL_PASSWORD_PLAIN" -h $MYSQL_HOST -P $MYSQL_PORT 2>&1

$installEnd = Get-Date
$installDuration = ($installEnd - $installStart).TotalSeconds

if ($LASTEXITCODE -ne 0) {
    Write-Log "ERROR: Installation failed!" "Red"
    Write-Log "Error details: $scriptResult" "Red"
    Write-Host ""
    Write-Log "Please check the log file for details: $LOG_FILE" "Yellow"
    Write-Host ""
    Read-Host "Press Enter to exit"
    exit 1
}

Write-Log "Installation script executed successfully!" "Green"
Write-Log "Installation completed in $([math]::Round($installDuration, 2)) seconds" "Gray"
Write-Host ""

# Verify installation
Write-Log "Verifying installation..." "Yellow"

$verifyQuery = @"
USE student_attendance_db;
SELECT 
    (SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = 'student_attendance_db') AS tables,
    (SELECT COUNT(*) FROM information_schema.routines WHERE routine_schema = 'student_attendance_db' AND routine_type = 'PROCEDURE') AS procedures,
    (SELECT COUNT(*) FROM information_schema.views WHERE table_schema = 'student_attendance_db') AS views,
    (SELECT COUNT(*) FROM users) AS users,
    (SELECT COUNT(*) FROM devices) AS devices,
    (SELECT COUNT(*) FROM system_settings) AS settings;
"@

$verifyResult = $verifyQuery | mysql -u $MYSQL_USER -p"$MYSQL_PASSWORD_PLAIN" -h $MYSQL_HOST -P $MYSQL_PORT 2>&1

if ($LASTEXITCODE -eq 0) {
    Write-Log "Verification successful!" "Green"
    Write-Host ""
    Write-Host "Installation Summary:" -ForegroundColor Cyan
    Write-Host $verifyResult
    Write-Host ""
}

# Display success message
Write-Host ""
Write-Host "============================================" -ForegroundColor Green
Write-Host "INSTALLATION COMPLETED SUCCESSFULLY!" -ForegroundColor Green
Write-Host "============================================" -ForegroundColor Green
Write-Host ""

Write-Host "Database: student_attendance_db" -ForegroundColor Cyan
Write-Host "Host: $MYSQL_HOST:$MYSQL_PORT" -ForegroundColor Cyan
Write-Host ""

Write-Host "Default Admin Credentials:" -ForegroundColor Yellow
Write-Host "  Username: admin" -ForegroundColor White
Write-Host "  Password: admin123" -ForegroundColor White
Write-Host ""
Write-Host "IMPORTANT: Change the password immediately after first login!" -ForegroundColor Red
Write-Host ""

Write-Host "Next Steps:" -ForegroundColor Cyan
Write-Host "  1. Update App.config with database connection string" -ForegroundColor White
Write-Host "  2. Configure SMTP settings in App.config" -ForegroundColor White
Write-Host "  3. Run the C# application" -ForegroundColor White
Write-Host "  4. Login with default credentials" -ForegroundColor White
Write-Host "  5. Change admin password immediately" -ForegroundColor White
Write-Host "  6. Add students via Student Management" -ForegroundColor White
Write-Host ""

Write-Log "Installation log saved to: $LOG_FILE" "Gray"
Write-Host ""

# Offer to display App.config template
Write-Host "Would you like to see the App.config connection string template? (Y/N)" -ForegroundColor Yellow
$showConfig = Read-Host

if ($showConfig -eq "Y" -or $showConfig -eq "y") {
    Write-Host ""
    Write-Host "============================================" -ForegroundColor Cyan
    Write-Host "App.config Connection String Template:" -ForegroundColor Cyan
    Write-Host "============================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host '<connectionStrings>' -ForegroundColor Gray
    Write-Host '  <add name="MySqlConnection"' -ForegroundColor Gray
    Write-Host '       connectionString="Server=localhost;Port=3306;Database=student_attendance_db;' -ForegroundColor Gray
    Write-Host '                         Uid=root;Pwd=YOUR_MYSQL_PASSWORD;' -ForegroundColor Gray
    Write-Host '                         SslMode=None;CharSet=utf8mb4;' -ForegroundColor Gray
    Write-Host '                         AllowUserVariables=True;' -ForegroundColor Gray
    Write-Host '                         ConnectionTimeout=30;"' -ForegroundColor Gray
    Write-Host '       providerName="MySql.Data.MySqlClient" />' -ForegroundColor Gray
    Write-Host '</connectionStrings>' -ForegroundColor Gray
    Write-Host ""
    Write-Host "Replace YOUR_MYSQL_PASSWORD with your actual MySQL password" -ForegroundColor Yellow
    Write-Host ""
}

Write-Host "Installation complete! Press Enter to exit..." -ForegroundColor Green
Read-Host

# Clear sensitive data
$MYSQL_PASSWORD_PLAIN = $null

Write-Log "Installation script finished" "Green"
