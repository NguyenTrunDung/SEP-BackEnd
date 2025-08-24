using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using HOMMS.Common.Helpers;

namespace HOMMS.API.Converters
{
    /// <summary>
    /// JSON converter for DateTime that handles Vietnam timezone conversion
    /// Stores as UTC in database, converts to Vietnam time for API responses
    /// </summary>
    public class VietnamTimeZoneDateTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var dateString = reader.GetString();
            if (string.IsNullOrEmpty(dateString))
            {
                throw new JsonException("Invalid date format");
            }
            
            return TimeZoneHelper.ParseApiDate(dateString);
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            // Convert UTC to Vietnam time for API response
            var vietnamTime = TimeZoneHelper.ConvertUtcToVietnam(value);
            
            // Write in ISO 8601 format with timezone offset
            writer.WriteStringValue(vietnamTime.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz"));
        }
    }

    /// <summary>
    /// JSON converter for nullable DateTime
    /// </summary>
    public class VietnamTimeZoneNullableDateTimeConverter : JsonConverter<DateTime?>
    {
        public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var dateString = reader.GetString();
            if (string.IsNullOrEmpty(dateString))
            {
                return null;
            }
            
            return TimeZoneHelper.ParseApiDate(dateString);
        }

        public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNullValue();
                return;
            }
            
            // Convert UTC to Vietnam time for API response
            var vietnamTime = TimeZoneHelper.ConvertUtcToVietnam(value.Value);
            
            // Write in ISO 8601 format with timezone offset
            writer.WriteStringValue(vietnamTime.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz"));
        }
    }
}
