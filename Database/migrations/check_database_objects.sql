-- ============================================
-- Check Database Objects
-- ============================================

USE student_attendance_db;

-- List all stored procedures
SELECT 
    'PROCEDURE' as object_type,
    ROUTINE_NAME as object_name
FROM information_schema.ROUTINES 
WHERE ROUTINE_SCHEMA = 'student_attendance_db' 
AND ROUTINE_TYPE = 'PROCEDURE';

-- List all views
SELECT 
    'VIEW' as object_type,
    TABLE_NAME as object_name
FROM information_schema.VIEWS 
WHERE TABLE_SCHEMA = 'student_attendance_db';

-- List all tables
SELECT 
    'TABLE' as object_type,
    TABLE_NAME as object_name
FROM information_schema.TABLES 
WHERE TABLE_SCHEMA = 'student_attendance_db' 
AND TABLE_TYPE = 'BASE TABLE'
ORDER BY TABLE_NAME;

-- List all triggers
SELECT 
    'TRIGGER' as object_type,
    TRIGGER_NAME as object_name,
    EVENT_OBJECT_TABLE as on_table
FROM information_schema.TRIGGERS 
WHERE TRIGGER_SCHEMA = 'student_attendance_db';
