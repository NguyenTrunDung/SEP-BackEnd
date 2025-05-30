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
        Task<IEnumerable<OrderDto>> GetOrderListByChefAsync(int branchId);
        Task<bool> UpdateOrderStatusByChefAsync(int orderId, string status);
    }
}
