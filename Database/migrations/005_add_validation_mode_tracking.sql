-- ============================================
-- Migration 005: Add Validation Mode Tracking
-- Solution 1: Time-Out Validation Against Time-In
-- ============================================
-- Purpose: Track whether time-in and time-out were validated online or offline
--          This prevents WiFi disconnect + time tampering exploit
-- Date: December 1, 2025
-- ============================================

USE student_attendance_db;

-- Add columns to track validation mode for time-in and time-out
ALTER TABLE scan_history
ADD COLUMN time_in_validation_mode VARCHAR(20) DEFAULT NULL AFTER validation_status,
ADD COLUMN time_out_validation_mode VARCHAR(20) DEFAULT NULL AFTER time_in_validation_mode;

-- Add index for quick filtering of mixed-mode scans (high risk)
CREATE INDEX idx_validation_modes ON scan_history(time_in_validation_mode, time_out_validation_mode, status);

-- Update existing records to set validation mode based on validation_status
UPDATE scan_history
SET time_in_validation_mode = CASE 
    WHEN validation_status = 'offline_mode' THEN 'offline'
    WHEN validation_status = 'verified' THEN 'online'
    ELSE 'unknown'
END
WHERE time_in_validation_mode IS NULL;

-- Set time_out_validation_mode same as time_in for existing completed scans
UPDATE scan_history
SET time_out_validation_mode = time_in_validation_mode
WHERE time_out IS NOT NULL AND time_out_validation_mode IS NULL;

-- ============================================
-- Migration completed successfully
-- ============================================
SELECT 
    'Migration 005 completed: Added validation mode tracking columns' AS Status,
    COUNT(*) AS total_records_updated
FROM scan_history
WHERE time_in_validation_mode IS NOT NULL;
