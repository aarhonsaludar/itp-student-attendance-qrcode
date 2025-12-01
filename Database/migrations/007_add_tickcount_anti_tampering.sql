-- ============================================
-- Migration 007: Add TickCount Anti-Tampering Fields
-- Adds Environment.TickCount64 tracking for offline time tampering detection
-- ============================================
-- Date: December 1, 2025
-- ============================================

USE student_attendance_db;

-- Add TickCount columns for offline tampering detection
ALTER TABLE scan_history
ADD COLUMN time_in_tick_count BIGINT NULL COMMENT 'Environment.TickCount64 at Time In - tamper-proof elapsed time',
ADD COLUMN time_out_tick_count BIGINT NULL COMMENT 'Environment.TickCount64 at Time Out - tamper-proof elapsed time',
ADD COLUMN connection_drop_count INT NULL DEFAULT 0 COMMENT 'Number of connection drops during session',
ADD COLUMN offline_duration_minutes DOUBLE NULL COMMENT 'Total offline duration in minutes';

-- Add indexes for performance
CREATE INDEX idx_tickcount_validation ON scan_history(time_in_tick_count, time_out_tick_count);

SELECT 
    'Migration 007 completed: TickCount anti-tampering fields added' AS Status,
    NOW() AS CompletedAt;
