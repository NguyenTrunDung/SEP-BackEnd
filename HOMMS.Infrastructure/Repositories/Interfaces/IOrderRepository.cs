<<<<<<< HEAD
﻿using HOMMS.Domain.Entities;
using System;
=======
﻿using System;
>>>>>>> e3865cdc654066f1ea2d6f094ce2e277511e07c4
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
<<<<<<< HEAD
=======
using HOMMS.Domain.Entities;
>>>>>>> e3865cdc654066f1ea2d6f094ce2e277511e07c4

namespace HOMMS.Infrastructure.Repositories.Interfaces
{
    public interface IOrderRepository : IRepository<Order, int>
    {
<<<<<<< HEAD
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

=======
        /// <summary>
        /// Retrieve orders that chef needs to prepare, filter by branch ID
        /// </summary>
        /// <param name="branchId">Branch ID</param>
        /// <returns>A list of orders for chef</returns>
>>>>>>> e3865cdc654066f1ea2d6f094ce2e277511e07c4
        Task<IEnumerable<Order>> GetOrderListByChefAsync(int branchId);
        Task<bool> UpdateOrderStatusByChefAsync(int orderId, string status);
    }
}
