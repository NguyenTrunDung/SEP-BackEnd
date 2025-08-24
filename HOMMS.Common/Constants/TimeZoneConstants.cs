using System;

namespace HOMMS.Common.Constants
{
    /// <summary>
    /// Timezone constants for the Hospital Order Management System
    /// </summary>
    public static class TimeZoneConstants
    {
        /// <summary>
        /// Vietnam Standard Time zone ID
        /// </summary>
        public const string VIETNAM_TIMEZONE_ID = "SE Asia Standard Time"; // Windows
        public const string VIETNAM_TIMEZONE_IANA = "Asia/Ho_Chi_Minh"; // Linux/IANA
        
        /// <summary>
        /// UTC timezone (for storage and API communication)
        /// </summary>
        public const string UTC_TIMEZONE_ID = "UTC";
        
        /// <summary>
        /// Default timezone for the application (Vietnam)
        /// </summary>
        public static readonly TimeZoneInfo VietnamTimeZone = GetVietnamTimeZone();
        
        /// <summary>
        /// UTC timezone info
        /// </summary>
        public static readonly TimeZoneInfo UtcTimeZone = TimeZoneInfo.Utc;
        
        private static TimeZoneInfo GetVietnamTimeZone()
        {
            try
            {
                // Try Windows timezone first
                return TimeZoneInfo.FindSystemTimeZoneById(VIETNAM_TIMEZONE_ID);
            }
            catch (TimeZoneNotFoundException)
            {
                try
                {
                    // Try IANA timezone for Linux/Mac
                    return TimeZoneInfo.FindSystemTimeZoneById(VIETNAM_TIMEZONE_IANA);
                }
                catch (TimeZoneNotFoundException)
                {
                    // Fallback to UTC+7 offset
                    return TimeZoneInfo.CreateCustomTimeZone(
                        "Vietnam Standard Time",
                        TimeSpan.FromHours(7),
                        "Vietnam Standard Time",
                        "Vietnam Standard Time"
                    );
                }
            }
        }
    }
}
