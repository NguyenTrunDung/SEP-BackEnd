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
        Task<List<Order>> GetOrdersByBranchIdAsync(int branchId);
        Task<List<Order>> SearchOrdersAsync(string keyword);

        // Lọc theo các tiêu chí: chi nhánh, ngày, trạng thái, loại đơn, VAT, in ấn
        Task<List<Order>> FilterOrdersAsync(
            int? branchId,
            DateTime? startDate,
            DateTime? endDate,
            string? status,
            string? type,
            bool? hasVat,
            bool? printed
        );
    }
}
