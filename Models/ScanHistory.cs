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

        // Navigation properties
        public string StudentNumber { get; set; }
        public string StudentName { get; set; }
        public string DeviceName { get; set; }
    }
}
