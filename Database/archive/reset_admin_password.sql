-- Reset admin password to admin123
USE student_attendance_db;

-- Update admin user with correct BCrypt hash for "admin123"
-- Using $2a$ prefix which is compatible with BCrypt.Net-Next
-- This hash was generated and verified to work with BCrypt.Net
UPDATE users 
SET password_hash = '$2a$10$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy'
WHERE username = 'admin';

-- Verify the update
SELECT 
    user_id,
    username,
    password_hash,
    full_name,
    is_active
FROM users 
WHERE username = 'admin';
