using System.Text.Json.Serialization;

namespace HOMMS.Common.Helpers
{
    /// <summary>
    /// Standard API response wrapper for consistent JSON output
    /// </summary>
    /// <typeparam name="T">Type of the data payload</typeparam>
    public class ApiResponseBase<T> // gerneric type to allow any data type
        where T : class // constraint to ensure T is a reference type
    {
        /// <summary>
        /// Gets or sets the status of the response (success or error)
        /// </summary>
        [JsonPropertyName("status")]
        public string Status { get; set; } = "success";

        /// <summary>
        /// Gets or sets the message for the response
        /// </summary>
        [JsonPropertyName("message")]
        public string? Message { get; set; }

        /// <summary>
        /// Gets or sets the data payload
        /// </summary>
        [JsonPropertyName("data")]
        public T? Data { get; set; }

        public ApiResponseBase() { }
        public ApiResponseBase(T? data, string? message = null, string status = "success")
        {
            Data = data;
            Message = message;
            Status = status;
        }
    }
} 