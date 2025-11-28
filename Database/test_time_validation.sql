-- ============================================
-- Test Time Validation
-- This script checks if client/server times match
-- ============================================

USE student_attendance_db;

-- Get current server time
SELECT 
    'SERVER TIME (Database)' as Source,
    NOW() as CurrentTime,
    DATE(NOW()) as CurrentDate;

-- Show what client time should be (approximately)
SELECT 
    'Expected Client Time' as Note,
    'Should match server time within 2 minutes' as Requirement;

-- Check recent validation logs
SELECT 
    created_at,
    event_type,
    details,
    category
FROM system_logs
WHERE category = 'time_validation'
ORDER BY created_at DESC
LIMIT 10;

-- Check if any tampering was detected
SELECT 
    COUNT(*) as TamperingAttempts,
    MAX(created_at) as LastAttempt
FROM system_logs
WHERE category = 'time_tampering';
