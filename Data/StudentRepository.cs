using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using MySqlConnector;
using ITP104_FINAL_PROJECT.Models;

namespace ITP104_FINAL_PROJECT.Data
{
    public class StudentRepository
    {
        /// <summary>
        /// Register a new student with individual parameters (convenience method)
        /// </summary>
        public async Task<(bool Success, string Message, int StudentId)> RegisterStudentAsync(
            string studentNumber, string firstName, string middleName, string lastName,
            string email, string phone, string yearLevel, string program, 
            string section, string qrCodeData, DateTime enrollmentDate)
        {
            var student = new Student
            {
                StudentNumber = studentNumber,
                FirstName = firstName,
                MiddleName = middleName,
                LastName = lastName,
                Email = email,
                Phone = phone,
                YearLevel = yearLevel,
                Program = program,
                Section = section,
                QRCodeData = qrCodeData,
                EnrollmentDate = enrollmentDate
            };

            return await RegisterStudentAsync(student);
        }

        /// <summary>
        /// Register a new student
        /// </summary>
        public async Task<(bool Success, string Message, int StudentId)> RegisterStudentAsync(Student student)
        {
            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    await connection.OpenAsync();
                    
                    // Check if student number exists
                    if (await IsStudentNumberExistsAsync(student.StudentNumber))
                    {
                        return (false, "Student number already exists", 0);
                    }

                    string query = @"INSERT INTO students (student_number, first_name, middle_name, last_name, email, phone, year_level, program, section, qr_code_data, enrollment_date, status, created_at)
                                   VALUES (@studentNumber, @firstName, @middleName, @lastName, @email, @phone, @yearLevel, @program, @section, @qrCodeData, @enrollmentDate, 'Active', CURRENT_TIMESTAMP);
                                   SELECT LAST_INSERT_ID();";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@studentNumber", student.StudentNumber);
                        command.Parameters.AddWithValue("@firstName", student.FirstName);
                        command.Parameters.AddWithValue("@middleName", student.MiddleName ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@lastName", student.LastName);
                        command.Parameters.AddWithValue("@email", student.Email ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@phone", student.Phone ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@yearLevel", student.YearLevel);
                        command.Parameters.AddWithValue("@program", student.Program);
                        command.Parameters.AddWithValue("@section", student.Section ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@qrCodeData", student.QRCodeData);
                        command.Parameters.AddWithValue("@enrollmentDate", student.EnrollmentDate);

                        var result = await command.ExecuteScalarAsync();
                        int studentId = Convert.ToInt32(result);

                        return (true, "Student registered successfully", studentId);
                    }
                }
            }
            catch (Exception ex)
            {
                return (false, $"Database error: {ex.Message}", 0);
            }
        }

        /// <summary>
        /// Get student by ID
        /// </summary>
        public async Task<Student> GetByIdAsync(int studentId)
        {
            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    await connection.OpenAsync();
                    string query = @"SELECT student_id, student_number, first_name, middle_name, last_name, 
                                    email, phone, year_level, program, section, qr_code_data, photo_path, 
                                    status, enrollment_date, created_at, updated_at
                                    FROM students WHERE student_id = @studentId";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@studentId", studentId);
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return MapStudent(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving student: {ex.Message}", ex);
            }

            return null;
        }

        /// <summary>
        /// Get student by QR code data
        /// </summary>
        public async Task<Student> GetByQRCodeAsync(string qrCodeData)
        {
            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    await connection.OpenAsync();
                    string query = @"SELECT student_id, student_number, first_name, middle_name, last_name, 
                                    email, phone, year_level, program, section, qr_code_data, photo_path, 
                                    status, enrollment_date, created_at, updated_at
                                    FROM students WHERE qr_code_data = @qrCodeData";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@qrCodeData", qrCodeData);
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return MapStudent(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving student by QR: {ex.Message}", ex);
            }

            return null;
        }

        /// <summary>
        /// Get all students (optionally filter by active status)
        /// </summary>
        public async Task<List<Student>> GetAllAsync(bool activeOnly = true)
        {
            var students = new List<Student>();

            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    await connection.OpenAsync();
                    string query = @"SELECT student_id, student_number, first_name, middle_name, last_name, 
                                    email, phone, year_level, program, section, qr_code_data, photo_path, 
                                    status, enrollment_date, created_at, updated_at
                                    FROM students";
                    
                    if (activeOnly)
                    {
                        query += " WHERE status = 'Active'";
                    }
                    
                    query += " ORDER BY last_name, first_name";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                students.Add(MapStudent(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving students: {ex.Message}", ex);
            }

            return students;
        }

        /// <summary>
        /// Update student information
        /// </summary>
        public async Task<bool> UpdateAsync(Student student)
        {
            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    await connection.OpenAsync();
                    string query = @"UPDATE students SET 
                                    student_number = @studentNumber,
                                    first_name = @firstName,
                                    middle_name = @middleName,
                                    last_name = @lastName,
                                    email = @email,
                                    phone = @phone,
                                    year_level = @yearLevel,
                                    program = @program,
                                    section = @section,
                                    qr_code_data = @qrCodeData,
                                    photo_path = @photoPath,
                                    status = @status,
                                    updated_at = CURRENT_TIMESTAMP
                                    WHERE student_id = @studentId";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@studentId", student.StudentId);
                        command.Parameters.AddWithValue("@studentNumber", student.StudentNumber);
                        command.Parameters.AddWithValue("@firstName", student.FirstName);
                        command.Parameters.AddWithValue("@middleName", student.MiddleName ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@lastName", student.LastName);
                        command.Parameters.AddWithValue("@email", student.Email ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@phone", student.Phone ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@yearLevel", student.YearLevel);
                        command.Parameters.AddWithValue("@program", student.Program);
                        command.Parameters.AddWithValue("@section", student.Section ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@qrCodeData", student.QRCodeData);
                        command.Parameters.AddWithValue("@photoPath", student.PhotoPath ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@status", student.Status);

                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating student: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Delete student (soft delete by setting status to 'inactive')
        /// </summary>
        public async Task<bool> DeleteAsync(int studentId)
        {
            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    await connection.OpenAsync();
                    string query = "UPDATE students SET status = 'Inactive', updated_at = CURRENT_TIMESTAMP WHERE student_id = @studentId";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@studentId", studentId);
                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting student: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Search students by name or student number
        /// </summary>
        public async Task<List<Student>> SearchAsync(string searchTerm)
        {
            var students = new List<Student>();

            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    await connection.OpenAsync();
                    string query = @"SELECT student_id, student_number, first_name, middle_name, last_name, 
                                    email, phone, year_level, program, section, qr_code_data, photo_path, 
                                    status, enrollment_date, created_at, updated_at
                                    FROM students 
                                    WHERE status = 'active' 
                                    AND (student_number LIKE @search 
                                         OR first_name LIKE @search 
                                         OR last_name LIKE @search
                                         OR CONCAT(first_name, ' ', last_name) LIKE @search)
                                    ORDER BY last_name, first_name";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@search", $"%{searchTerm}%");
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                students.Add(MapStudent(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error searching students: {ex.Message}", ex);
            }

            return students;
        }

        /// <summary>
        /// Check if a student number already exists
        /// </summary>
        public async Task<bool> IsStudentNumberExistsAsync(string studentNumber)
        {
            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    await connection.OpenAsync();
                    // Check if student number exists, regardless of status
                    string query = "SELECT COUNT(*) FROM students WHERE student_number = @studentNumber";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@studentNumber", studentNumber);
                        var result = await command.ExecuteScalarAsync();
                        return Convert.ToInt64(result) > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error checking student number: {ex.Message}", ex);
            }
        }

        private Student MapStudent(MySqlDataReader reader)
        {
            return new Student
            {
                StudentId = Convert.ToInt32(reader["student_id"]),
                StudentNumber = reader["student_number"].ToString(),
                FirstName = reader["first_name"].ToString(),
                MiddleName = reader["middle_name"] != DBNull.Value ? reader["middle_name"].ToString() : null,
                LastName = reader["last_name"].ToString(),
                Email = reader["email"] != DBNull.Value ? reader["email"].ToString() : null,
                Phone = reader["phone"] != DBNull.Value ? reader["phone"].ToString() : null,
                YearLevel = reader["year_level"].ToString(),
                Program = reader["program"].ToString(),
                Section = reader["section"] != DBNull.Value ? reader["section"].ToString() : null,
                QRCodeData = reader["qr_code_data"].ToString(),
                PhotoPath = reader["photo_path"] != DBNull.Value ? reader["photo_path"].ToString() : null,
                Status = reader["status"].ToString(),
                EnrollmentDate = Convert.ToDateTime(reader["enrollment_date"]),
                CreatedAt = Convert.ToDateTime(reader["created_at"]),
                UpdatedAt = reader["updated_at"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(reader["updated_at"]) : null
            };
        }
    }
}
