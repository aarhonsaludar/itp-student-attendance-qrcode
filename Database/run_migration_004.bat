@echo off
REM Run migration 004 to exclude for_review scans from recent scans view
REM This migration adds a filter to vw_recent_scans to hide pending approval scans

echo ========================================
echo Migration 004: Exclude for_review scans
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

echo Running migration 004...
echo.

REM Run the migration script
mysql -u root -padmin student_attendance_db < "%~dp0migrations\004_exclude_for_review_from_recent_scans.sql"

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ========================================
    echo Migration 004 completed successfully!
    echo ========================================
    echo.
    echo The vw_recent_scans view has been updated.
    echo Scans with 'for_review' status will no longer appear
    echo in Recent Scan Activity until approved.
    echo.
) else (
    echo.
    echo ========================================
    echo ERROR: Migration 004 failed!
    echo ========================================
    echo.
    echo Please check:
    echo 1. Database connection is working
    echo 2. You have permissions to modify views
    echo 3. Migration file exists
    echo.
)

pause
