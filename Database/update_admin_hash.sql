USE student_attendance_db;

UPDATE users 
SET password_hash = '$2a$11$8pGt.qb8oQRO9LlGPbO2DOxFd7usI/Va8a4KsxJ0vAvFw1HdTH/oe'
WHERE username = 'admin';

SELECT user_id, username, password_hash, full_name 
FROM users 
WHERE username = 'admin';
