using HOMMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HOMMS.API.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet("chef-order-list/{branchId}")]
        [Authorize(Policy = "Permission:orders:view")]
        public async Task<IActionResult> GetOrderListByChefAsync(int branchId)
        {
            var orders = await _orderService.GetOrderListByChefAsync(branchId);
            return Ok(orders);
        }

        [HttpPut("chef-update-order-status/{orderId}")]
        [Authorize(Policy = "Permission:orders:edit")]
        public async Task<IActionResult> UpdateOrderStatusByChefAsync(int orderId, [FromBody]string status)
        {
            var success = await _orderService.UpdateOrderStatusByChefAsync(orderId, status);
            return success ? Ok() : BadRequest();
        }
    }
}
