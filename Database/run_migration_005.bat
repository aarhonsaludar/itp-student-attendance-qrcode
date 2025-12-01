@echo off
REM Run migration 005 to add validation mode tracking
REM This migration adds columns to detect WiFi disconnect + time tampering

echo ========================================
echo Migration 005: Add Validation Mode Tracking
echo ========================================
echo.

REM Check if MySQL is available
where mysql >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: MySQL command-line client not found!
    echo Please ensure MySQL is installed and added to PATH.
    pause
    exit /b 1
)

echo Running migration 005...
echo.

REM Run the migration script
mysql -u root -padmin student_attendance_db < "%~dp0migrations\005_add_validation_mode_tracking.sql"

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ========================================
    echo Migration 005 completed successfully!
    echo ========================================
    echo.
    echo Validation mode tracking columns added.
    echo System can now detect time-in/time-out mode mismatches.
    echo.
) else (
    echo.
    echo ========================================
    echo ERROR: Migration 005 failed!
    echo ========================================
    echo.
    echo Please check:
    echo 1. Database connection is working
    echo 2. You have permissions to alter tables
    echo 3. Migration file exists
    echo.
)

pause
