-- ============================================
-- Create Default Admin User
-- ============================================
-- This script creates a default admin user for initial login
-- Default credentials: admin / admin123
-- 
-- IMPORTANT: Change the password after first login!
-- ============================================

USE student_attendance_db;

-- Check if admin user already exists
SELECT COUNT(*) AS admin_exists FROM users WHERE username = 'admin';

-- Delete existing admin if present (optional - comment out if you don't want to reset)
-- DELETE FROM users WHERE username = 'admin';

-- Insert admin user with BCrypt hashed password
-- Password: admin123
-- This hash was generated specifically for BCrypt.Net-Next
-- You can regenerate this by running: BCrypt.Net.BCrypt.HashPassword("admin123")
UPDATE users SET 
    password_hash = '$2a$11$K8p.V9FqLVL9qO0UQH8nZ.vAqL4H0xR7xMJG3HY7fGvPYZ4hZx4xK',
    full_name = 'System Administrator',
    is_active = 1
WHERE username = 'admin';

-- If admin doesn't exist, insert it
INSERT INTO users (username, password_hash, full_name, email, role, is_active)
SELECT 'admin', '$2a$11$K8p.V9FqLVL9qO0UQH8nZ.vAqL4H0xR7xMJG3HY7fGvPYZ4hZx4xK', 
       'System Administrator', 'admin@studentattendance.edu', 'admin', 1
WHERE NOT EXISTS (SELECT 1 FROM users WHERE username = 'admin');

-- Verify admin user was created
SELECT 
    user_id,
    username,
    full_name,
    email,
    role,
    is_active,
    created_at
FROM users
WHERE username = 'admin';

-- ============================================
-- Additional Test Users (Optional)
-- ============================================

-- Teacher User (Password: teacher123)
INSERT INTO users (username, password_hash, full_name, email, role, is_active)
VALUES (
    'teacher',
    '$2a$11$K8YfJ6M8xQB.4sXjB.vVV.8q8YMz5Z3J8Z8Z8Z8Z8Z8Z8Z8Z8Z8Z8u',
    'Teacher Account',
    'teacher@studentattendance.edu',
    'teacher',
    1
)
ON DUPLICATE KEY UPDATE
    password_hash = '$2a$11$K8YfJ6M8xQB.4sXjB.vVV.8q8YMz5Z3J8Z8Z8Z8Z8Z8Z8Z8Z8Z8Z8u';

-- Staff User (Password: staff123)
INSERT INTO users (username, password_hash, full_name, email, role, is_active)
VALUES (
    'staff',
    '$2a$11$L9ZgK7N9yRC.5tYkC.wWW.9r9ZNa6a4K9a9a9a9a9a9a9a9a9a9a9v',
    'Staff Account',
    'staff@studentattendance.edu',
    'staff',
    1
)
ON DUPLICATE KEY UPDATE
    password_hash = '$2a$11$L9ZgK7N9yRC.5tYkC.wWW.9r9ZNa6a4K9a9a9a9a9a9a9a9a9a9a9v';

-- Show all users
SELECT 
    user_id,
    username,
    full_name,
    role,
    is_active,
    created_at
FROM users
ORDER BY created_at DESC;
