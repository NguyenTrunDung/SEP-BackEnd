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
        public static DateTime ParseApiDate(string dateString)
        {
            if (DateTime.TryParse(dateString, out var parsedDate))
            {
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
