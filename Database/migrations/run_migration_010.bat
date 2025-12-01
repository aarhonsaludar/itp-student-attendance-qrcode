@echo off
REM ============================================
REM Run Migration 010: Fix Missing scan_data Field
REM ============================================
echo Running Migration 010...
echo.

mysql -u root -p student_attendance_db < "%~dp0010_fix_missing_scan_data.sql"

echo.
echo Migration 010 completed!
pause
