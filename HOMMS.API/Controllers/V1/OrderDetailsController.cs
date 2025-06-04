using HOMMS.Application.Interfaces;
using HOMMS.Domain.Dtos;
using HOMMS.Common.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HOMMS.API.Controllers.V1
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderDetailsController : ControllerBase
    {
        private readonly IOrderDetailService _orderService;

        public OrderDetailsController(IOrderDetailService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet("order/{orderId}")]
        public async Task<ActionResult<ApiResponseBase<List<OrderDetailsDto>>>> GetOrderDetailsByOrderId(int orderId)
        {
            var orderDetails = await _orderService.GetOrderDetailsByOrderIdAsync(orderId);
            if (orderDetails == null || orderDetails.Count == 0)
                return NotFound(new ApiResponseBase<List<OrderDetailsDto>>(null, "Order details not found", "error", 0));
            var totalCount = orderDetails.Count;
            return Ok(new ApiResponseBase<List<OrderDetailsDto>>(orderDetails, "Order details retrieved successfully", "success", totalCount));
        }
    }
}
