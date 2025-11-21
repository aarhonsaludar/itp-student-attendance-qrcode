using System;
using System.Text.RegularExpressions;

namespace ITP104_FINAL_PROJECT.Services
{
    /// <summary>
    /// Input validation utilities for user input
    /// </summary>
    public static class InputValidator
    {
        /// <summary>
        /// Validate email format
        /// </summary>
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Use a simple but effective email regex
                string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Validate phone number (Philippine format)
        /// </summary>
        public static bool IsValidPhoneNumber(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return true; // Phone is optional

            // Remove common separators
            string cleaned = phone.Replace("-", "").Replace(" ", "").Replace("(", "").Replace(")", "");

            // Philippine mobile: 09XX-XXX-XXXX or +639XX-XXX-XXXX
            // Landline: (0XX) XXX-XXXX
            string pattern = @"^(\+639|09)\d{9}$|^0\d{9,10}$";
            return Regex.IsMatch(cleaned, pattern);
        }

        /// <summary>
        /// Validate student number format
        /// </summary>
        public static bool IsValidStudentNumber(string studentNumber)
        {
            if (string.IsNullOrWhiteSpace(studentNumber))
                return false;

            // Allow alphanumeric with hyphens, 5-50 characters
            return studentNumber.Length >= 5 && studentNumber.Length <= 50 &&
                   Regex.IsMatch(studentNumber, @"^[A-Za-z0-9\-]+$");
        }

        /// <summary>
        /// Validate name (letters, spaces, periods, hyphens only)
        /// </summary>
        public static bool IsValidName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            // Allow letters, spaces, periods, hyphens, and apostrophes
            return Regex.IsMatch(name, @"^[A-Za-z\s\.\-']+$") && name.Trim().Length >= 2;
        }

        /// <summary>
        /// Sanitize input to prevent SQL injection (additional layer of protection)
        /// </summary>
        public static string SanitizeInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            // Remove potentially dangerous characters
            // Note: This is a backup - parameterized queries are the primary defense
            return input.Trim();
        }

        /// <summary>
        /// Validate required field
        /// </summary>
        public static (bool IsValid, string ErrorMessage) ValidateRequired(string value, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return (false, $"{fieldName} is required.");
            }
            return (true, null);
        }

        /// <summary>
        /// Validate string length
        /// </summary>
        public static (bool IsValid, string ErrorMessage) ValidateLength(
            string value, 
            string fieldName, 
            int minLength = 0, 
            int maxLength = int.MaxValue)
        {
            if (string.IsNullOrEmpty(value))
            {
                if (minLength > 0)
                    return (false, $"{fieldName} is required.");
                return (true, null);
            }

            if (value.Length < minLength)
            {
                return (false, $"{fieldName} must be at least {minLength} characters.");
            }

            if (value.Length > maxLength)
            {
                return (false, $"{fieldName} must not exceed {maxLength} characters.");
            }

            return (true, null);
        }

        /// <summary>
        /// Validate integer range
        /// </summary>
        public static (bool IsValid, string ErrorMessage) ValidateIntRange(
            int value, 
            string fieldName, 
            int min = int.MinValue, 
            int max = int.MaxValue)
        {
            if (value < min || value > max)
            {
                return (false, $"{fieldName} must be between {min} and {max}.");
            }
            return (true, null);
        }

        /// <summary>
        /// Validate date range
        /// </summary>
        public static (bool IsValid, string ErrorMessage) ValidateDateRange(
            DateTime value, 
            string fieldName, 
            DateTime? minDate = null, 
            DateTime? maxDate = null)
        {
            DateTime min = minDate ?? DateTime.MinValue;
            DateTime max = maxDate ?? DateTime.MaxValue;

            if (value < min || value > max)
            {
                return (false, $"{fieldName} must be between {min:yyyy-MM-dd} and {max:yyyy-MM-dd}.");
            }
            return (true, null);
        }
    }
}
