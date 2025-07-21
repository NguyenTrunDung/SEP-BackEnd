using Asp.Versioning;
using HOMMS.Application.Interfaces;
using HOMMS.Common.Helpers;
using HOMMS.Domain.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HOMMS.API.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
 
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

        //[HttpGet("order-kitchen")]
        //public async Task<ActionResult<ApiResponseBase<List<OrderDto>>>> GetOrderDetailsByStatusPrepare()
        //{
     
        //    return Ok();
        //}

        //[HttpGet("order-delivery")]
        //public async Task<ActionResult<ApiResponseBase<List<OrderDto>>>> GetOrderDetailsByStatusDelivering()
        //{

        //    return Ok();
        //}
    }
}
