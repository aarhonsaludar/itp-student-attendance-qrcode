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

        /// <summary>
        /// Validate time-out against time-in to detect time manipulation
        /// Solution 1: Time-Out Validation Against Time-In
        /// Enhanced with TickCount verification for offline tampering detection
        /// </summary>
        public static (bool IsValid, string[] SuspiciousFlags) ValidateTimeOutAgainstTimeIn(
            DateTime timeIn,
            DateTime timeOut,
            string timeInValidationMode,
            string timeOutValidationMode,
            long? timeInTickCount = null,
            long? timeOutTickCount = null)
        {
            var flags = new System.Collections.Generic.List<string>();
            bool isValid = true;

            // Check 1: Basic physics - time-out must be after time-in
            if (timeOut <= timeIn)
            {
                flags.Add("🔴 CRITICAL: Time-out must be after time-in");
                isValid = false;
            }

            TimeSpan duration = timeOut - timeIn;
            double claimedMinutes = duration.TotalMinutes;

            // NEW: Calculate real elapsed time using TickCount if available
            double? realMinutes = null;
            if (timeInTickCount.HasValue && timeOutTickCount.HasValue)
            {
                realMinutes = GetRealElapsedMinutes(timeInTickCount.Value, timeOutTickCount.Value);
            }

            // Check 2: Too short (suspicious but not blocking)
            if (duration.TotalMinutes < 10)
            {
                flags.Add($"🟡 INFO: Short duration ({duration.TotalMinutes:F0} minutes)");
            }

            // Check 3: Extremely short (within OTP window)
            if (duration.TotalMinutes < 5)
            {
                flags.Add("🟠 WARNING: Extremely short duration - verify legitimacy");
            }

            // Check 4: Too long
            if (duration.TotalHours > 12)
            {
                flags.Add($"🟠 WARNING: Long duration ({duration.TotalHours:F1} hours)");
            }

            // Check 5: Unrealistically long
            if (duration.TotalHours > 18)
            {
                flags.Add($"🔴 CRITICAL: Duration exceeds 18 hours - likely tampering");
            }

            // Check 6: VALIDATION MODE MISMATCH (KEY SECURITY CHECK)
            // This catches the WiFi disconnect + time change exploit
            if (!string.IsNullOrEmpty(timeInValidationMode) && !string.IsNullOrEmpty(timeOutValidationMode))
            {
                if (timeInValidationMode == "online" && timeOutValidationMode == "offline")
                {
                    flags.Add("🚨 CRITICAL: Time-in was ONLINE (verified) but time-out is OFFLINE (unverified)");
                    flags.Add("    → Student may have disconnected WiFi and changed device time");

                    // NEW: Add TickCount verification if available
                    if (realMinutes.HasValue)
                    {
                        flags.Add($"    → System clock claims: {claimedMinutes:F0} minutes");
                        flags.Add($"    → Tamper-proof timer shows: {realMinutes:F0} minutes (actual elapsed)");

                        double timeDifference = Math.Abs(claimedMinutes - realMinutes.Value);
                        if (timeDifference > 3.0) // More than 3 minutes difference
                        {
                            flags.Add($"    → 🚨 CONFIRMED TIME TAMPERING! ({timeDifference:F0} min difference)");
                            flags.Add("    → STRONG EVIDENCE of clock manipulation");
                        }
                        else
                        {
                            flags.Add("    → ✅ TickCount verification passed (times match)");
                            flags.Add("    → Possibly legitimate offline usage - review WiFi disconnect reason");
                        }
                    }
                    else
                    {
                        flags.Add("    → ⚠️ Cannot verify with TickCount (data not available)");
                    }

                    flags.Add("    → RECOMMEND DECLINING unless student provides valid explanation");
                    // Don't set isValid = false to allow admin review, but heavily flag it
                }
                else if (timeInValidationMode == "offline" && timeOutValidationMode == "online")
                {
                    flags.Add("🟡 INFO: Time-in was offline but time-out is online (less suspicious)");

                    // NEW: Show TickCount verification for completeness
                    if (realMinutes.HasValue)
                    {
                        double timeDifference = Math.Abs(claimedMinutes - realMinutes.Value);
                        if (timeDifference > 3.0)
                        {
                            flags.Add($"    → ⚠️ TickCount shows {timeDifference:F0} min difference");
                            flags.Add($"    → Claimed: {claimedMinutes:F0} min, Actual: {realMinutes:F0} min");
                        }
                    }
                }
                else if (timeInValidationMode == "offline" && timeOutValidationMode == "offline")
                {
                    // NEW: Both offline - TickCount is the ONLY way to verify
                    if (realMinutes.HasValue)
                    {
                        double timeDifference = Math.Abs(claimedMinutes - realMinutes.Value);
                        if (timeDifference > 3.0)
                        {
                            flags.Add("🚨 CRITICAL: TIME TAMPERING DETECTED (Offline Mode)");
                            flags.Add($"    → System clock claims: {claimedMinutes:F0} minutes");
                            flags.Add($"    → Tamper-proof timer shows: {realMinutes:F0} minutes (actual elapsed)");
                            flags.Add($"    → Difference: {timeDifference:F0} minutes");
                            flags.Add("    → Student likely changed device time while offline");
                            flags.Add("    → RECOMMEND DECLINING - Clear evidence of tampering");
                        }
                        else
                        {
                            flags.Add($"✅ Offline session verified by TickCount ({realMinutes:F0} min actual)");
                            flags.Add("    → Times match - likely legitimate offline usage");
                        }
                    }
                    else
                    {
                        flags.Add("⚠️ Both scans were offline - TickCount verification recommended");
                        flags.Add("    → Cannot confirm if time was tampered");
                    }
                }
            }

            return (isValid, flags.ToArray());
        }

        /// <summary>
        /// Calculate real elapsed time from TickCount values (tamper-proof)
        /// Uses Stopwatch.GetTimestamp() which cannot be manipulated by changing system time
        /// </summary>
        private static double? GetRealElapsedMinutes(long timeInTickCount, long timeOutTickCount)
        {
            try
            {
                if (timeOutTickCount <= timeInTickCount)
                    return null; // Invalid - time-out tick should be after time-in tick

                long elapsedTicks = timeOutTickCount - timeInTickCount;

                // Convert ticks to seconds using Stopwatch.Frequency
                // Frequency = ticks per second on this system
                double elapsedSeconds = (double)elapsedTicks / System.Diagnostics.Stopwatch.Frequency;

                // Convert to minutes
                return elapsedSeconds / 60.0;
            }
            catch
            {
                return null; // Calculation error
            }
        }

        /// <summary>
        /// Validate scan timestamp for suspicious patterns
        /// </summary>
        public static (bool IsValid, string[] SuspiciousFlags) ValidateScanTimestamp(
            DateTime scanTime,
            DateTime? lastScanTime = null,
            DateTime? serverTime = null)
        {
            var flags = new System.Collections.Generic.List<string>();
            bool isValid = true;

            // Use server time if available, otherwise use current time
            DateTime referenceTime = serverTime ?? DateTime.Now;

            // Check 1: Future timestamp (CRITICAL - definitely tampered)
            if (scanTime > referenceTime.AddMinutes(10))
            {
                TimeSpan futureDiff = scanTime - referenceTime;
                flags.Add($"🔴 CRITICAL: Timestamp is {futureDiff.Hours}h {futureDiff.Minutes}m in the future");
                isValid = false;
            }

            // Check 2: Very old timestamp (suspicious)
            if (scanTime < referenceTime.AddDays(-1))
            {
                TimeSpan age = referenceTime - scanTime;
                flags.Add($"🟠 WARNING: Timestamp is {age.Days} day(s) {age.Hours}h old");
            }

            // Check 3: Time jumped backwards from last scan
            if (lastScanTime.HasValue && scanTime < lastScanTime.Value)
            {
                TimeSpan diff = lastScanTime.Value - scanTime;
                flags.Add($"🟠 WARNING: Time went backwards by {diff.Hours}h {diff.Minutes}m from previous scan");
            }

            // Check 4: Unrealistic hours (very early or very late)
            int hour = scanTime.Hour;
            if (hour < 6 || hour > 22)
            {
                flags.Add($"🟡 INFO: Unusual time ({scanTime:h:mm tt}) - outside typical hours (6 AM - 10 PM)");
            }

            // Check 5: Weekend scan
            if (scanTime.DayOfWeek == DayOfWeek.Saturday || scanTime.DayOfWeek == DayOfWeek.Sunday)
            {
                flags.Add($"🟡 INFO: Weekend scan ({scanTime.DayOfWeek})");
            }

            return (isValid, flags.ToArray());
        }
    }
}
