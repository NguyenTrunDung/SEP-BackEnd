using HOMMS.Common.Constants;
using System.Globalization;

namespace HOMMS.API.Middleware
{
    /// <summary>
    /// Middleware to ensure consistent timezone handling across the application
    /// </summary>
    public class TimeZoneMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<TimeZoneMiddleware> _logger;

        public TimeZoneMiddleware(RequestDelegate next, ILogger<TimeZoneMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Set the current culture to Vietnam for consistent date formatting
            var vietnamCulture = new CultureInfo("vi-VN");
            Thread.CurrentThread.CurrentCulture = vietnamCulture;
            Thread.CurrentThread.CurrentUICulture = vietnamCulture;

            // Add timezone info to response headers for client reference
            context.Response.Headers.Add("X-Server-Timezone", TimeZoneConstants.VIETNAM_TIMEZONE_IANA);
            context.Response.Headers.Add("X-Server-Utc-Offset", TimeZoneConstants.VietnamTimeZone.GetUtcOffset(DateTime.UtcNow).ToString());

            // Log timezone information for debugging (only in development)
            if (context.Request.Headers.ContainsKey("X-Debug-Timezone"))
            {
                var currentUtc = DateTime.UtcNow;
                var vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(currentUtc, TimeZoneConstants.VietnamTimeZone);
                
                _logger.LogInformation("Timezone Debug - UTC: {UtcTime}, Vietnam: {VietnamTime}, Offset: {Offset}",
                    currentUtc.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    vietnamTime.ToString("yyyy-MM-ddTHH:mm:ss.fff"),
                    TimeZoneConstants.VietnamTimeZone.GetUtcOffset(currentUtc));
            }

            await _next(context);
        }
    }

    /// <summary>
    /// Extension method to register the timezone middleware
    /// </summary>
    public static class TimeZoneMiddlewareExtensions
    {
        public static IApplicationBuilder UseTimeZoneMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TimeZoneMiddleware>();
        }
    }
}
