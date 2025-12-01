@echo off
REM ============================================
REM Run Migration 009: Fix TickCount Frequency Calculation
REM ============================================
echo.
echo ============================================
echo Migration 009: Fix TickCount Frequency
echo ============================================
echo.

REM Prompt for MySQL root password
set /p MYSQL_PASSWORD="Enter MySQL root password: "

echo.
echo Running migration...
mysql -u root -p%MYSQL_PASSWORD% < migrations\009_fix_tickcount_frequency.sql

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ✓ Migration 009 completed successfully!
    echo.
) else (
    echo.
    echo ✗ Migration 009 failed!
    echo.
)

pause
