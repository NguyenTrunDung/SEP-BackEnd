using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HOMMS.Domain.Entities;

namespace HOMMS.Infrastructure.Repositories.Interfaces
{
    public interface IOrderRepository : IRepository<Order, int>
    {
        /// <summary>
        /// Retrieve orders that chef needs to prepare, filter by branch ID
        /// </summary>
        /// <param name="branchId">Branch ID</param>
        /// <returns>A list of orders for chef</returns>
        Task<IEnumerable<Order>> GetOrderListByChefAsync(int branchId);
        Task<bool> UpdateOrderStatusByChefAsync(int orderId, string status);
    }
}
