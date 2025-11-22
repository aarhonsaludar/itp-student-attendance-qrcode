@echo off
REM ============================================
REM MySQL Migration Script - Add Home Address
REM ============================================

setlocal enabledelayedexpansion

REM MySQL Connection Details (from App.config)
set MYSQL_HOST=localhost
set MYSQL_PORT=3306
set MYSQL_USER=root
set MYSQL_PASS=admin
set MYSQL_DB=student_attendance_db

REM Migration File
set MIGRATION_FILE=007_add_home_address_column.sql

REM Execute Migration
echo.
echo ============================================
echo Running Migration: %MIGRATION_FILE%
echo ============================================
echo.

mysql -h %MYSQL_HOST% -P %MYSQL_PORT% -u %MYSQL_USER% -p%MYSQL_PASS% %MYSQL_DB% < %MIGRATION_FILE%

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ============================================
    echo SUCCESS: Migration completed!
    echo ============================================
    echo.
) else (
    echo.
    echo ============================================
    echo ERROR: Migration failed!
    echo ============================================
    echo.
    pause
)

endlocal
