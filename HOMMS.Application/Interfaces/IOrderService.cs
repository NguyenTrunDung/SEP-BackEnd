using HOMMS.Domain.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Application.Interfaces
{
    public interface IOrderService
    {
<<<<<<< HEAD
        Task<List<OrderDto>> GetOrdersByBranchIdAsync(int branchId);

        Task<List<OrderDto>> SearchOrdersAsync(string keyword);

        /// <summary>
        /// Lọc đơn hàng theo các tiêu chí cụ thể: ngày đơn hàng, ngày nhận, thời gian nhận, trạng thái, tên KH, SĐT KH, tổng tiền, mã đơn.
        /// </summary>
        Task<List<OrderDto>> FilterOrdersAsync(
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

=======
>>>>>>> e3865cdc654066f1ea2d6f094ce2e277511e07c4
        Task<IEnumerable<OrderDto>> GetOrderListByChefAsync(int branchId);
        Task<bool> UpdateOrderStatusByChefAsync(int orderId, string status);
    }
}
