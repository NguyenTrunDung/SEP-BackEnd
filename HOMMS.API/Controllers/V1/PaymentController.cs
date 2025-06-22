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
            var paymentUrl = await _vnPayService.GeneratePaymentUrlAsync(request.OrderId, request.Amount);
            return Ok(new ApiResponseBase<string>(paymentUrl, "Payment URL created", "success"));
        }

        [HttpGet("vnpay-return")]
        public async Task<IActionResult> VnPayReturnAsync()
        {
            var query = Request.Query;
            var response = await _vnPayService.ProcessVnPayReturnAsync(query);

            if (response.IsSuccess)
                return Ok(new ApiResponseBase<object>(null, "Payment success via VNPay", "success"));
            else
                return BadRequest(new ApiResponseBase<object>(null, response.Message, "error"));
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
