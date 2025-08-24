using HOMMS.Domain.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Application.Interfaces
{
    public interface IOrderDetailService
    {
        Task<List<OrderDetailsDto>> GetOrderDetailsByOrderIdAsync(int orderId);

        /// <summary>
        /// Gets order details with comprehensive patient information for kitchen view
        /// </summary>
        /// <param name="orderId">The order ID</param>
        /// <returns>Order details with patient information</returns>
        Task<List<OrderDetailsDto>> GetOrderDetailsWithPatientInfoAsync(int orderId);

        /// <summary>
        /// Gets orders with status "Preparing" for kitchen view
        /// Note: This method is implemented in OrderService instead
        /// </summary>
        [Obsolete("Use OrderService.GetOrdersByStatusPreparingAsync instead")]
        Task<List<OrderDto>> GetOrderDetailsByStatusPrepare();

    }
}
