using System;

namespace ITP104_FINAL_PROJECT.Models
{
    public class SystemSetting
    {
        public int SettingId { get; set; }
        public string SettingKey { get; set; }
        public string SettingValue { get; set; }
        public string SettingCategory { get; set; }
        public string Description { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
