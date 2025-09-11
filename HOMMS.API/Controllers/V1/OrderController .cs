using Asp.Versioning;
using HOMMS.Application.Interfaces;
using HOMMS.Common.Helpers;
using HOMMS.Domain.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace HOMMS.API.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]

    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }


        [HttpGet("chef/{branchId}")]
        //[Authorize(Policy = "Permission:kitchen:view")]
        public async Task<ActionResult<ApiResponseBase<List<OrderDto>>>> GetOrderListByChefAsync(int branchId)
        {
            var orders = await _orderService.GetOrderListByChefAsync(branchId);
            var orderList = orders?.ToList() ?? new List<OrderDto>();
            var totalCount = orderList.Count;
            return Ok(new ApiResponseBase<List<OrderDto>>(orderList, "Chef order list retrieved successfully", "success", totalCount));
        }

        [HttpPatch("chef/status/{orderId}")]
        //[Authorize(Policy = "Permission:kitchen:status")]
        public async Task<IActionResult> UpdateOrderStatusByChefAsync(int orderId)
        {
            var success = await _orderService.UpdateOrderStatusByChefAsync(orderId);
            return success ? Ok(new ApiResponseBase<object>(null, "Order status updated successfully")) : BadRequest(new ApiResponseBase<object>(null, "Failed to update order status", "error"));
        }


        [HttpGet("branch/{branchId}")]
        public async Task<IActionResult> GetOrdersByBranchWithFilters(
            int branchId,
            [FromQuery] DateTime? startOrderDate,
            [FromQuery] DateTime? endOrderDate,
            [FromQuery] DateTime? startReceiveDate,
            [FromQuery] DateTime? endReceiveDate,
            [FromQuery] string? receiveTime,
            
            [FromQuery] bool? IsPatientOrder,
            [FromQuery] string? customerName,
            [FromQuery] string? customerPhone,
            [FromQuery] int? minTotal,
            [FromQuery] int? maxTotal,
            [FromQuery] string? code,
            [FromQuery] string? keyword
            )
        {
            var orders = await _orderService.GetOrdersByBranchWithFiltersAsync(
                branchId,
                startOrderDate,
                endOrderDate,
                startReceiveDate,
                endReceiveDate,
                receiveTime,
               
                IsPatientOrder,
                customerName,
                customerPhone,
                minTotal,
                maxTotal,
                code,
                keyword
            );
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

        [HttpGet("order-kitchen")]
        public async Task<ActionResult<ApiResponseBase<List<OrderDto>>>> GetOrderDetailsByStatusPrepare([FromQuery] int branchId)
        {
            var orders = await _orderService.GetOrdersByStatusPreparingAsync(branchId);
            var totalCount = orders?.Count ?? 0;
            return Ok(new ApiResponseBase<List<OrderDto>>(orders, "Kitchen orders retrieved successfully", "success", totalCount));
        }


        [HttpPost("AddDishesforPatient")]
        [Authorize(Policy = "Permission:orders:add")]
        public async Task<ActionResult<ApiResponseBase<OrderDto>>> AddDishesforPatient([FromBody] CreatePatientOrderDto dto)
        {
            var or = await _orderService.AddPatientOrderAsync(dto);
            return Ok(new ApiResponseBase<OrderDto>(or, "Add dishes for patient successfully "));

        }


        [HttpPost("AddOrder")]
        [Authorize(Policy = "Permission:orders:add")]
        public async Task<ActionResult<ApiResponseBase<OrderDto>>> AddOrder([FromBody] OrderDto dto)
        {
            var or = await _orderService.AddAsync(dto);
            return Ok(new ApiResponseBase<OrderDto>(or, "Add order successfully "));

        }



        [HttpPut("UpdateOrder")]
        [Authorize(Policy = "Permission:orders:edit")]
       public async Task<ActionResult<ApiResponseBase<OrderDto>>>UpdateOrder(int id, [FromBody] UpdateOrderDto dto)
        {
            var or = await _orderService.UpdateAsync(id, dto);
            if (or == null)return NotFound(new ApiResponseBase<OrderDto>(null,"Order not found","error"));
            return Ok(new ApiResponseBase<OrderDto>(or, "Order updated successfully"));

        }

        [HttpPatch("UpdateOrderStatus")]
       // [Authorize(Policy = "Permission:orders:edit")]
        public async Task<ActionResult<ApiResponseBase<OrderDto>>> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Status))
            {
                return BadRequest(new ApiResponseBase<OrderDto>(null, "Status is required", "error"));
            }
            
            var or = await _orderService.UpdateOrderStatusAsync(id, dto.Status);
            if (or == null) return NotFound(new ApiResponseBase<OrderDto>(null, "Order not found", "error"));
            return Ok(new ApiResponseBase<OrderDto>(or, "Order status updated successfully"));
        }


        [HttpDelete("DeleteOrder")]
        [Authorize(Policy = "Permission:orders:delete")]
        public async Task<ActionResult<ApiResponseBase<object>>>DeleteOrder(int id)
        {
            var or = await _orderService.DeleteAsync(id);
            if(!or) return NotFound(new ApiResponseBase<object>(null, "Order not found", "error"));
            return Ok(new ApiResponseBase<object>(null, "Order deleted successfully"));

        }

        //add order with location
        [HttpPost("AddOrderV2")]
        //[Authorize(Policy = "Permission:orders:add")]
        public async Task<ActionResult<ApiResponseBase<OrderDto>>> AddOrderV2([FromBody] OrderDtoV2 dto)
        {
            try
            {
                // Log the incoming request data for debugging
                Console.WriteLine($"[AddOrderV2] Received request at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
                Console.WriteLine($"[AddOrderV2] DTO BranchId: {dto?.BranchId}");
                Console.WriteLine($"[AddOrderV2] DTO UserId: {dto?.UserId}");
                Console.WriteLine($"[AddOrderV2] DTO CustomerName: {dto?.CustomerName}");
                Console.WriteLine($"[AddOrderV2] DTO CustomerPhone: {dto?.CustomerPhone}");
                Console.WriteLine($"[AddOrderV2] DTO Total: {dto?.Total}");
                Console.WriteLine($"[AddOrderV2] DTO PaymentMethod: {dto?.PaymentMethod}");
                Console.WriteLine($"[AddOrderV2] DTO Status: {dto?.Status}");
                Console.WriteLine($"[AddOrderV2] DTO OrderDetails Count: {dto?.OrderDetails?.Count ?? 0}");
                Console.WriteLine($"[AddOrderV2] DTO OrderDetails Object debug: {dto?.OrderDetails}");



                // Log detailed order details information
                if (dto?.OrderDetails != null)
                {
                    Console.WriteLine("[AddOrderV2] DTO OrderDetails Details:");
                    foreach (var detail in dto.OrderDetails)
                    {
                        Console.WriteLine($"  - FoodId: {detail.FoodId}, Qty: {detail.Qty}, Note: {detail.Note}, Price: {detail.Price}");
                    }
                }

                // Validate the DTO
                if (dto == null)
                {
                    Console.WriteLine("[AddOrderV2] ERROR: DTO is null");
                    return BadRequest(new ApiResponseBase<OrderDto>(null, "Order data is required", "error"));
                }

                if (dto.BranchId <= 0)
                {
                    Console.WriteLine($"[AddOrderV2] ERROR: Invalid BranchId: {dto.BranchId}");
                    return BadRequest(new ApiResponseBase<OrderDto>(null, "Valid Branch ID is required", "error"));
                }

                if (string.IsNullOrWhiteSpace(dto.CustomerName))
                {
                    Console.WriteLine("[AddOrderV2] ERROR: CustomerName is missing");
                    return BadRequest(new ApiResponseBase<OrderDto>(null, "Customer name is required", "error"));
                }

                if (string.IsNullOrWhiteSpace(dto.CustomerPhone))
                {
                    Console.WriteLine("[AddOrderV2] ERROR: CustomerPhone is missing");
                    return BadRequest(new ApiResponseBase<OrderDto>(null, "Customer phone is required", "error"));
                }

                if (dto.OrderDetails == null || !dto.OrderDetails.Any())
                {
                    Console.WriteLine("[AddOrderV2] ERROR: OrderDetails is empty");
                    return BadRequest(new ApiResponseBase<OrderDto>(null, "Order details are required", "error"));
                }

                if (dto.Total <= 0)
                {
                    Console.WriteLine($"[AddOrderV2] ERROR: Invalid Total: {dto.Total}");
                    return BadRequest(new ApiResponseBase<OrderDto>(null, "Order total must be greater than 0", "error"));
                }

                Console.WriteLine("[AddOrderV2] DTO validation passed, calling service...");

                // Call the service
                var orderResult = await _orderService.AddOrderV2Async(dto);
                
                // Log the result to see what was returned
                Console.WriteLine($"[AddOrderV2] Service returned successfully. Order ID: {orderResult?.Id}");
                if (orderResult?.OrderDetails != null)
                {
                    Console.WriteLine("[AddOrderV2] Result OrderDetails Details:");
                    foreach (var detail in orderResult.OrderDetails)
                    {
                        Console.WriteLine($"  - FoodId: {detail.FoodId}, Qty: {detail.Qty}, Note: {detail.Note}, Price: {detail.Price}");
                    }
                }

                return Ok(new ApiResponseBase<OrderDto>(orderResult, "Add order v2 successfully"));
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine($"[AddOrderV2] ArgumentNullException: {ex.Message}");
                Console.WriteLine($"[AddOrderV2] Parameter: {ex.ParamName}");
                Console.WriteLine($"[AddOrderV2] StackTrace: {ex.StackTrace}");
                return BadRequest(new ApiResponseBase<OrderDto>(null, $"Missing required parameter: {ex.ParamName}", "error"));
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"[AddOrderV2] ArgumentException: {ex.Message}");
                Console.WriteLine($"[AddOrderV2] Parameter: {ex.ParamName}");
                Console.WriteLine($"[AddOrderV2] StackTrace: {ex.StackTrace}");
                return BadRequest(new ApiResponseBase<OrderDto>(null, $"Invalid argument: {ex.Message}", "error"));
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"[AddOrderV2] InvalidOperationException: {ex.Message}");
                Console.WriteLine($"[AddOrderV2] StackTrace: {ex.StackTrace}");
                return BadRequest(new ApiResponseBase<OrderDto>(null, $"Invalid operation: {ex.Message}", "error"));
            }
            catch (System.Data.Common.DbException ex)
            {
                Console.WriteLine($"[AddOrderV2] Database Exception: {ex.Message}");
                Console.WriteLine($"[AddOrderV2] Error Number: {ex.ErrorCode}");
                Console.WriteLine($"[AddOrderV2] StackTrace: {ex.StackTrace}");
                return StatusCode(500, new ApiResponseBase<OrderDto>(null, "Database error occurred. Please try again.", "error"));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AddOrderV2] Unexpected Exception: {ex.GetType().Name}");
                Console.WriteLine($"[AddOrderV2] Message: {ex.Message}");
                Console.WriteLine($"[AddOrderV2] StackTrace: {ex.StackTrace}");
                
                // Log inner exception if exists
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"[AddOrderV2] Inner Exception: {ex.InnerException.GetType().Name}");
                    Console.WriteLine($"[AddOrderV2] Inner Message: {ex.InnerException.Message}");
                    Console.WriteLine($"[AddOrderV2] Inner StackTrace: {ex.InnerException.StackTrace}");
                }

                return StatusCode(500, new ApiResponseBase<OrderDto>(null, $"An unexpected error occurred. Please contact support: {ex.Message}", "error"));
            }
        }

        // Update order payment status (for VNPay integration)
        //[HttpPut("{orderId}/payment-status")]
        //public async Task<ActionResult<ApiResponseBase<object>>> UpdateOrderPaymentStatus(int orderId, [FromBody] UpdatePaymentStatusDto dto)
        //{
        //    try
        //    {
        //        Console.WriteLine($"[UpdateOrderPaymentStatus] Received request at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
        //        Console.WriteLine($"[UpdateOrderPaymentStatus] OrderId: {orderId}");
        //        Console.WriteLine($"[UpdateOrderPaymentStatus] IsPaid: {dto?.IsPaid}");
        //        Console.WriteLine($"[UpdateOrderPaymentStatus] Status: {dto?.Status}");

        //        if (dto == null)
        //        {
        //            Console.WriteLine("[UpdateOrderPaymentStatus] ERROR: DTO is null");
        //            return BadRequest(new ApiResponseBase<object>(null, "Payment status data is required", "error"));
        //        }

        //        if (orderId <= 0)
        //        {
        //            Console.WriteLine($"[UpdateOrderPaymentStatus] ERROR: Invalid OrderId: {orderId}");
        //            return BadRequest(new ApiResponseBase<object>(null, "Valid Order ID is required", "error"));
        //        }

        //        Console.WriteLine("[UpdateOrderPaymentStatus] Request validation passed, updating payment status...");

        //        // Call service to update payment status
        //        var success = await _orderService.UpdateOrderPaymentStatusAsync(orderId, dto.IsPaid, dto.Status);

        //        if (success)
        //        {
        //            Console.WriteLine($"[UpdateOrderPaymentStatus] Payment status updated successfully for order {orderId}");
        //            return Ok(new ApiResponseBase<object>(null, "Order payment status updated successfully", "success"));
        //        }
        //        else
        //        {
        //            Console.WriteLine($"[UpdateOrderPaymentStatus] Failed to update payment status for order {orderId}");
        //            return NotFound(new ApiResponseBase<object>(null, "Order not found or could not be updated", "error"));
        //        }
        //    }
        //    catch (ArgumentNullException ex)
        //    {
        //        Console.WriteLine($"[UpdateOrderPaymentStatus] ArgumentNullException: {ex.Message}");
        //        Console.WriteLine($"[UpdateOrderPaymentStatus] StackTrace: {ex.StackTrace}");
        //        return BadRequest(new ApiResponseBase<object>(null, $"Missing required parameter: {ex.ParamName}", "error"));
        //    }
        //    catch (InvalidOperationException ex)
        //    {
        //        Console.WriteLine($"[UpdateOrderPaymentStatus] InvalidOperationException: {ex.Message}");
        //        Console.WriteLine($"[UpdateOrderPaymentStatus] StackTrace: {ex.StackTrace}");
        //        return BadRequest(new ApiResponseBase<object>(null, $"Invalid operation: {ex.Message}", "error"));
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"[UpdateOrderPaymentStatus] Unexpected Exception: {ex.GetType().Name}");
        //        Console.WriteLine($"[UpdateOrderPaymentStatus] Message: {ex.Message}");
        //        Console.WriteLine($"[UpdateOrderPaymentStatus] StackTrace: {ex.StackTrace}");
                
        //        if (ex.InnerException != null)
        //        {
        //            Console.WriteLine($"[UpdateOrderPaymentStatus] Inner Exception: {ex.InnerException.GetType().Name}");
        //            Console.WriteLine($"[UpdateOrderPaymentStatus] Inner Message: {ex.InnerException.Message}");
        //        }

        //        return StatusCode(500, new ApiResponseBase<object>(null, "An unexpected error occurred while updating payment status.", "error"));
        //    }
        //}

        //// Get order by ID (for VNPay integration)
        //[HttpGet("{orderId}")]
        //public async Task<ActionResult<ApiResponseBase<OrderDto>>> GetOrderById(int orderId)
        //{
        //    try
        //    {
        //        Console.WriteLine($"[GetOrderById] Received request for order {orderId} at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");

        //        if (orderId <= 0)
        //        {
        //            Console.WriteLine($"[GetOrderById] ERROR: Invalid OrderId: {orderId}");
        //            return BadRequest(new ApiResponseBase<OrderDto>(null, "Valid Order ID is required", "error"));
        //        }

        //        var order = await _orderService.GetOrderByIdAsync(orderId);

        //        if (order == null)
        //        {
        //            Console.WriteLine($"[GetOrderById] Order not found: {orderId}");
        //            return NotFound(new ApiResponseBase<OrderDto>(null, "Order not found", "error"));
        //        }

        //        Console.WriteLine($"[GetOrderById] Order found successfully: {orderId}");
        //        return Ok(new ApiResponseBase<OrderDto>(order, "Order retrieved successfully", "success"));
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"[GetOrderById] Unexpected Exception: {ex.GetType().Name}");
        //        Console.WriteLine($"[GetOrderById] Message: {ex.Message}");
        //        Console.WriteLine($"[GetOrderById] StackTrace: {ex.StackTrace}");

        //        return StatusCode(500, new ApiResponseBase<OrderDto>(null, "An unexpected error occurred while retrieving order.", "error"));
        //    }
        //}


    }
}
