using System;

namespace ITP104_FINAL_PROJECT.Models
{
    /// <summary>
    /// OTP Session model
    /// </summary>
    public class OTPSession
    {
        public string SessionId { get; set; }
        public string StudentId { get; set; }
        public string StudentNumber { get; set; }
        public string StudentName { get; set; }
        public string Email { get; set; }
        public string OTP { get; set; }
        public AttendanceType AttendanceType { get; set; }
        public string QRData { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; }
        public bool IsVerified { get; set; }
        public DateTime? VerifiedAt { get; set; }
        public int FailedAttempts { get; set; }
        public int ResendCount { get; set; }
    }
}
