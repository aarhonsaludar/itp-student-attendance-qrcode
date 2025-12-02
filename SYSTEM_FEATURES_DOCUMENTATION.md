# Student Attendance QR Code System

## Complete Features Documentation

**Project:** ITP104 Final Project - Student Attendance Management System  
**Version:** 1.0  
**Last Updated:** December 2, 2025  
**Developer:** Jaycee  
**Technology Stack:** C# Windows Forms, MySQL Database, .NET Framework 4.7.2

---

## Table of Contents

1. [System Overview](#1-system-overview)
2. [Application Screens](#2-application-screens)
3. [Core Features](#3-core-features)
4. [Security Features](#4-security-features)
5. [Database Structure](#5-database-structure)
6. [Technical Architecture](#6-technical-architecture)
7. [Installation Requirements](#7-installation-requirements)

---

## 1. System Overview

### Purpose

A secure, modern Windows desktop application for managing student attendance using QR code technology with multi-layer security verification.

### Key Highlights

- ✅ QR Code-based attendance tracking
- ✅ Email OTP verification for attendance confirmation
- ✅ Time tampering detection (prevents clock manipulation)
- ✅ Offline mode support with manual review flags
- ✅ Real-time dashboard analytics
- ✅ Comprehensive audit trail

### Target Users

| User Type         | Access Level                              |
| ----------------- | ----------------------------------------- |
| **Administrator** | Full system access (single admin account) |
| Students          | Use QR codes for check-in/check-out       |

---

## 2. Application Screens

### 2.1 Splash Screen (`SplashScreen.cs`)

- **Purpose:** Application loading screen
- **Features:**
  - Animated loading indicator
  - Application branding display
  - System initialization

### 2.2 Login Screen (`LoginScreen.cs`)

- **Purpose:** User authentication
- **Features:**
  - Username/password login
  - BCrypt password verification
  - Database connection validation
  - Session management
  - Error handling with user-friendly messages

### 2.3 Main Dashboard (`MainDashboard.cs`)

- **Purpose:** Central hub for all system functions
- **Features:**

  #### Dashboard Statistics (Real-time)

  | Statistic      | Description                                |
  | -------------- | ------------------------------------------ |
  | Total Students | Count of active students in database       |
  | Scans Today    | Number of attendance scans for current day |
  | Most Used Scan | QR Code vs Manual scan comparison          |

  #### Recent Scans Feed

  - Displays last 10 scan records
  - Shows: Student ID, Name, Course, Scan Type, Time
  - Auto-refreshes periodically

  #### Navigation Menu

  - Dashboard (Home)
  - Register Student
  - Scan QR Code
  - Student Records
  - Scan History
  - Settings
  - Logout

### 2.4 Student Registration (`StudentRegistration.cs`)

- **Purpose:** Register new students with QR code generation
- **Required Fields:**
  | Field | Validation |
  |-------|------------|
  | Student ID | Unique, 5-50 characters |
  | Full Name | First, Middle (optional), Last name |
  | Email | Valid email format (for OTP) |
  | Phone | Optional, valid format |
  | Sex | Male/Female dropdown |
  | Course | Computer Science, IT, Nursing, Education, Psychology |
  | Year Level | 1st Year - 4th Year |
  | Section | Optional text field |
  | Home Address | Optional text field |

- **QR Code Generation:**
  - Unique QR code per student
  - Format: `ID:{StudentID}|Name:{FullName}|Email:{Email}|Course:{Course}|Year:{Year}`
  - Downloadable as image file
  - Displayed in preview box

### 2.5 QR Scanner Form (`QRScannerForm.cs`)

- **Purpose:** Real-time QR code scanning for attendance
- **Features:**

  #### Camera Integration

  - Webcam device selection dropdown
  - Start/Stop camera controls
  - Real-time video feed display

  #### Scan Box Overlay

  - 600x500 pixel centered scan area
  - Visual corner brackets
  - Semi-transparent overlay outside scan area
  - Color-coded status (Green = Ready, Orange = Processing)

  #### Scan Processing

  - 2-second cooldown between scans
  - Automatic student lookup from QR data
  - Time validation before OTP
  - OTP dialog trigger on valid scan

  #### Attendance Flow

  ```
  Scan QR → Validate Time → Determine Time In/Out → Send OTP → Verify → Record
  ```

### 2.6 OTP Verification Dialog (`OTPVerificationDialog.cs`)

- **Purpose:** Email-based attendance verification
- **Features:**
  - 6-digit OTP code entry
  - 5-minute expiration countdown
  - Visual timer with color coding (Green → Yellow → Red)
  - Resend OTP option (max 3 times)
  - 3 failed attempts = session locked
  - Time In (Green) / Time Out (Orange) visual distinction

### 2.7 Scan Details Dialog (`ScanDetailsDialog.cs`)

- **Purpose:** Display successful scan confirmation
- **Shows:**
  - Student photo (if available)
  - Student name and ID
  - Time In / Time Out timestamp
  - Course and year information
  - Validation status

### 2.8 Student Record Screen (`StudentRecordScreen.cs`)

- **Purpose:** View and manage individual student profiles
- **Features:**

  #### Student Information Display

  - Profile photo (uploadable)
  - Student ID, Name, Email, Phone
  - Course, Year Level, Section
  - Home Address
  - Status (Active/Inactive/Suspended)
  - Enrollment date

  #### Student QR Code

  - Full-size QR code display
  - Click to download option

  #### Scan History Table

  - All attendance records for selected student
  - Date, Time In, Time Out, Duration, Status columns
  - Export to CSV functionality

  #### Actions

  - Edit student information
  - Upload/change profile photo
  - Back to scan navigation

### 2.9 Edit Student Dialog (`EditStudentDialog.cs`)

- **Purpose:** Modify existing student information
- **Editable Fields:**
  - Full Name
  - Email (with OTP verification if changed)
  - Phone
  - Course
  - Year Level
  - Section
  - Sex
  - Home Address
- **Read-Only:** Student ID (cannot be changed)
- **Validation:** Same as registration form

### 2.10 Scan History Screen (`ScanHistoryScreen.cs`)

- **Purpose:** Browse all attendance records system-wide
- **Features:**

  #### Filters

  | Filter | Options                         |
  | ------ | ------------------------------- |
  | Date   | Date picker (defaults to today) |
  | Search | Student name or ID              |

  #### Data Grid Columns

  - Scan ID
  - Student Number
  - Student Name
  - Course/Program
  - Time In
  - Time Out
  - Duration
  - Status (Success, For Review, Failed)
  - Validation Mode

  #### Pagination

  - 50 records per page
  - Previous/Next navigation
  - Total record count display

  #### Export

  - Export to CSV file
  - Includes all visible columns

### 2.11 Settings Screen (`SettingsScreen.cs`)

- **Purpose:** Configure system settings
- **Settings Categories:**

  #### Scanner Settings

  | Setting            | Default | Description             |
  | ------------------ | ------- | ----------------------- |
  | QR Scanner Enabled | True    | Enable/disable scanning |
  | Connection Timeout | 30 sec  | Scanner timeout value   |
  | Beep on Scan       | True    | Audio feedback          |

  #### System Settings

  | Setting           | Default | Description        |
  | ----------------- | ------- | ------------------ |
  | Auto-Logout Timer | 15 min  | Session timeout    |
  | Theme             | Light   | UI theme selection |
  | Language          | English | System language    |

  #### Database Info (Read-only)

  - Server Address: localhost
  - Port: 3306

---

## 3. Core Features

### 3.1 QR Code Generation

- **Library:** QRCoder
- **Format:** Structured data string with pipe separators
- **Contents:** Student ID, Name, Email, Course, Year
- **Output:** Bitmap image, downloadable

### 3.2 QR Code Scanning

- **Libraries:** AForge.Video, ZXing.Net
- **Camera Support:** All DirectShow compatible webcams
- **Scan Area:** Centered 600x500 pixel box
- **Detection:** Automatic, continuous scanning
- **Cooldown:** 2 seconds between scans

### 3.3 OTP Email Verification

- **Library:** MailKit
- **SMTP:** Gmail (smtp.gmail.com:587 with TLS)
- **OTP Length:** 6 digits
- **Expiry:** 5 minutes
- **Max Attempts:** 3 per session
- **Max Resends:** 3 per session
- **Email Template:** HTML formatted with branding

### 3.4 Attendance Tracking

- **Time In:** First scan of the day
- **Time Out:** Second scan after Time In
- **Auto-Detection:** System determines scan type automatically
- **Duplicate Prevention:** Stored procedure checks for existing Time In

### 3.5 Time Validation (Anti-Tampering)

- **Purpose:** Prevent clock manipulation cheating
- **Sources:** Google.com, TimeAPI.io, Microsoft.com
- **Max Drift:** 5 minutes tolerance
- **Checks:**
  - Date match validation
  - Time drift calculation
  - Offline mode detection

### 3.6 Offline Mode

- **Detection:** When internet time sources unavailable
- **Behavior:**
  - Allows attendance with warning
  - Flags record for manual review
  - Uses TickCount for tamper detection

### 3.7 Anti-Tampering (Offline)

- **Technology:** `Stopwatch.GetTimestamp()`
- **Purpose:** Detect clock manipulation even without internet
- **Metrics:**
  - Time In TickCount
  - Time Out TickCount
  - Real elapsed time calculation
  - Connection drop count

---

## 4. Security Features

### 4.1 Authentication

| Feature            | Implementation           |
| ------------------ | ------------------------ |
| Password Hashing   | BCrypt.Net               |
| Session Management | Static user reference    |
| Login Validation   | Database + BCrypt.Verify |

### 4.2 Time Tampering Prevention

```
┌─────────────────────────────────────────────────┐
│           TIME VALIDATION FLOW                  │
├─────────────────────────────────────────────────┤
│ 1. Get client system time                       │
│ 2. Fetch trusted internet time (Google/API)    │
│ 3. Compare date (must match)                    │
│ 4. Calculate time drift (max 5 min)            │
│ 5. If failed → BLOCK attendance                │
│ 6. If offline → Allow but FLAG for review      │
└─────────────────────────────────────────────────┘
```

### 4.3 OTP Security

- One-time use codes
- 5-minute expiration
- Max 3 failed attempts
- Session invalidation on expiry
- Email-bound verification

### 4.4 Data Validation

| Layer    | Validation                     |
| -------- | ------------------------------ |
| Client   | Required fields, format checks |
| Service  | InputValidator class           |
| Database | Constraints, foreign keys      |

### 4.5 Audit Trail

- All actions logged to `system_logs` table
- Includes: User, Action, Timestamp, Old/New values
- Error logging via `ErrorLoggingService`

---

## 5. Database Structure

### 5.1 Tables

#### `users`

| Column        | Type         | Description            |
| ------------- | ------------ | ---------------------- |
| user_id       | INT          | Primary key            |
| username      | VARCHAR(50)  | Unique login name      |
| password_hash | VARCHAR(255) | BCrypt hashed password |
| full_name     | VARCHAR(100) | Display name           |
| email         | VARCHAR(100) | Contact email          |
| role          | ENUM         | admin, staff, teacher  |
| is_active     | BOOLEAN      | Account status         |
| last_login    | TIMESTAMP    | Last login time        |

#### `students`

| Column          | Type         | Description                 |
| --------------- | ------------ | --------------------------- |
| student_id      | INT          | Primary key                 |
| student_number  | VARCHAR(50)  | Unique student ID           |
| first_name      | VARCHAR(50)  | First name                  |
| middle_name     | VARCHAR(50)  | Middle name (optional)      |
| last_name       | VARCHAR(50)  | Last name                   |
| email           | VARCHAR(100) | Email for OTP               |
| phone           | VARCHAR(20)  | Contact number              |
| sex             | ENUM         | Male, Female                |
| year_level      | ENUM         | 1, 2, 3, 4, Graduate        |
| program         | VARCHAR(100) | Course/Program              |
| section         | VARCHAR(50)  | Class section               |
| home_address    | VARCHAR(255) | Address                     |
| qr_code_data    | TEXT         | QR code content             |
| photo_path      | VARCHAR(255) | Profile photo               |
| status          | ENUM         | Active, Inactive, Suspended |
| enrollment_date | DATE         | Registration date           |

#### `scan_history`

| Column             | Type        | Description                 |
| ------------------ | ----------- | --------------------------- |
| scan_id            | INT         | Primary key                 |
| student_id         | INT         | Foreign key to students     |
| device_id          | INT         | Foreign key to devices      |
| scan_type          | ENUM        | QR, MANUAL                  |
| scan_datetime      | DATETIME    | Time In timestamp           |
| time_out           | DATETIME    | Time Out timestamp          |
| status             | ENUM        | success, failed, for_review |
| validation_status  | VARCHAR(30) | verified, offline_mode      |
| requires_review    | BOOLEAN     | Flag for admin review       |
| client_time        | DATETIME    | Device clock time           |
| server_time        | DATETIME    | Internet time               |
| time_drift_seconds | INT         | Difference in seconds       |

#### `devices`

| Column      | Type         | Description                   |
| ----------- | ------------ | ----------------------------- |
| device_id   | INT          | Primary key                   |
| device_name | VARCHAR(100) | Device identifier             |
| device_type | ENUM         | QR_SCANNER                    |
| location    | VARCHAR(100) | Physical location             |
| status      | ENUM         | active, inactive, maintenance |

#### `system_settings`

| Column           | Type         | Description                   |
| ---------------- | ------------ | ----------------------------- |
| setting_id       | INT          | Primary key                   |
| setting_key      | VARCHAR(100) | Setting name                  |
| setting_value    | TEXT         | Setting value                 |
| setting_category | ENUM         | Scanner, System, Database, UI |

#### `system_logs`

| Column     | Type         | Description          |
| ---------- | ------------ | -------------------- |
| log_id     | INT          | Primary key          |
| user_id    | INT          | Who performed action |
| action     | VARCHAR(100) | What was done        |
| table_name | VARCHAR(50)  | Affected table       |
| record_id  | INT          | Affected record      |
| old_value  | TEXT         | Previous value       |
| new_value  | TEXT         | New value            |
| timestamp  | TIMESTAMP    | When it happened     |

---

## 6. Technical Architecture

### 6.1 Project Structure

```
ITP104-FINAL-PROJECT/
├── Data/                          # Data Access Layer
│   ├── DatabaseHelper.cs          # Connection management
│   ├── StudentRepository.cs       # Student CRUD operations
│   ├── ScanHistoryRepository.cs   # Attendance records
│   ├── UserRepository.cs          # Authentication
│   └── SettingsRepository.cs      # System settings
├── Models/                        # Data Models
│   ├── Student.cs
│   ├── ScanHistory.cs
│   ├── User.cs
│   ├── OTPSession.cs
│   ├── AttendanceType.cs
│   └── SystemSetting.cs
├── Services/                      # Business Logic
│   ├── OTPService.cs              # Email OTP handling
│   ├── TimeValidationService.cs   # Anti-tampering
│   ├── ErrorLoggingService.cs     # Logging
│   └── InputValidator.cs          # Validation
├── Database/                      # SQL Scripts
│   ├── schema.sql                 # Database schema
│   └── migrations/                # Schema updates
├── Resources/                     # Images, icons
├── [Form].cs                      # UI Forms
├── [Form].Designer.cs             # Form layouts
└── App.config                     # Connection strings
```

### 6.2 Dependencies (NuGet Packages)

| Package                 | Version | Purpose               |
| ----------------------- | ------- | --------------------- |
| MySqlConnector          | Latest  | MySQL database driver |
| BCrypt.Net-Next         | Latest  | Password hashing      |
| QRCoder                 | Latest  | QR code generation    |
| ZXing.Net               | Latest  | QR code reading       |
| AForge.Video.DirectShow | Latest  | Webcam access         |
| MailKit                 | Latest  | Email sending         |
| Guna.UI2.WinForms       | Latest  | Modern UI controls    |

### 6.3 Configuration

**App.config Connection String:**

```xml
<connectionStrings>
  <add name="StudentAttendanceDB"
       connectionString="Server=localhost;Database=student_attendance_db;Uid=root;Pwd=admin;"/>
</connectionStrings>
```

---

## 7. Installation Requirements

### 7.1 System Requirements

| Requirement    | Specification                                |
| -------------- | -------------------------------------------- |
| OS             | Windows 10/11                                |
| .NET Framework | 4.7.2 or higher                              |
| RAM            | 4GB minimum                                  |
| Storage        | 100MB for application                        |
| Camera         | Webcam for QR scanning                       |
| Network        | Internet for OTP (optional for offline mode) |

### 7.2 Database Setup

1. Install MySQL Server 8.0+
2. Create database: `student_attendance_db`
3. Run `schema.sql`
4. Run migration scripts in order
5. Default admin: `admin` / `admin123`

### 7.3 Email Configuration

- Gmail account with App Password enabled
- Update `OTPService.cs` with credentials:
  - `SENDER_EMAIL`
  - `EMAIL_PASSWORD` (App Password)

---

## Summary

| Feature Category    | Count |
| ------------------- | ----- |
| Application Screens | 11    |
| Database Tables     | 6     |
| Core Features       | 7     |
| Security Layers     | 5     |

**System Status:** ✅ Production Ready (No known bugs)

---

_Documentation generated: December 2, 2025_
