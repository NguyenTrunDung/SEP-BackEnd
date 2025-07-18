using Asp.Versioning;
using HOMMS.Application.Interfaces;
using HOMMS.Common.Helpers;
using HOMMS.Domain.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace HOMMS.API.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IVnPayService _vnPayService;
        private readonly IConfiguration _configuration;

        public PaymentController(IVnPayService vnPayService, IConfiguration configuration)
        {
            _vnPayService = vnPayService;
            _configuration = configuration;
        }

        [HttpPost("create-vnpay-payment")]
        public async Task<IActionResult> CreateVnPayPaymentAsync([FromBody] CreateVnPayRequestDto request)
        {
            try
            {
                Console.WriteLine($"[CreateVnPayPayment] Received request at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
                Console.WriteLine($"[CreateVnPayPayment] OrderId: {request?.OrderId}");
                Console.WriteLine($"[CreateVnPayPayment] Amount: {request?.Amount}");

                if (request == null)
                {
                    Console.WriteLine("[CreateVnPayPayment] ERROR: Request is null");
                    return BadRequest(new ApiResponseBase<string>(null, "Payment request data is required", "error"));
                }

                if (request.OrderId <= 0)
                {
                    Console.WriteLine("[CreateVnPayPayment] ERROR: OrderId is missing or invalid");
                    return BadRequest(new ApiResponseBase<string>(null, "Order ID must be greater than 0", "error"));
                }

                if (request.Amount <= 0)
                {
                    Console.WriteLine($"[CreateVnPayPayment] ERROR: Invalid amount: {request.Amount}");
                    return BadRequest(new ApiResponseBase<string>(null, "Amount must be greater than 0", "error"));
                }

                Console.WriteLine("[CreateVnPayPayment] Request validation passed, generating payment URL...");

                // Fix: Pass OrderId as an integer instead of converting to string
                var paymentUrl = await _vnPayService.GeneratePaymentUrlAsync(request.OrderId, request.Amount);

                Console.WriteLine($"[CreateVnPayPayment] Payment URL generated successfully");
                Console.WriteLine($"[CreateVnPayPayment] URL length: {paymentUrl?.Length ?? 0}");

                return Ok(new ApiResponseBase<string>(paymentUrl, "Payment URL created", "success"));
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine($"[CreateVnPayPayment] ArgumentNullException: {ex.Message}");
                Console.WriteLine($"[CreateVnPayPayment] StackTrace: {ex.StackTrace}");
                return BadRequest(new ApiResponseBase<string>(null, $"Missing required parameter: {ex.ParamName}", "error"));
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"[CreateVnPayPayment] InvalidOperationException: {ex.Message}");
                Console.WriteLine($"[CreateVnPayPayment] StackTrace: {ex.StackTrace}");
                return BadRequest(new ApiResponseBase<string>(null, $"VNPay configuration error: {ex.Message}", "error"));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CreateVnPayPayment] Unexpected Exception: {ex.GetType().Name}");
                Console.WriteLine($"[CreateVnPayPayment] Message: {ex.Message}");
                Console.WriteLine($"[CreateVnPayPayment] StackTrace: {ex.StackTrace}");

                if (ex.InnerException != null)
                {
                    Console.WriteLine($"[CreateVnPayPayment] Inner Exception: {ex.InnerException.GetType().Name}");
                    Console.WriteLine($"[CreateVnPayPayment] Inner Message: {ex.InnerException.Message}");
                }

                return StatusCode(500, new ApiResponseBase<string>(null, "Failed to create payment URL. Please try again.", "error"));
            }
        }

        [HttpGet("vnpay-return")]
        public async Task<IActionResult> VnPayReturnAsync()
        {
            try
            {
                Console.WriteLine($"[VnPayReturn] Received return request at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
                
                var query = Request.Query;
                Console.WriteLine($"[VnPayReturn] Query parameters count: {query.Count}");
                
                // Log key VNPay parameters for debugging
                foreach (var param in query)
                {
                    Console.WriteLine($"[VnPayReturn] {param.Key}: {param.Value}");
                }

                if (query == null || query.Count == 0)
                {
                    Console.WriteLine("[VnPayReturn] ERROR: No query parameters received");
                    var errorUrl = BuildFrontendUrl("error", null, "No payment data received from VNPay", null, null);
                    return Redirect(errorUrl);
                }

                Console.WriteLine("[VnPayReturn] Processing VNPay return...");

                var response = await _vnPayService.ProcessVnPayReturnAsync(query);

                Console.WriteLine($"[VnPayReturn] VNPay service response - IsSuccess: {response.IsSuccess}");
                Console.WriteLine($"[VnPayReturn] VNPay service response - Message: {response.Message}");

                // Build frontend redirect URL with payment result
                var status = response.IsSuccess ? "success" : "failed";
                var redirectUrl = BuildFrontendUrl(
                    status, 
                    response.OrderId, 
                    response.Message, 
                    response.TransactionId, 
                    response.Amount
                );

                Console.WriteLine($"[VnPayReturn] Redirecting to frontend: {redirectUrl}");
                return Redirect(redirectUrl);
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine($"[VnPayReturn] ArgumentNullException: {ex.Message}");
                Console.WriteLine($"[VnPayReturn] StackTrace: {ex.StackTrace}");
                var errorUrl = BuildFrontendUrl("error", null, "Invalid payment data received", null, null);
                return Redirect(errorUrl);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"[VnPayReturn] InvalidOperationException: {ex.Message}");
                Console.WriteLine($"[VnPayReturn] StackTrace: {ex.StackTrace}");
                var errorUrl = BuildFrontendUrl("error", null, $"Payment processing error: {ex.Message}", null, null);
                return Redirect(errorUrl);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[VnPayReturn] Unexpected Exception: {ex.GetType().Name}");
                Console.WriteLine($"[VnPayReturn] Message: {ex.Message}");
                Console.WriteLine($"[VnPayReturn] StackTrace: {ex.StackTrace}");
                
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"[VnPayReturn] Inner Exception: {ex.InnerException.GetType().Name}");
                    Console.WriteLine($"[VnPayReturn] Inner Message: {ex.InnerException.Message}");
                }

                var errorUrl = BuildFrontendUrl("error", null, "Failed to process payment return. Please contact support.", null, null);
                return Redirect(errorUrl);
            }
        }

        [HttpGet("vnpay-ipn")]
        public async Task<IActionResult> VnPayIpnAsync()
        {
            var query = Request.Query;
            var response = await _vnPayService.ProcessVnPayReturnAsync(query, isIpn: true);

            if (response.IsSuccess)
                return Ok("00"); // VNPAY yêu cầu trả "00" khi xử lý thành công
            else
                return BadRequest("99"); // Trả "99" nếu xử lý thất bại
        }

        /// <summary>
        /// Helper method to build frontend redirect URL with payment result parameters
        /// </summary>
        /// <param name="status">Payment status: success, failed, error</param>
        /// <param name="orderId">Order ID</param>
        /// <param name="message">Payment message</param>
        /// <param name="transactionId">VNPay transaction ID</param>
        /// <param name="amount">Payment amount</param>
        /// <returns>Complete frontend URL with query parameters</returns>
        private string BuildFrontendUrl(string status, int? orderId, string message, string transactionId, long? amount)
        {
            try
            {
                // Get frontend base URL from configuration
                var frontendBaseUrl = _configuration["Frontend:BaseUrl"] ?? "http://localhost:3000";
                
                // Ensure URL ends with /vnpay-return
                var baseUrl = $"{frontendBaseUrl.TrimEnd('/')}/vnpay-return";
                
                // Build query parameters
                var queryParams = new Dictionary<string, string>
                {
                    ["status"] = status
                };

                if (orderId.HasValue)
                    queryParams["orderId"] = orderId.Value.ToString();

                if (!string.IsNullOrEmpty(message))
                    queryParams["message"] = Uri.EscapeDataString(message);

                if (!string.IsNullOrEmpty(transactionId))
                    queryParams["transactionId"] = transactionId;

                if (amount.HasValue)
                    queryParams["amount"] = amount.Value.ToString();

                // Build final URL with query parameters
                var queryString = string.Join("&", 
                    queryParams.Where(kv => !string.IsNullOrEmpty(kv.Value))
                               .Select(kv => $"{kv.Key}={kv.Value}")
                );

                var finalUrl = $"{baseUrl}?{queryString}";
                
                Console.WriteLine($"[BuildFrontendUrl] Generated URL: {finalUrl}");
                return finalUrl;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[BuildFrontendUrl] Error building URL: {ex.Message}");
                
                // Fallback to simple error URL
                var frontendBaseUrl = _configuration["Frontend:BaseUrl"] ?? "http://localhost:3000";
                return $"{frontendBaseUrl.TrimEnd('/')}/vnpay-return?status=error&message={Uri.EscapeDataString("Error processing payment")}";
            }
        }

    }
}
