using AutoMapper;
using HOMMS.Application.Interfaces;
using HOMMS.Domain.Dtos;
using HOMMS.Infrastructure.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Application.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public OrderService(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<OrderDto>> GetOrderListByChefAsync(int branchId)
        {
            var orders = await _orderRepository.GetOrderListByChefAsync(branchId);
            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task<bool> UpdateOrderStatusByChefAsync(int orderId, string status)
        {
            return await _orderRepository.UpdateOrderStatusByChefAsync(orderId, status);
        }
    }
}
