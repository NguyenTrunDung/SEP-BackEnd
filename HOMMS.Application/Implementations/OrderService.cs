using AutoMapper;
using HOMMS.Application.Interfaces;
using HOMMS.Domain.Dtos;
using HOMMS.Domain.Entities;
using HOMMS.Infrastructure.Repositories.Implementations;
using HOMMS.Infrastructure.Repositories.Interfaces;
using Microsoft.Graph.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Application.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IOrderRepository _orderRepository;
        private readonly IPatientRepository _patientRepository;

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper, IOrderRepository orderRepository, IPatientRepository patientRepository)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _orderRepository = orderRepository;
            _patientRepository = patientRepository;
        }

        public async Task<IEnumerable<OrderDto>> GetOrderListByChefAsync(int branchId)
        {
            var orders = await _orderRepository.GetOrderListByChefAsync(branchId);
            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task<bool> UpdateOrderStatusByChefAsync(int orderId)
        {
            return await _orderRepository.UpdateOrderStatusByChefAsync(orderId);
        }

        public async Task<List<OrderDto>> GetOrdersByBranchIdAsync(int branchId)
        {
            var orders = await _unitOfWork.OrderRepository.GetOrdersByBranchIdAsync(branchId);

            return orders.Select(o => new OrderDto
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                ReceiveDate = o.ReceiveDate,
                ReceiveTime = o.ReceiveTime,
                Status = o.Status,
                CustomerName = o.CustomerName,
                CustomerPhone = o.CustomerPhone,
                Total = o.Total,
                Code = o.Code
            }).ToList();
        }

        public async Task<List<OrderDto>> SearchOrdersAsync(string keyword)
        {
            var orders = await _unitOfWork.OrderRepository.SearchOrdersAsync(keyword);

            return orders.Select(o => new OrderDto
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                ReceiveDate = o.ReceiveDate,
                ReceiveTime = o.ReceiveTime,
                Status = o.Status,
                CustomerName = o.CustomerName,
                CustomerPhone = o.CustomerPhone,
                Total = o.Total,
                Code = o.Code
            }).ToList();
        }

        public async Task<List<OrderDto>> FilterOrdersAsync(
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
            string? code)
        {
            var orders = await _unitOfWork.OrderRepository.FilterOrdersAsync(
                startOrderDate,
                endOrderDate,
                startReceiveDate,
                endReceiveDate,
                receiveTime,
                status,
                customerName,
                customerPhone,
                minTotal,
                maxTotal,
                code
            );

            return orders.Select(o => new OrderDto
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                ReceiveDate = o.ReceiveDate,
                ReceiveTime = o.ReceiveTime,
                Status = o.Status,
                CustomerName = o.CustomerName,
                CustomerPhone = o.CustomerPhone,
                Total = o.Total,
                Code = o.Code
            }).ToList();
        }



        public async Task<OrderDto> AddAsync(OrderDto entity)
        {
            var or = _mapper.Map<Order>(entity);
           
            var der = await _orderRepository.AddAsync(or);
            return _mapper.Map<OrderDto>(der);
        }



        public async Task<OrderDto> AddPatientOrderAsync(CreatePatientOrderDto entity)
        {
            var or = _mapper.Map<Order>(entity);
            var pa = await _patientRepository.GetByIdAsync(entity.PatientId);
            var saved = await _orderRepository.AddAsync(or);        
            return _mapper.Map<OrderDto>(or);
        }

        public async Task<OrderDto> UpdateAsync(int id, UpdateOrderDto entity)
        {
            var or = await _orderRepository.GetByIdAsync(id);
            if (or == null) return null;
            _mapper.Map(entity, or);
            await _orderRepository.UpdateAsync(or);
            return _mapper.Map<OrderDto>(or);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var or = await _orderRepository.GetByIdAsync(id);
            if (or == null) return false;
            await _orderRepository.DeleteAsync(or);
            return true;
        }


        public async Task<OrderDto> GetByIdAsync(int id)
        {
            var or = await _orderRepository.GetByIdAsync(id);
            return _mapper.Map<OrderDto>(or);
        }

        //add order with location
        public async Task<OrderDto> AddOrderV2Async(OrderDtoV2 dto)
        {
            var order = _mapper.Map<Order>(dto);

            var result = await _orderRepository.AddOrderV2Async(order);

            return _mapper.Map<OrderDto>(result);
        }


    }
}
