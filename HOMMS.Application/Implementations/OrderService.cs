using HOMMS.Application.Interfaces;
using HOMMS.Domain.Dtos;
using HOMMS.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Application.Implementations
{
    public class OrderService: IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<OrderDetailsDto>> GetOrderDetailsByOrderIdAsync(int orderId)
        {
            var orderDetails = await _unitOfWork.OrderDetailsRepository
                .GetAll()
                .Include(od => od.Food)
                .Include(od => od.Menu)
            .Where(od => od.OrderId == orderId)
                .ToListAsync();

            return orderDetails.Select(od => new OrderDetailsDto
            {
                Id = od.Id,
                Qty = od.Qty,
                Price = od.Price,
                Total = od.Total,
                Note = od.Note,
                FoodName = od.Food?.Name,
                MenuName = od.Menu?.Name
            }).ToList();
        }
    }
}
