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

        /// <summary>
        /// Gets or sets the total count of items (for list responses)
        /// </summary>
        [JsonPropertyName("totalCount")]
        public int? TotalCount { get; set; }

        public ApiResponseBase() { }
        public ApiResponseBase(T? data, string? message = null, string status = "success", int? totalCount = null)
        {
            Data = data;
            Message = message;
            Status = status;
            TotalCount = totalCount;
        }

        /// <summary>
        /// Creates a successful response with data and message
        /// </summary>
        /// <param name="data">The data to return</param>
        /// <param name="message">Success message</param>
        /// <param name="totalCount">Total count for pagination (optional)</param>
        /// <returns>ApiResponseBase with success status</returns>
        public static ApiResponseBase<T> Success(T? data, string? message = null, int? totalCount = null)
        {
            return new ApiResponseBase<T>(data, message, "success", totalCount);
        }

        /// <summary>
        /// Creates an error response with message
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="errors">Additional error details (optional)</param>
        /// <returns>ApiResponseBase with error status</returns>
        public static ApiResponseBase<T> Error(string message, object? errors = null)
        {
            var response = new ApiResponseBase<T>(null, message, "error", null);
            if (errors != null)
            {
                // If we need to include validation errors, we can add them as an additional property
                // For now, we'll include the error message in the message field
                response.Message = message;
            }
            return response;
        }
    }

    /// <summary>
    /// Non-generic version for error responses that don't need typed data
    /// </summary>
    public class ApiResponseBase : ApiResponseBase<object>
    {
        public ApiResponseBase() : base() { }
        public ApiResponseBase(object? data, string? message = null, string status = "success", int? totalCount = null)
            : base(data, message, status, totalCount) { }

        /// <summary>
        /// Creates a successful response with data and message
        /// </summary>
        public static new ApiResponseBase Success(object? data, string? message = null, int? totalCount = null)
        {
            return new ApiResponseBase(data, message, "success", totalCount);
        }

        /// <summary>
        /// Creates an error response with message
        /// </summary>
        public static new ApiResponseBase Error(string message, object? errors = null)
        {
            return new ApiResponseBase(null, message, "error", null);
        }
    }
} 