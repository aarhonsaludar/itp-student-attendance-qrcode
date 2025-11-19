# Student Attendance Database Schema - QR Code Scanner System

## Overview
This database schema is designed for a student attendance system using QR Code scanning. Optimized for MySqlConnector in C#.

## Database Structure
- 7 Tables (users, students, devices, scan_history, tokens, system_settings, system_logs)
- 5 Stored Procedures (registration, scanning, reporting)
- 4 Views (active students, recent scans, statistics)
- 3 Triggers (audit logging)

## Installation
1. Install MySQL Server
2. Run: mysql -u root -p < schema.sql
3. Verify: USE student_attendance_db; SHOW TABLES;

## Connection String for C#
Server=localhost;Database=student_attendance_db;Uid=root;Pwd=yourpassword;AllowUserVariables=true;CharSet=utf8mb4;

## Default Login
Username: admin | Password: admin123 (hash with BCrypt on first run)

## Features
- QR Code Only (simplified)
- Duplicate Detection (5-minute window)
- Audit Logging
- Performance Optimized
- MySqlConnector Ready
