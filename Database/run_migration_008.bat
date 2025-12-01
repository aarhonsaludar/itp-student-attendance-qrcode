@echo off
REM ============================================
REM Run Migration 008 - Update Stored Procedure with TickCount Support
REM ============================================

echo.
echo ========================================
echo Migration 008: Update Stored Procedure with TickCount Support
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
echo Running migration 008...
echo.

REM Execute migration
mysql -h %DB_HOST% -P %DB_PORT% -u %DB_USER% -p%DB_PASSWORD% %DB_NAME% < migrations\008_update_stored_procedure_tickcount.sql

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ========================================
    echo Migration 008 completed successfully!
    echo ========================================
    echo.
    echo The stored procedure sp_record_attendance_scan_secure has been updated.
    echo Now supports TickCount64 anti-tampering parameters.
    echo.
) else (
    echo.
    echo ========================================
    echo Migration 008 FAILED!
    echo ========================================
    echo Please check the error messages above.
    echo.
)

pause
