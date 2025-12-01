-- ============================================
-- Database Cleanup: Remove Unused Objects
-- ============================================
-- Purpose: Remove stored procedures, views, and tables that are not used by the application
-- Date: 2025-11-29
-- IMPORTANT: Backup your database before running this script!
-- ============================================

USE student_attendance_db;

-- ============================================
-- Drop Unused Stored Procedures
-- ============================================

DROP PROCEDURE IF EXISTS sp_get_scan_history;
DROP PROCEDURE IF EXISTS sp_accept_scan;
DROP PROCEDURE IF EXISTS sp_decline_scan;

-- ============================================
-- Drop Unused Views
-- ============================================

DROP VIEW IF EXISTS vw_active_students;
DROP VIEW IF EXISTS vw_daily_offline_scans;
DROP VIEW IF EXISTS vw_device_stats;
DROP VIEW IF EXISTS vw_pending_reviews;
DROP VIEW IF EXISTS vw_scans_pending_review;

-- ============================================
-- Drop Unused Table
-- ============================================

DROP TABLE IF EXISTS system_logs_archive;

-- ============================================
-- Cleanup completed
-- ============================================
SELECT 'Database cleanup completed: Removed 3 unused procedures, 5 unused views, and 1 unused table' AS Status;
