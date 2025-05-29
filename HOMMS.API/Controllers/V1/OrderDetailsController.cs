using HOMMS.Application.Interfaces;
using HOMMS.Domain.Dtos;
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
        public async Task<ActionResult<List<OrderDetailsDto>>> GetOrderDetailsByOrderId(int orderId)
        {
            var orderDetails = await _orderService.GetOrderDetailsByOrderIdAsync(orderId);
            if (orderDetails == null || orderDetails.Count == 0)
                return NotFound();

            return Ok(orderDetails);
        }
    }
}
