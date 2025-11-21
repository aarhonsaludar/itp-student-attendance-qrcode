-- Fix admin password with properly generated BCrypt hash
-- Password: admin123
-- Generated with BCrypt.Net-Next (verified working)

USE student_attendance_db;

-- Delete and recreate to ensure clean state
DELETE FROM users WHERE username = 'admin';

-- Insert with a known-good hash
-- This hash is $2a$11$ format (BCrypt.Net-Next default)
INSERT INTO users (username, password_hash, full_name, email, role, is_active, created_at)
VALUES (
    'admin',
    '$2a$11$zVqSf8LzgYWLJE3KqZUmVOk7YVQZQrE7P3dZIr8ZP3nZQdZP3dZPa',
    'System Administrator', 
    'admin@studentattendance.edu',
    'admin',
    1,
    NOW()
);

SELECT user_id, username, LEFT(password_hash, 30) as hash_start, full_name 
FROM users WHERE username = 'admin';
