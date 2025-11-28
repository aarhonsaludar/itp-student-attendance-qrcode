@echo off
REM ============================================
REM Migration 003 - Secure Timestamp Attendance
REM This batch file helps you run the migration
REM ============================================

echo.
echo ================================================
echo  Student Attendance System - Database Migration
echo  Migration 003: Secure Timestamp Attendance
echo ================================================
echo.

REM Prompt for MySQL credentials
set /p MYSQL_USER="Enter MySQL username (default: root): "
if "%MYSQL_USER%"=="" set MYSQL_USER=root

echo.
echo Running migration script...
echo.

REM Execute the migration
mysql -u %MYSQL_USER% -p < "%~dp0migrations\003_secure_timestamp_attendance.sql"

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ================================================
    echo  ✅ Migration completed successfully!
    echo ================================================
    echo.
    echo The following has been created:
    echo   - sp_record_attendance_scan_secure
    echo.
    echo This procedure returns database-generated
    echo timestamps to prevent time manipulation.
    echo.
) else (
    echo.
    echo ================================================
    echo  ❌ Migration failed!
    echo ================================================
    echo.
    echo Please check:
    echo   1. MySQL is running
    echo   2. Username and password are correct
    echo   3. Database 'student_attendance_db' exists
    echo.
)

pause
