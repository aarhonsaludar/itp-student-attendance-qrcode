using System;
using System.Data;
using System.Threading.Tasks;
using MySqlConnector;
using ITP104_FINAL_PROJECT.Data;

namespace ITP104_FINAL_PROJECT.Services
{
    /// <summary>
    /// Time Validation Service - Detects client-side time manipulation
    /// Compares client system time with internet time sources to prevent tampering
    /// Uses Google.com, TimeAPI.io, and Microsoft.com as trusted sources
    /// </summary>
    public static class TimeValidationService
    {
        /// <summary>
        /// Maximum allowed time drift between client and server (5 minutes)
        /// If drift exceeds this, time tampering is suspected
        /// Increased to 5 minutes to account for network delays and normal clock drift
        /// </summary>
        private static readonly TimeSpan MaxAllowedTimeDrift = TimeSpan.FromMinutes(5);

        /// <summary>
        /// Validate that client system time is synchronized with internet time sources
        /// Returns true if times are within acceptable range, false if tampering detected
        /// Uses trusted internet APIs (Google, TimeAPI, Microsoft) - NOT local database
        /// </summary>
        public static async Task<TimeValidationResult> ValidateClientTimeAsync()
        {
            try
            {
                // Get client's current system time (potentially tampered)
                DateTime clientTime = DateTime.Now;

                // Get trusted internet time (tamper-proof source)
                DateTime? serverTime = await GetTrustedInternetTimeAsync();

                if (!serverTime.HasValue)
                {
                    // HYBRID MODE: Allow offline attendance but flag for manual review
                    await ErrorLoggingService.LogWarningAsync(
                        "⚠️ OFFLINE ATTENDANCE - REQUIRES REVIEW",
                        $"Client Time: {clientTime:yyyy-MM-dd HH:mm:ss}\n" +
                        $"No internet connection - unable to validate time\n" +
                        $"Source: All internet time sources unavailable (Google, TimeAPI, Microsoft)\n" +
                        $"⚠️ THIS ATTENDANCE WILL BE FLAGGED FOR MANUAL REVIEW",
                        "offline_attendance");

                    return new TimeValidationResult
                    {
                        IsValid = true, // Allow attendance
                        ClientTime = clientTime,
                        ServerTime = null,
                        TimeDrift = TimeSpan.Zero,
                        ErrorMessage = "⚠️ Offline Mode - No internet connection available.\n\nAttendance recorded using device time.\nThis record will be flagged for manual review.",
                        ValidationStatus = TimeValidationStatus.OfflineMode,
                        RequiresManualReview = true // Flag for review
                    };
                }

                // ===================================================
                // CRITICAL: Check BOTH date and time tampering
                // ===================================================

                // 1. Check if DATE is different (even by 1 day = tampering)
                bool isDifferentDate = clientTime.Date != serverTime.Value.Date;

                // 2. Calculate time drift (for same-day time manipulation)
                TimeSpan timeDrift = clientTime - serverTime.Value;
                TimeSpan absoluteDrift = timeDrift.Duration(); // Get absolute value

                // 3. Check if drift exceeds maximum allowed
                bool isTimeDriftValid = absoluteDrift <= MaxAllowedTimeDrift;

                // 4. BOTH date must match AND time drift must be within range
                bool isValid = !isDifferentDate && isTimeDriftValid;

                // DEBUG: Log validation details
                await ErrorLoggingService.LogInfoAsync(
                    "Time Validation Check",
                    $"Client: {clientTime:yyyy-MM-dd HH:mm:ss}\n" +
                    $"Server: {serverTime.Value:yyyy-MM-dd HH:mm:ss}\n" +
                    $"Date Match: {!isDifferentDate}\n" +
                    $"Time Drift: {absoluteDrift.TotalMinutes:F2} min (Max: {MaxAllowedTimeDrift.TotalMinutes} min)\n" +
                    $"Time Valid: {isTimeDriftValid}\n" +
                    $"RESULT: {(isValid ? "✓ VALID" : "✗ BLOCKED")}",
                    "time_validation");

                // Build detailed error message
                string errorMessage = null;
                if (!isValid)
                {
                    if (isDifferentDate)
                    {
                        errorMessage = $"⚠️ DATE TAMPERING DETECTED!\n\n" +
                            $"Your device date does not match the server date.\n\n" +
                            $"Device Date: {clientTime:yyyy-MM-dd} (Time: {clientTime:HH:mm:ss})\n" +
                            $"Server Date: {serverTime.Value:yyyy-MM-dd} (Time: {serverTime.Value:HH:mm:ss})\n\n" +
                            $"Date Difference: {Math.Abs((clientTime.Date - serverTime.Value.Date).Days)} day(s)\n\n" +
                            $"❌ ATTENDANCE BLOCKED\n\n" +
                            $"Please set your system date/time to match the current date and try again.";
                    }
                    else
                    {
                        errorMessage = $"⚠️ TIME TAMPERING DETECTED!\n\n" +
                            $"Client time differs from server by {FormatTimeDrift(timeDrift)}.\n\n" +
                            $"Device Time: {clientTime:yyyy-MM-dd HH:mm:ss}\n" +
                            $"Server Time: {serverTime.Value:yyyy-MM-dd HH:mm:ss}\n" +
                            $"Time Difference: {absoluteDrift.TotalMinutes:F2} minutes\n\n" +
                            $"❌ ATTENDANCE BLOCKED\n\n" +
                            $"Please synchronize your system clock.";
                    }
                }

                return new TimeValidationResult
                {
                    IsValid = isValid,
                    ClientTime = clientTime,
                    ServerTime = serverTime.Value,
                    TimeDrift = timeDrift,
                    ErrorMessage = errorMessage,
                    ValidationStatus = isValid ? TimeValidationStatus.Valid : TimeValidationStatus.ClockManipulationDetected
                };
            }
            catch (Exception ex)
            {
                await ErrorLoggingService.LogErrorAsync(
                    "Time Validation Failed",
                    ex,
                    "time_validation");

                return new TimeValidationResult
                {
                    IsValid = false,
                    ClientTime = DateTime.Now,
                    ServerTime = null,
                    TimeDrift = TimeSpan.Zero,
                    ErrorMessage = $"Time validation error: {ex.Message}",
                    ValidationStatus = TimeValidationStatus.Unknown
                };
            }
        }

        /// <summary>
        /// Get the current time from TRUSTED INTERNET sources ONLY
        /// Uses internet APIs to prevent local time manipulation
        /// Primary: Google.com | Fallback: TimeAPI.io | Final: Microsoft.com
        /// NOTE: Does NOT use local database time (would fail if DB is on same PC)
        /// </summary>
        private static async Task<DateTime?> GetTrustedInternetTimeAsync()
        {
            // Get time from internet sources (tamper-proof)
            return await GetInternetTimeAsync();
        }

        /// <summary>
        /// Get trusted time from internet sources
        /// Primary: Google.com HTTP Date header (most reliable)
        /// Fallback: TimeAPI.io
        /// </summary>
        private static async Task<DateTime?> GetInternetTimeAsync()
        {
            // Try TimeAPI.io first (returns Philippine local time directly)
            var timeApiResult = await GetTimeFromTimeAPIAsync();
            if (timeApiResult.HasValue)
            {
                return timeApiResult;
            }

            // Fallback: Try Google (returns UTC, will be converted to local)
            var googleResult = await GetTimeFromHttpHeaderAsync("https://www.google.com");
            if (googleResult.HasValue)
            {
                return googleResult;
            }

            // Try Microsoft as final fallback (returns UTC, will be converted to local)
            var microsoftResult = await GetTimeFromHttpHeaderAsync("https://www.microsoft.com");
            if (microsoftResult.HasValue)
            {
                return microsoftResult;
            }

            return null;
        }

        /// <summary>
        /// Get time from TimeAPI.io (free, reliable, no authentication)
        /// API: https://timeapi.io/api/Time/current/zone?timeZone=Asia/Manila
        /// </summary>
        private static async Task<DateTime?> GetTimeFromTimeAPIAsync()
        {
            try
            {
                using (var httpClient = new System.Net.Http.HttpClient())
                {
                    httpClient.Timeout = TimeSpan.FromSeconds(5);

                    // Use Asia/Manila timezone (Philippines)
                    string url = "https://timeapi.io/api/Time/current/zone?timeZone=Asia/Manila";
                    var response = await httpClient.GetStringAsync(url);

                    // Parse JSON response manually (simple string parsing)
                    // Example response: {"year":2025,"month":11,"day":28,"hour":20,"minute":50,"seconds":30,...,"dateTime":"2025-11-28T20:50:30"}
                    // Extract the dateTime value
                    int dateTimeIndex = response.IndexOf("\"dateTime\":\"");
                    if (dateTimeIndex >= 0)
                    {
                        int startIndex = dateTimeIndex + 12; // Length of "dateTime":"
                        int endIndex = response.IndexOf("\"", startIndex);
                        if (endIndex > startIndex)
                        {
                            string dateTimeStr = response.Substring(startIndex, endIndex - startIndex);
                            if (DateTime.TryParse(dateTimeStr, out DateTime parsedTime))
                            {
                                return parsedTime;
                            }
                        }
                    }
                }
            }
            catch
            {
                // TimeAPI failed, will try fallback
            }

            return null;
        }

        /// <summary>
        /// Get time from HTTP Date header (fallback method)
        /// Uses Google.com or other reliable servers
        /// NOTE: HTTP headers return UTC time, which is converted to local Philippine time
        /// </summary>
        private static async Task<DateTime?> GetTimeFromHttpHeaderAsync(string url)
        {
            try
            {
                using (var httpClient = new System.Net.Http.HttpClient())
                {
                    httpClient.Timeout = TimeSpan.FromSeconds(5);

                    var response = await httpClient.GetAsync(url, System.Net.Http.HttpCompletionOption.ResponseHeadersRead);

                    if (response.Headers.Date.HasValue)
                    {
                        // HTTP Date header returns UTC time - convert to local Philippine time
                        DateTime utcTime = response.Headers.Date.Value.DateTime;
                        DateTime localTime = utcTime.ToLocalTime();
                        return localTime;
                    }
                }
            }
            catch
            {
                // HTTP header method failed
            }

            return null;
        }

        /// <summary>
        /// Format time drift for user-friendly display
        /// </summary>
        private static string FormatTimeDrift(TimeSpan drift)
        {
            bool isNegative = drift < TimeSpan.Zero;
            TimeSpan absoluteDrift = drift.Duration();

            string direction = isNegative ? "behind" : "ahead";

            if (absoluteDrift.TotalDays >= 1)
            {
                return $"{absoluteDrift.TotalDays:F1} days {direction}";
            }
            else if (absoluteDrift.TotalHours >= 1)
            {
                return $"{absoluteDrift.TotalHours:F1} hours {direction}";
            }
            else if (absoluteDrift.TotalMinutes >= 1)
            {
                return $"{absoluteDrift.TotalMinutes:F1} minutes {direction}";
            }
            else
            {
                return $"{absoluteDrift.TotalSeconds:F0} seconds {direction}";
            }
        }

        /// <summary>
        /// Quick check if time validation is likely to pass
        /// Used to show warning before attempting scan
        /// </summary>
        public static async Task<bool> IsTimeSynchronizedAsync()
        {
            var result = await ValidateClientTimeAsync();
            return result.IsValid;
        }
    }

    /// <summary>
    /// Result of time validation check
    /// </summary>
    public class TimeValidationResult
    {
        /// <summary>
        /// Whether the client time is within acceptable range of server time
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// Client's system time (potentially tampered)
        /// </summary>
        public DateTime ClientTime { get; set; }

        /// <summary>
        /// Database server's time (trusted source)
        /// </summary>
        public DateTime? ServerTime { get; set; }

        /// <summary>
        /// Time difference between client and server (client - server)
        /// Positive = client is ahead, Negative = client is behind
        /// </summary>
        public TimeSpan TimeDrift { get; set; }

        /// <summary>
        /// User-friendly error message if validation failed
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Detailed validation status
        /// </summary>
        public TimeValidationStatus ValidationStatus { get; set; }

        /// <summary>
        /// Whether this attendance record requires manual review
        /// Set to true for offline scans or suspicious activity
        /// </summary>
        public bool RequiresManualReview { get; set; }
    }

    /// <summary>
    /// Detailed status of time validation
    /// </summary>
    public enum TimeValidationStatus
    {
        /// <summary>
        /// Time is synchronized and valid
        /// </summary>
        Valid,

        /// <summary>
        /// Client time differs significantly from server time (tampering suspected)
        /// </summary>
        ClockManipulationDetected,

        /// <summary>
        /// Unable to connect to database server to verify time
        /// </summary>
        NetworkError,

        /// <summary>
        /// Unknown error during validation
        /// </summary>
        Unknown,

        /// <summary>
        /// Offline mode - No internet connection, using device time (requires manual review)
        /// </summary>
        OfflineMode
    }
}
