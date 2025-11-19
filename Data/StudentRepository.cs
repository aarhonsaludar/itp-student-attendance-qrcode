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
            string studentNumber,
            string firstName,
            string middleName,
            string lastName,
            string email,
            string phone,
            string yearLevel,
            string program,
            string section,
            string qrCodeData,
            DateTime enrollmentDate)
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
                EnrollmentDate = enrollmentDate,
                Status = "Active"
            };

            return await RegisterStudentAsync(student);
        }

        /// <summary>
        /// Register a new student using stored procedure
        /// </summary>
        public async Task<(bool success, string message, int studentId)> RegisterStudentAsync(Student student)
        {
            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    await connection.OpenAsync();
                    using (var command = new MySqlCommand("sp_register_student", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Input parameters
                        command.Parameters.AddWithValue("@p_student_number", student.StudentNumber);
                        command.Parameters.AddWithValue("@p_first_name", student.FirstName);
                        command.Parameters.AddWithValue("@p_middle_name", student.MiddleName ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@p_last_name", student.LastName);
                        command.Parameters.AddWithValue("@p_email", student.Email ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@p_phone", student.Phone ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@p_year_level", student.YearLevel);
                        command.Parameters.AddWithValue("@p_program", student.Program);
                        command.Parameters.AddWithValue("@p_section", student.Section ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@p_qr_code_data", student.QRCodeData);
                        command.Parameters.AddWithValue("@p_enrollment_date", student.EnrollmentDate);

                        // Output parameters
                        var studentIdParam = new MySqlParameter("@p_student_id", MySqlDbType.Int32) { Direction = ParameterDirection.Output };
                        var resultParam = new MySqlParameter("@p_result", MySqlDbType.VarChar, 100) { Direction = ParameterDirection.Output };

                        command.Parameters.Add(studentIdParam);
                        command.Parameters.Add(resultParam);

                        await command.ExecuteNonQueryAsync();

                        int studentId = studentIdParam.Value != DBNull.Value ? Convert.ToInt32(studentIdParam.Value) : -1;
                        string result = resultParam.Value?.ToString() ?? "Unknown error";

                        bool success = result == "SUCCESS";
                        string message = success ? "Student registered successfully" : result.Replace("ERROR: ", "");

                        return (success, message, studentId);
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
        /// Get student by QR code data using stored procedure
        /// </summary>
        public async Task<Student> GetByQRCodeAsync(string qrCodeData)
        {
            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    await connection.OpenAsync();
                    using (var command = new MySqlCommand("sp_get_student_by_qrcode", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Input parameter
                        command.Parameters.AddWithValue("@p_qr_code", qrCodeData);

                        // Output parameters
                        var studentIdParam = new MySqlParameter("@p_student_id", MySqlDbType.Int32) { Direction = ParameterDirection.Output };
                        var studentNumberParam = new MySqlParameter("@p_student_number", MySqlDbType.VarChar, 50) { Direction = ParameterDirection.Output };
                        var fullNameParam = new MySqlParameter("@p_full_name", MySqlDbType.VarChar, 200) { Direction = ParameterDirection.Output };
                        var emailParam = new MySqlParameter("@p_email", MySqlDbType.VarChar, 100) { Direction = ParameterDirection.Output };
                        var programParam = new MySqlParameter("@p_program", MySqlDbType.VarChar, 100) { Direction = ParameterDirection.Output };
                        var yearLevelParam = new MySqlParameter("@p_year_level", MySqlDbType.VarChar, 10) { Direction = ParameterDirection.Output };
                        var statusParam = new MySqlParameter("@p_status", MySqlDbType.VarChar, 20) { Direction = ParameterDirection.Output };

                        command.Parameters.Add(studentIdParam);
                        command.Parameters.Add(studentNumberParam);
                        command.Parameters.Add(fullNameParam);
                        command.Parameters.Add(emailParam);
                        command.Parameters.Add(programParam);
                        command.Parameters.Add(yearLevelParam);
                        command.Parameters.Add(statusParam);

                        await command.ExecuteNonQueryAsync();

                        // Check if student was found (student_id will be null if not found)
                        if (studentIdParam.Value == DBNull.Value || studentIdParam.Value == null)
                        {
                            return null;
                        }

                        // Parse the full name into first/middle/last
                        string fullName = fullNameParam.Value?.ToString() ?? "";
                        string[] nameParts = fullName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        string firstName = nameParts.Length > 0 ? nameParts[0] : "";
                        string lastName = nameParts.Length > 1 ? nameParts[nameParts.Length - 1] : "";
                        string middleName = nameParts.Length > 2 ? string.Join(" ", nameParts, 1, nameParts.Length - 2) : "";

                        return new Student
                        {
                            StudentId = Convert.ToInt32(studentIdParam.Value),
                            StudentNumber = studentNumberParam.Value?.ToString(),
                            FirstName = firstName,
                            MiddleName = middleName,
                            LastName = lastName,
                            Email = emailParam.Value?.ToString(),
                            Program = programParam.Value?.ToString(),
                            YearLevel = yearLevelParam.Value?.ToString(),
                            Status = statusParam.Value?.ToString(),
                            QRCodeData = qrCodeData
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving student by QR code: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Get all active students
        /// </summary>
        public async Task<List<Student>> GetAllAsync(bool activeOnly = true)
        {
            var students = new List<Student>();

            try
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    await connection.OpenAsync();
                    string query = activeOnly
                        ? "SELECT * FROM vw_active_students ORDER BY last_name, first_name"
                        : @"SELECT student_id, student_number, first_name, middle_name, last_name, 
                           email, phone, year_level, program, section, qr_code_data, photo_path, 
                           status, enrollment_date, created_at, updated_at
                           FROM students ORDER BY last_name, first_name";

                    using (var command = new MySqlCommand(query, connection))
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            students.Add(MapStudent(reader));
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
                    string query = "UPDATE students SET status = 'inactive', updated_at = CURRENT_TIMESTAMP WHERE student_id = @studentId";

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

        private Student MapStudent(MySqlDataReader reader)
        {
            return new Student
            {
                StudentId = reader.GetInt32("student_id"),
                StudentNumber = reader.GetString("student_number"),
                FirstName = reader.GetString("first_name"),
                MiddleName = reader.IsDBNull(reader.GetOrdinal("middle_name")) ? null : reader.GetString("middle_name"),
                LastName = reader.GetString("last_name"),
                Email = reader.IsDBNull(reader.GetOrdinal("email")) ? null : reader.GetString("email"),
                Phone = reader.IsDBNull(reader.GetOrdinal("phone")) ? null : reader.GetString("phone"),
                YearLevel = reader.GetString("year_level"),
                Program = reader.GetString("program"),
                Section = reader.IsDBNull(reader.GetOrdinal("section")) ? null : reader.GetString("section"),
                QRCodeData = reader.GetString("qr_code_data"),
                PhotoPath = reader.IsDBNull(reader.GetOrdinal("photo_path")) ? null : reader.GetString("photo_path"),
                Status = reader.GetString("status"),
                EnrollmentDate = reader.GetDateTime("enrollment_date"),
                CreatedAt = reader.GetDateTime("created_at"),
                UpdatedAt = reader.IsDBNull(reader.GetOrdinal("updated_at")) ? null : (DateTime?)reader.GetDateTime("updated_at")
            };
        }
    }
}
