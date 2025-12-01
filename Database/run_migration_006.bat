@echo off
echo ============================================
echo Running Migration 006: Update vw_recent_scans
echo ============================================
echo.

REM Check if MySQL is accessible
mysql --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ERROR: MySQL command not found in PATH
    echo Please ensure MySQL is installed and added to system PATH
    pause
    exit /b 1
)

echo Running migration script...
mysql -u root -p < migrations\006_update_vw_recent_scans_with_validation_columns.sql

if %errorlevel% equ 0 (
    echo.
    echo ============================================
    echo Migration 006 completed successfully!
    echo ============================================
) else (
    echo.
    echo ============================================
    echo ERROR: Migration 006 failed!
    echo ============================================
)

echo.
pause
