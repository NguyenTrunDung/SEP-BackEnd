using Asp.Versioning;
using HOMMS.Application.Interfaces;
using HOMMS.Common.Helpers;
using HOMMS.Domain.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace HOMMS.API.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IVnPayService _vnPayService;

        public PaymentController(IVnPayService vnPayService)
        {
            _vnPayService = vnPayService;
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
                    return BadRequest(new ApiResponseBase<object>(null, "No payment data received from VNPay", "error"));
                }

                Console.WriteLine("[VnPayReturn] Processing VNPay return...");

                var response = await _vnPayService.ProcessVnPayReturnAsync(query);

                Console.WriteLine($"[VnPayReturn] VNPay service response - IsSuccess: {response.IsSuccess}");
                Console.WriteLine($"[VnPayReturn] VNPay service response - Message: {response.Message}");

                if (response.IsSuccess)
                {
                    Console.WriteLine("[VnPayReturn] Payment processed successfully");
                    return Ok(new ApiResponseBase<object>(null, "Payment success via VNPay", "success"));
                }
                else
                {
                    Console.WriteLine($"[VnPayReturn] Payment failed: {response.Message}");
                    return BadRequest(new ApiResponseBase<object>(null, response.Message, "error"));
                }
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine($"[VnPayReturn] ArgumentNullException: {ex.Message}");
                Console.WriteLine($"[VnPayReturn] StackTrace: {ex.StackTrace}");
                return BadRequest(new ApiResponseBase<object>(null, "Invalid payment data received", "error"));
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"[VnPayReturn] InvalidOperationException: {ex.Message}");
                Console.WriteLine($"[VnPayReturn] StackTrace: {ex.StackTrace}");
                return BadRequest(new ApiResponseBase<object>(null, $"Payment processing error: {ex.Message}", "error"));
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

                return StatusCode(500, new ApiResponseBase<object>(null, "Failed to process payment return. Please contact support.", "error"));
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

    }
}
