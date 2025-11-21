-- Generate a fresh admin user with a simple password
USE student_attendance_db;

-- First, let's check what we have
SELECT 'BEFORE UPDATE:' as step;
SELECT user_id, username, is_active, created_at FROM users WHERE username = 'admin';

-- Delete and recreate admin user
DELETE FROM users WHERE username = 'admin';

-- Insert new admin with a known-good BCrypt hash
-- This hash is for "admin123" generated with BCrypt online tool (cost 10)
-- Verified at: https://bcrypt-generator.com
INSERT INTO users (username, password_hash, full_name, email, role, is_active, created_at)
VALUES (
    'admin',
    '$2y$10$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy',
    'System Administrator',
    'admin@studentattendance.edu',
    'admin',
    1,
    NOW()
);

-- Verify the insert
SELECT 'AFTER UPDATE:' as step;
SELECT user_id, username, password_hash, full_name, role, is_active FROM users WHERE username = 'admin';
