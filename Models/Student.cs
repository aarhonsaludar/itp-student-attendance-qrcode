using System;

namespace ITP104_FINAL_PROJECT.Models
{
    public class Student
    {
        public int StudentId { get; set; }
        public string StudentNumber { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string YearLevel { get; set; }
        public string Program { get; set; }
        public string Section { get; set; }
        public string QRCodeData { get; set; }
        public string PhotoPath { get; set; }
        public string Status { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public string FullName => $"{FirstName} {MiddleName} {LastName}".Replace("  ", " ").Trim();
    }
}
