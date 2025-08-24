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

        /// <summary>
        /// Gets order details with comprehensive patient information for kitchen view
        /// </summary>
        /// <param name="orderId">The order ID</param>
        /// <returns>Order details with patient information</returns>
        [HttpGet("order/{orderId}/with-patient-info")]
        public async Task<ActionResult<ApiResponseBase<List<OrderDetailsDto>>>> GetOrderDetailsWithPatientInfo(int orderId)
        {
            try
            {
                var orderDetails = await _orderService.GetOrderDetailsWithPatientInfoAsync(orderId);
                if (orderDetails == null || orderDetails.Count == 0)
                    return NotFound(new ApiResponseBase<List<OrderDetailsDto>>(null, "Order details not found", "error", 0));
                
                var totalCount = orderDetails.Count;
                return Ok(new ApiResponseBase<List<OrderDetailsDto>>(orderDetails, "Order details with patient information retrieved successfully", "success", totalCount));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseBase<List<OrderDetailsDto>>(null, $"Error retrieving order details: {ex.Message}", "error", 0));
            }
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
