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

        // Navigation properties
        public string StudentNumber { get; set; }
        public string StudentName { get; set; }
        public string DeviceName { get; set; }
        public string Program { get; set; }

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
