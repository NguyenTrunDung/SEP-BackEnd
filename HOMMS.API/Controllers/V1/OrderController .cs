using HOMMS.Application.Interfaces;
using HOMMS.Domain.Dtos;
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
            return Ok(orders);
        }
        // GET: api/order/search?keyword=abc
        [HttpGet("search")]
        public async Task<IActionResult> SearchOrders([FromQuery] string keyword)
        {
            var orders = await _orderService.SearchOrdersAsync(keyword);
            return Ok(orders);
        }

        // GET: api/order/filter?...params...
        [HttpGet("filter")]
        public async Task<IActionResult> FilterOrders(
            [FromQuery] int? branchId,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] string? status,
            [FromQuery] string? type,
            [FromQuery] bool? hasVat,
            [FromQuery] bool? printed)
        {
            var orders = await _orderService.FilterOrdersAsync(branchId, startDate, endDate, status, type, hasVat, printed);
            return Ok(orders);
        }

    }
}
