using System;
using HOMMS.Common.Constants;

namespace HOMMS.Common.Helpers
{
    /// <summary>
    /// Helper class for timezone operations in HOMMS
    /// </summary>
    public static class TimeZoneHelper
    {
        /// <summary>
        /// Get current Vietnam time
        /// </summary>
        /// <returns>Current DateTime in Vietnam timezone</returns>
        public static DateTime GetVietnamNow()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneConstants.VietnamTimeZone);
        }
        
        /// <summary>
        /// Convert UTC time to Vietnam time
        /// </summary>
        /// <param name="utcDateTime">UTC DateTime</param>
        /// <returns>DateTime in Vietnam timezone</returns>
        public static DateTime ConvertUtcToVietnam(DateTime utcDateTime)
        {
            if (utcDateTime.Kind != DateTimeKind.Utc)
            {
                utcDateTime = DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc);
            }
            
            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, TimeZoneConstants.VietnamTimeZone);
        }
        
        /// <summary>
        /// Convert Vietnam time to UTC
        /// </summary>
        /// <param name="vietnamDateTime">DateTime in Vietnam timezone</param>
        /// <returns>UTC DateTime</returns>
        public static DateTime ConvertVietnamToUtc(DateTime vietnamDateTime)
        {
            return TimeZoneInfo.ConvertTimeToUtc(vietnamDateTime, TimeZoneConstants.VietnamTimeZone);
        }
        
        /// <summary>
        /// Convert nullable UTC time to Vietnam time
        /// </summary>
        /// <param name="utcDateTime">Nullable UTC DateTime</param>
        /// <returns>Nullable DateTime in Vietnam timezone</returns>
        public static DateTime? ConvertUtcToVietnam(DateTime? utcDateTime)
        {
            return utcDateTime?.Let(dt => ConvertUtcToVietnam(dt));
        }
        
        /// <summary>
        /// Convert nullable Vietnam time to UTC
        /// </summary>
        /// <param name="vietnamDateTime">Nullable DateTime in Vietnam timezone</param>
        /// <returns>Nullable UTC DateTime</returns>
        public static DateTime? ConvertVietnamToUtc(DateTime? vietnamDateTime)
        {
            return vietnamDateTime?.Let(dt => ConvertVietnamToUtc(dt));
        }
        
        /// <summary>
        /// Format DateTime for API response (ISO 8601 with timezone)
        /// </summary>
        /// <param name="dateTime">DateTime to format</param>
        /// <param name="includeTimezone">Include timezone info in output</param>
        /// <returns>Formatted string</returns>
        public static string FormatForApi(DateTime dateTime, bool includeTimezone = true)
        {
            if (includeTimezone)
            {
                var vietnamTime = ConvertUtcToVietnam(dateTime);
                return vietnamTime.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz");
            }
            
            return dateTime.ToString("yyyy-MM-ddTHH:mm:ss.fff");
        }
        
        /// <summary>
        /// Format nullable DateTime for API response
        /// </summary>
        /// <param name="dateTime">Nullable DateTime to format</param>
        /// <param name="includeTimezone">Include timezone info in output</param>
        /// <returns>Formatted string or null</returns>
        public static string? FormatForApi(DateTime? dateTime, bool includeTimezone = true)
        {
            return dateTime?.Let(dt => FormatForApi(dt, includeTimezone));
        }
        
        /// <summary>
        /// Parse API date string to UTC DateTime
        /// </summary>
        /// <param name="dateString">Date string from API</param>
        /// <returns>UTC DateTime</returns>
        /// <remarks>
        /// This method handles different date formats:
        /// - Date-only strings (YYYY-MM-DD): Treated as UTC dates without timezone conversion
        /// - DateTime strings with timezone: Properly converted to UTC
        /// - Unspecified timezone: Assumed to be Vietnam time and converted to UTC
        /// </remarks>
        public static DateTime ParseApiDate(string dateString)
        {
            if (DateTime.TryParse(dateString, out var parsedDate))
            {
                // If it's a date-only string (no time component), treat it as UTC date
                if (IsDateOnlyString(dateString))
                {
                    return ParseDateOnlyAsUtc(dateString);
                }
                
                // If no timezone info, assume Vietnam time and convert to UTC
                if (parsedDate.Kind == DateTimeKind.Unspecified)
                {
                    return ConvertVietnamToUtc(parsedDate);
                }
                
                // If already UTC, return as is
                if (parsedDate.Kind == DateTimeKind.Utc)
                {
                    return parsedDate;
                }
                
                // If local time, convert to UTC
                return parsedDate.ToUniversalTime();
            }
            
            throw new ArgumentException($"Invalid date format: {dateString}");
        }

        /// <summary>
        /// Check if a string represents a date-only value (YYYY-MM-DD format)
        /// </summary>
        /// <param name="dateString">Date string to check</param>
        /// <returns>True if it's a date-only string</returns>
        private static bool IsDateOnlyString(string dateString)
        {
            return dateString.Length == 10 && 
                   dateString.Contains("-") && 
                   dateString.Count(c => c == '-') == 2 &&
                   !dateString.Contains("T") && 
                   !dateString.Contains(":") &&
                   !dateString.Contains("Z");
        }

        /// <summary>
        /// Parse a date-only string (YYYY-MM-DD) and return it as UTC DateTime
        /// This prevents timezone conversion issues for date-only values
        /// </summary>
        /// <param name="dateString">Date string in YYYY-MM-DD format</param>
        /// <returns>UTC DateTime at 00:00:00</returns>
        private static DateTime ParseDateOnlyAsUtc(string dateString)
        {
            if (!DateTime.TryParse(dateString, out var parsedDate))
            {
                throw new ArgumentException($"Invalid date format: {dateString}");
            }
            
            // Create UTC date without timezone conversion
            // This ensures the date stays exactly as intended
            return new DateTime(parsedDate.Year, parsedDate.Month, parsedDate.Day, 0, 0, 0, DateTimeKind.Utc);
        }

        /// <summary>
        /// Parse a menu date string (YYYY-MM-DD) and return it as UTC DateTime
        /// This is specifically for menu dates where we want the exact date without timezone shifts
        /// </summary>
        /// <param name="dateString">Date string in YYYY-MM-DD format</param>
        /// <returns>UTC DateTime at 00:00:00</returns>
        public static DateTime ParseMenuDate(string dateString)
        {
            if (IsDateOnlyString(dateString))
            {
                return ParseDateOnlyAsUtc(dateString);
            }
            
            // If it's not a date-only string, fall back to the general parser
            return ParseApiDate(dateString);
        }

        /// <summary>
        /// Debug method to log timezone conversion details
        /// Useful for troubleshooting date/time issues
        /// </summary>
        /// <param name="inputString">Input date string</param>
        /// <param name="outputDateTime">Output UTC DateTime</param>
        /// <param name="method">Method name that called this</param>
        public static void LogTimezoneConversion(string inputString, DateTime outputDateTime, string method = "Unknown")
        {
            var vietnamTime = ConvertUtcToVietnam(outputDateTime);
            
            // This would typically use a proper logging framework
            // For now, we'll use Console.WriteLine for debugging
            Console.WriteLine($"[Timezone Debug] {method}:");
            Console.WriteLine($"  Input: '{inputString}'");
            Console.WriteLine($"  Output UTC: {outputDateTime:yyyy-MM-dd HH:mm:ss} UTC");
            Console.WriteLine($"  Output Vietnam: {vietnamTime:yyyy-MM-dd HH:mm:ss} (+7)");
            Console.WriteLine($"  IsDateOnly: {IsDateOnlyString(inputString)}");
        }
        
        /// <summary>
        /// Get start of day in Vietnam timezone, converted to UTC
        /// </summary>
        /// <param name="date">Date in Vietnam timezone</param>
        /// <returns>Start of day in UTC</returns>
        public static DateTime GetStartOfDayUtc(DateTime date)
        {
            var vietnamStartOfDay = date.Date; // 00:00:00 in Vietnam
            return ConvertVietnamToUtc(vietnamStartOfDay);
        }
        
        /// <summary>
        /// Get end of day in Vietnam timezone, converted to UTC
        /// </summary>
        /// <param name="date">Date in Vietnam timezone</param>
        /// <returns>End of day in UTC</returns>
        public static DateTime GetEndOfDayUtc(DateTime date)
        {
            var vietnamEndOfDay = date.Date.AddDays(1).AddTicks(-1); // 23:59:59.999... in Vietnam
            return ConvertVietnamToUtc(vietnamEndOfDay);
        }
        
        /// <summary>
        /// Check if a UTC DateTime falls within a specific Vietnam date
        /// </summary>
        /// <param name="utcDateTime">UTC DateTime to check</param>
        /// <param name="vietnamDate">Vietnam date to check against</param>
        /// <returns>True if the UTC time falls within the Vietnam date</returns>
        public static bool IsDateInVietnamTimezone(DateTime utcDateTime, DateTime vietnamDate)
        {
            var vietnamDateTime = ConvertUtcToVietnam(utcDateTime);
            return vietnamDateTime.Date == vietnamDate.Date;
        }
    }
    
    /// <summary>
    /// Extension methods for functional programming style
    /// </summary>
    public static class FunctionalExtensions
    {
        public static TResult Let<T, TResult>(this T obj, Func<T, TResult> func)
        {
            return func(obj);
        }
    }
}
