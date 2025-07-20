using HOMMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Infrastructure.Repositories.Interfaces
{
    public interface IOrderRepository : IRepository<Order, int>
    {
        /// <summary>
        /// Retrieves a list of orders for a chef by a specific branch ID
        /// </summary>
        Task<IEnumerable<Order>> GetOrderListByChefAsync(int branchId);

        /// <summary>
        /// Allows a chef to update the status of an order
        /// </summary>
        Task<bool> UpdateOrderStatusByChefAsync(int orderId);

        Task<List<Order>> GetOrdersByBranchIdAsync(int branchId);
        /// <summary>
        /// Tìm kiếm đơn hàng theo từ khóa, áp dụng trên các trường: Code, CustomerName, CustomerPhone, Status, ReceiveTime.
        /// </summary>
        Task<List<Order>> SearchOrdersAsync(string keyword);

        /// <summary>
        /// Lọc đơn hàng theo các tiêu chí cụ thể: ngày đơn hàng, ngày nhận, thời gian nhận, trạng thái, tên KH, số điện thoại KH, tổng tiền, mã đơn.
        /// </summary>
        Task<List<Order>> FilterOrdersAsync(
            DateTime? startOrderDate,
            DateTime? endOrderDate,
            DateTime? startReceiveDate,
            DateTime? endReceiveDate,
            string? receiveTime,
            string? status,
            string? customerName,
            string? customerPhone,
            int? minTotal,
            int? maxTotal,
            string? code
        );

        //add order with location
        Task<Order> AddOrderV2Async(Order order);

        /// <summary>
        /// Gets orders by branch ID with optional filtering and search capabilities
        /// </summary>
        Task<List<Order>> GetOrdersByBranchWithFiltersAsync(
            int branchId,
            DateTime? startOrderDate = null,
            DateTime? endOrderDate = null,
            DateTime? startReceiveDate = null,
            DateTime? endReceiveDate = null,
            string? receiveTime = null,
            string? status = null,
            string? customerName = null,
            string? customerPhone = null,
            int? minTotal = null,
            int? maxTotal = null,
            string? code = null,
            string? keyword = null,
            bool? isPaid = null
        );

    }
}
