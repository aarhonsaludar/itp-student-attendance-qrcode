using System;

namespace ITP104_FINAL_PROJECT.Models
{
    public class ScanHistory
    {
        public int ScanId { get; set; }
        public int StudentId { get; set; }
        public int? DeviceId { get; set; }
        public string ScanType { get; set; }
        public string ScanData { get; set; }
        public DateTime ScanDateTime { get; set; }
        public DateTime? TimeOut { get; set; }
        public string ScanPurpose { get; set; }
        public string Location { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool RequiresReview { get; set; }

        // Additional timestamp validation fields
        public string ValidationStatus { get; set; }
        public DateTime? ClientTime { get; set; }
        public DateTime? ServerTime { get; set; }
        public int? TimeDriftSeconds { get; set; }

        // Solution 1: Track how time-in/time-out was validated
        public string TimeInValidationMode { get; set; } // 'online' or 'offline'
        public string TimeOutValidationMode { get; set; } // 'online' or 'offline'

        // ===== ANTI-TAMPERING FIELDS (Works WITHOUT internet!) =====
        /// <summary>
        /// Stores Environment.TickCount64 at Time In
        /// TickCount64 = milliseconds since computer boot
        /// CANNOT be tampered by changing system date/time
        /// Works 100% OFFLINE - no internet needed!
        /// </summary>
        public long? TimeInTickCount { get; set; }

        /// <summary>
        /// Stores Environment.TickCount64 at Time Out
        /// Used to calculate REAL elapsed time regardless of system clock tampering
        /// Works 100% OFFLINE - no internet needed!
        /// </summary>
        public long? TimeOutTickCount { get; set; }

        /// <summary>
        /// Tracks number of times connection was lost during this session
        /// Frequent disconnections = suspicious behavior
        /// </summary>
        public int? ConnectionDropCount { get; set; }

        /// <summary>
        /// Stores offline duration in minutes
        /// Long offline periods require additional verification
        /// </summary>
        public double? OfflineDurationMinutes { get; set; }

        // Navigation properties
        public string StudentNumber { get; set; }
        public string StudentName { get; set; }
        public string DeviceName { get; set; }
        public string Program { get; set; }

        // ===== ANTI-TAMPERING CONSTANTS =====
        public const int MIN_ATTENDANCE_DURATION_MINUTES = 15;
        public const int MAX_ATTENDANCE_DURATION_HOURS = 12;
        public const int MAX_ALLOWED_TIME_DRIFT_SECONDS = 300; // 5 minutes
        public const int SUSPICIOUS_DISCONNECT_COUNT = 3; // Flag if disconnected 3+ times
        public const int MAX_OFFLINE_DURATION_MINUTES = 60; // Auto-review if offline > 1 hour
        public const double TICK_COUNT_TOLERANCE_MINUTES = 3.0; // Allow 3 min variance

        /// <summary>
        /// Calculates the REAL elapsed time using Stopwatch.GetTimestamp() (tamper-proof)
        /// Works WITHOUT internet - uses computer's internal high-resolution timer
        /// Returns elapsed time in minutes
        /// 
        /// Note: Using Stopwatch.GetTimestamp() instead of Environment.TickCount64 for .NET Framework 4.x compatibility
        /// Stopwatch.Frequency provides ticks per second for accurate conversion
        /// </summary>
        public double? GetRealElapsedTimeMinutes()
        {
            if (!TimeInTickCount.HasValue || !TimeOutTickCount.HasValue)
                return null;

            long tickDifference = TimeOutTickCount.Value - TimeInTickCount.Value;

            // Convert Stopwatch ticks to seconds, then to minutes
            // Stopwatch.Frequency is ticks per second
            double seconds = (double)tickDifference / System.Diagnostics.Stopwatch.Frequency;
            return seconds / 60.0;
        }

        /// <summary>
        /// Detects if system clock was tampered during offline mode
        /// Compares what the clock CLAIMS vs what REALLY happened (using TickCount64)
        /// Works 100% OFFLINE - no internet required!
        /// 
        /// Example of catching tampering:
        /// System Clock Claims: 1:00 PM to 7:00 PM = 6 hours (360 minutes)
        /// TickCount64 Shows: Only 2 minutes actually passed
        /// Result: TAMPERING DETECTED! 🚨
        /// </summary>
        public bool IsTimeOutTampered()
        {
            if (!TimeOut.HasValue || !TimeInTickCount.HasValue || !TimeOutTickCount.HasValue)
                return false;

            // Calculate claimed duration (what user's system clock shows)
            TimeSpan claimedDuration = TimeOut.Value - ScanDateTime;
            double claimedMinutes = claimedDuration.TotalMinutes;

            // Calculate actual duration (using tamper-proof tick count)
            double? actualMinutes = GetRealElapsedTimeMinutes();

            if (!actualMinutes.HasValue)
                return false;

            // Compare with tolerance
            double difference = Math.Abs(claimedMinutes - actualMinutes.Value);

            // If difference exceeds tolerance, it's tampered
            return difference > TICK_COUNT_TOLERANCE_MINUTES;
        }

        /// <summary>
        /// Validates if Time Out duration is within reasonable limits
        /// </summary>
        public bool IsTimeOutDurationValid(DateTime proposedTimeOut)
        {
            TimeSpan duration = proposedTimeOut - ScanDateTime;

            if (duration.TotalMinutes < MIN_ATTENDANCE_DURATION_MINUTES)
                return false;

            if (duration.TotalHours > MAX_ATTENDANCE_DURATION_HOURS)
                return false;

            return true;
        }

        /// <summary>
        /// Checks if offline behavior is suspicious
        /// </summary>
        public bool IsSuspiciousOfflineBehavior()
        {
            // Too many disconnections
            if (ConnectionDropCount.HasValue && ConnectionDropCount.Value >= SUSPICIOUS_DISCONNECT_COUNT)
                return true;

            // Offline for too long
            if (OfflineDurationMinutes.HasValue && OfflineDurationMinutes.Value > MAX_OFFLINE_DURATION_MINUTES)
                return true;

            return false;
        }

        /// <summary>
        /// Gets detailed validation message for Time Out
        /// </summary>
        public string GetTimeOutValidationMessage(DateTime proposedTimeOut)
        {
            TimeSpan duration = proposedTimeOut - ScanDateTime;

            // Priority 1: Check for clock tampering (works offline!)
            if (IsTimeOutTampered())
            {
                double? actualMinutes = GetRealElapsedTimeMinutes();
                return $"🚨 TIME TAMPERING DETECTED! Claimed: {duration.TotalMinutes:F0} min, Actual: {actualMinutes:F0} min";
            }

            // Priority 2: Check suspicious offline behavior
            if (IsSuspiciousOfflineBehavior())
            {
                if (ConnectionDropCount >= SUSPICIOUS_DISCONNECT_COUNT)
                    return $"⚠️ Suspicious: Disconnected {ConnectionDropCount} times during session";

                if (OfflineDurationMinutes > MAX_OFFLINE_DURATION_MINUTES)
                    return $"⚠️ Suspicious: Offline for {OfflineDurationMinutes:F0} minutes";
            }

            // Priority 3: Check duration limits
            if (duration.TotalMinutes < MIN_ATTENDANCE_DURATION_MINUTES)
            {
                return $"⏱️ Time Out too soon. Minimum: {MIN_ATTENDANCE_DURATION_MINUTES} min. Current: {duration.TotalMinutes:F0} min.";
            }

            if (duration.TotalHours > MAX_ATTENDANCE_DURATION_HOURS)
            {
                return $"⏱️ Time Out too late. Maximum: {MAX_ATTENDANCE_DURATION_HOURS} hours. Current: {duration.TotalHours:F1} hours.";
            }

            return "✅ Valid";
        }

        /// <summary>
        /// Checks if system time drift is suspicious (online mode)
        /// </summary>
        public bool IsSuspiciousTimeDrift()
        {
            if (!TimeDriftSeconds.HasValue)
                return false;

            return Math.Abs(TimeDriftSeconds.Value) > MAX_ALLOWED_TIME_DRIFT_SECONDS;
        }

        /// <summary>
        /// Gets the attendance duration display
        /// Shows REAL time (from TickCount64) if available
        /// </summary>
        public string GetAttendanceDuration()
        {
            if (!TimeOut.HasValue)
                return "Pending Time Out";

            // Use real elapsed time if available (more accurate, tamper-proof)
            double? realMinutes = GetRealElapsedTimeMinutes();
            if (realMinutes.HasValue)
            {
                int hours = (int)(realMinutes.Value / 60);
                int minutes = (int)(realMinutes.Value % 60);

                if (hours >= 1)
                    return $"{hours}h {minutes}m (verified)";
                else
                    return $"{minutes}m (verified)";
            }

            // Fallback to displayed time (less trustworthy)
            TimeSpan duration = TimeOut.Value - ScanDateTime;
            if (duration.TotalHours >= 1)
                return $"{duration.Hours}h {duration.Minutes}m";
            else
                return $"{duration.Minutes}m";
        }

        /// <summary>
        /// Gets a user-friendly attendance status based on Time In and Time Out
        /// This provides better UX than the raw 'status' field
        /// </summary>
        public string AttendanceStatus
        {
            get
            {
                // Check database status first for failed/declined records
                if (Status?.ToLower() == "failed")
                    return "❌ Failed";

                // Priority 1: Check for TIME TAMPERING (works offline!)
                if (IsTimeOutTampered())
                    return "🚨 Time Tampering Detected";

                // Priority 2: Check for suspicious offline behavior
                if (IsSuspiciousOfflineBehavior())
                    return "⚠️ Suspicious Activity";

                // Priority 3: Check for suspicious time drift (online)
                if (IsSuspiciousTimeDrift())
                    return "⚠️ Suspicious Time Drift";

                // Check if requires review AND status is still 'for_review'
                // (After approval, status changes to 'success' even if RequiresReview is still true)
                if ((RequiresReview || Status?.ToLower() == "for_review") && Status?.ToLower() != "success")
                    return "⚠️ For Review";

                // If duplicate, show that
                if (Status?.ToLower() == "duplicate")
                    return "🔁 Duplicate";

                // Check completion based on Time In/Out
                if (TimeOut.HasValue)
                {
                    // Validate duration
                    if (!IsTimeOutDurationValid(TimeOut.Value))
                        return "⚠️ Invalid Duration";

                    return "✅ Completed"; // Has both Time In and Time Out
                }
                else if (ScanDateTime.Date == DateTime.Today.Date)
                {
                    return "⏳ Pending Time Out"; // Today, waiting for Time Out
                }
                else
                {
                    return "⚠️ Incomplete"; // Old record without Time Out
                }
            }
        }

        /// <summary>
        /// Gets a simplified status for display (Completed/Pending/Incomplete)
        /// </summary>
        public string SimpleStatus
        {
            get
            {
                if (TimeOut.HasValue)
                    return "Completed";
                else if (ScanDateTime.Date == DateTime.Today.Date)
                    return "Pending";
                else
                    return "Incomplete";
            }
        }
    }
}
