@echo off
REM ============================================
REM Run Migration 007 - Add TickCount Anti-Tampering Fields
REM ============================================

echo.
echo ========================================
echo Migration 007: Add TickCount Anti-Tampering Fields
echo ========================================
echo.

REM Configuration
set DB_HOST=localhost
set DB_PORT=3306
set DB_NAME=student_attendance_db
set DB_USER=root

REM Prompt for password
set /p DB_PASSWORD="Enter MySQL root password: "

echo.
echo Running migration 007...
echo.

REM Execute migration
mysql -h %DB_HOST% -P %DB_PORT% -u %DB_USER% -p%DB_PASSWORD% %DB_NAME% < migrations\007_add_tickcount_anti_tampering.sql

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ========================================
    echo Migration 007 completed successfully!
    echo ========================================
    echo.
    echo The following fields have been added to scan_history table:
    echo - time_in_tick_count: Stores Environment.TickCount64 at Time In
    echo - time_out_tick_count: Stores Environment.TickCount64 at Time Out
    echo - connection_drop_count: Tracks disconnections during session
    echo - offline_duration_minutes: Total offline duration
    echo.
    echo These fields enable offline time tampering detection.
    echo.
) else (
    echo.
    echo ========================================
    echo Migration 007 FAILED!
    echo ========================================
    echo Please check the error messages above.
    echo.
)

pause
