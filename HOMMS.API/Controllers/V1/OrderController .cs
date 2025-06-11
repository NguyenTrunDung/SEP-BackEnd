using HOMMS.Application.Interfaces;
using HOMMS.Domain.Dtos;
using HOMMS.Common.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HOMMS.API.Controllers.V1
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet("branch/{branchId}")]
        public async Task<IActionResult> GetOrdersByBranchId(int branchId)
        {
            var orders = await _orderService.GetOrdersByBranchIdAsync(branchId);
            var totalCount = orders?.Count ?? 0;
            return Ok(new ApiResponseBase<List<OrderDto>>(orders, "Orders retrieved successfully", "success", totalCount));
        }
        // GET: api/order/search?keyword=abc
        [HttpGet("search")]
        public async Task<IActionResult> SearchOrders([FromQuery] string keyword)
        {
            var orders = await _orderService.SearchOrdersAsync(keyword);
            var totalCount = orders?.Count ?? 0;
            return Ok(new ApiResponseBase<List<OrderDto>>(orders, "Orders retrieved successfully", "success", totalCount));
        }

        [HttpGet("filter")]
        public async Task<IActionResult> FilterOrders(
            [FromQuery] DateTime? startOrderDate,
            [FromQuery] DateTime? endOrderDate,
            [FromQuery] DateTime? startReceiveDate,
            [FromQuery] DateTime? endReceiveDate,
            [FromQuery] string? receiveTime,
            [FromQuery] string? status,
            [FromQuery] string? customerName,
            [FromQuery] string? customerPhone,
            [FromQuery] int? minTotal,
            [FromQuery] int? maxTotal,
            [FromQuery] string? code)
        {
            var orders = await _orderService.FilterOrdersAsync(
                startOrderDate,
                endOrderDate,
                startReceiveDate,
                endReceiveDate,
                receiveTime,
                status,
                customerName,
                customerPhone,
                minTotal,
                maxTotal,
                code
            );
            var totalCount = orders?.Count ?? 0;
            return Ok(new ApiResponseBase<List<OrderDto>>(orders, "Orders retrieved successfully", "success", totalCount));
        }

    }
}
