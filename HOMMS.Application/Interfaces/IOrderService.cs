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
        Task<List<OrderDto>> GetOrdersByBranchIdAsync(int branchId);
       
            Task<List<OrderDto>> SearchOrdersAsync(string keyword);
            Task<List<OrderDto>> FilterOrdersAsync(
                int? branchId,
                DateTime? startDate,
                DateTime? endDate,
                string? status,
                string? type,
                bool? hasVat,
                bool? printed);
        

    }
}
