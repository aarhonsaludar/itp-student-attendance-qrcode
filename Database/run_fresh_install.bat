@echo off
REM ============================================
REM Student Attendance System - Fresh Install
REM Batch file launcher for PowerShell script
REM ============================================

echo ============================================
echo STUDENT ATTENDANCE SYSTEM
echo Fresh Installation Setup
echo ============================================
echo.

REM Check if PowerShell is available
where powershell >nul 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: PowerShell not found!
    echo Please ensure PowerShell is installed on your system.
    pause
    exit /b 1
)

REM Check if script file exists
if not exist "run_fresh_install.ps1" (
    echo ERROR: PowerShell script not found: run_fresh_install.ps1
    echo Please ensure run_fresh_install.ps1 is in the same directory.
    pause
    exit /b 1
)

echo Starting PowerShell installation script...
echo.

REM Run PowerShell script with execution policy bypass
powershell.exe -ExecutionPolicy Bypass -File "run_fresh_install.ps1"

echo.
echo Batch script completed.
pause
