using Asp.Versioning;
using HOMMS.Application.Interfaces;
using HOMMS.Common.Helpers;
using HOMMS.Domain.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HOMMS.API.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }


        [HttpGet("chef-order-list/{branchId}")]
        //[Authorize(Policy = "Permission:orders:view")]
        public async Task<IActionResult> GetOrderListByChefAsync(int branchId)
        {
            var orders = await _orderService.GetOrderListByChefAsync(branchId);
            var orderList = orders?.ToList() ?? new List<OrderDto>();
            var totalCount = orderList.Count;
            return Ok(new ApiResponseBase<List<OrderDto>>(orderList, "Chef order list retrieved successfully", "success", totalCount));
        }

        [HttpPut("chef-update-order-status/{orderId}")]
        //[Authorize(Policy = "Permission:orders:edit")]
        public async Task<IActionResult> UpdateOrderStatusByChefAsync(int orderId, [FromBody] string status)
        {
            var success = await _orderService.UpdateOrderStatusByChefAsync(orderId, status);
            return success ? Ok(new ApiResponseBase<object>(null, "Order status updated successfully")) : BadRequest(new ApiResponseBase<object>(null, "Failed to update order status", "error"));
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
