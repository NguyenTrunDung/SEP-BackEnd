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
            // Log the incoming DTO order details
            Console.WriteLine($"[OrderService.AddOrderV2Async] Incoming DTO OrderDetails Count: {dto?.OrderDetails?.Count ?? 0}");
            if (dto?.OrderDetails != null)
            {
                Console.WriteLine("[OrderService.AddOrderV2Async] Incoming DTO OrderDetails Details:");
                foreach (var detail in dto.OrderDetails)
                {
                    Console.WriteLine($"  - FoodId: {detail.FoodId}, Qty: {detail.Qty}, Note: {detail.Note}, Price: {detail.Price}");
                }
            }

            var order = _mapper.Map<Order>(dto);
            
            // Log the mapped Order entity order details
            Console.WriteLine($"[OrderService.AddOrderV2Async] Mapped Order OrderDetails Count: {order?.OrderDetails?.Count ?? 0}");
            if (order?.OrderDetails != null)
            {
                Console.WriteLine("[OrderService.AddOrderV2Async] Mapped Order OrderDetails Details:");
                foreach (var detail in order.OrderDetails)
                {
                    Console.WriteLine($"  - FoodId: {detail.FoodId}, Qty: {detail.Qty}, Note: {detail.Note}, Price: {detail.Price}");
                }
            }

            var result = await _orderRepository.AddOrderV2Async(order);
            
            // Log the result from repository
            Console.WriteLine($"[OrderService.AddOrderV2Async] Repository Result OrderDetails Count: {result?.OrderDetails?.Count ?? 0}");
            if (result?.OrderDetails != null)
            {
                Console.WriteLine("[OrderService.AddOrderV2Async] Repository Result OrderDetails Details:");
                foreach (var detail in result.OrderDetails)
                {
                    Console.WriteLine($"  - FoodId: {detail.FoodId}, Qty: {detail.Qty}, Note: {detail.Note}, Price: {detail.Price}");
                }
            }

            return _mapper.Map<OrderDto>(result);
        }

        /// <summary>
        /// Gets orders by branch ID with optional filtering and search capabilities
        /// Combines the functionality of GetOrdersByBranchIdAsync, FilterOrdersAsync, and SearchOrdersAsync
        /// </summary>
        public async Task<List<OrderDto>> GetOrdersByBranchWithFiltersAsync(
            int branchId,
            DateTime? startOrderDate = null,
            DateTime? endOrderDate = null,
            DateTime? startReceiveDate = null,
            DateTime? endReceiveDate = null,
            string? receiveTime = null,
            string? status = null,
            bool? IsPatientOrder = null,
            string? customerName = null,
            string? customerPhone = null,
            int? minTotal = null,
            int? maxTotal = null,
            string? code = null,
            string? keyword = null,
            bool? isPaid = null)
        {
            var orders = await _orderRepository.GetOrdersByBranchWithFiltersAsync(
                branchId,
                startOrderDate,
                endOrderDate,
                startReceiveDate,
                endReceiveDate,
                receiveTime,
                status,
                IsPatientOrder,
                customerName,
                customerPhone,
                minTotal,
                maxTotal,
                code,
                keyword,
                isPaid
            );

            return orders.Select(o => new OrderDto
            {
                Id = o.Id,
                BranchId = o.BranchId,
                UserId = o.UserId,
                PatientId = o.PatientId,
                IsPatientOrder = o.IsPatientOrder,
                OrderDate = o.OrderDate,
                ReceiveDate = o.ReceiveDate,
                ReceiveTime = o.ReceiveTime,
                ReceiveType = o.ReceiveType,
                Type = o.Type,
                Status = o.Status,
                CustomerName = o.CustomerName,
                CustomerPhone = o.CustomerPhone,
                CustomerAddress = o.CustomerAddress,
                Total = o.Total,
                ShippingFee = o.ShippingFee,
                FoodToolFee = o.FoodToolFee,
                PaymentMethod = o.PaymentMethod,
                IsPaid = o.IsPaid,
                WalletAmountUsed = o.WalletAmountUsed,
                Code = o.Code,
                Note = o.Note,
                // Patient information
                PatientName = o.Patient?.FullName,
                PatientMedicalRecordNumber = o.Patient?.MedicalRecordNumber,
                PatientRoomNumber = o.Patient?.RoomNumber,
                PatientBedNumber = o.Patient?.BedNumber,
                AttendingPhysician = o.Patient?.AttendingPhysician,
                RequiresDietarySupervision = o.Patient?.RequiresDietarySupervision ?? false,
                // Branch and location information
                BranchName = o.Branch?.Name ?? "",
                // Map OrderDetails with complete information (same as kitchen endpoint)
                OrderDetails = o.OrderDetails?.Select(od => new OrderDetailsDto
                {
                    Id = od.Id,
                    MenuId = od.MenuId,
                    OrderId = od.OrderId,
                    FoodId = od.FoodId,
                    Qty = od.Qty,
                    Price = od.Price,
                    Total = od.Total,
                    Note = od.Note,
                    FoodName = od.Food?.Name ?? od.Menu?.Name,
                    MenuName = od.Menu?.Name,
                    CreatedAt = od.CreatedAt,
                    UpdatedAt = od.LastModifiedAt,
                    // Map detailed Food information if available
                    Food = od.Food != null ? new FoodDto
                    {
                        Id = od.Food.Id,
                        Name = od.Food.Name,
                        BranchId = od.Food.BranchId,
                        CategoryId = od.Food.CategoryId,
                        Description = od.Food.Description,
                        IsSetDish = od.Food.IsSetDish,
                        IsAddOn = od.Food.IsAddOn,
                        ForPatient = od.Food.ForPatient,
                        PriceForGuest = od.Food.PriceForGuest,
                        PriceForPatient = od.Food.PriceForPatient,
                        PriceForStaff = od.Food.PriceForStaff,
                        DiseaseCategoryId = od.Food.DiseaseCategoryId,
                        Sort = od.Food.Sort,
                        CreatedAt = od.Food.CreatedAt,
                        UpdatedAt = od.Food.LastModifiedAt,
                        CreatedBy = od.Food.CreatedBy,
                        UpdatedBy = od.Food.LastModifiedBy,
                        ImageUrl = od.Food.Image,
                        // Map the food category information
                        Category = od.Food.Category != null ? new FoodCategoryDto
                        {
                            Id = od.Food.Category.Id,
                            Name = od.Food.Category.Name,
                            ImageUrl = od.Food.Category.Image ?? "",
                            Sort = od.Food.Category.Sort ?? 0,
                            BranchId = od.Food.Category.BranchId
                        } : null,
                        SetDishDetails = null // Can be populated if needed
                    } : null
                }).ToList() ?? new List<OrderDetailsDto>()
                // Note: OrderTypeDisplay and LocationDisplay are computed properties and don't need to be set
            }).ToList();
        }

        /// <summary>
        /// Gets orders with status "Preparing" for kitchen view with detailed food information
        /// </summary>
        public async Task<List<OrderDto>> GetOrdersByStatusPreparingAsync(int branchId)
        {
            var orders = await _orderRepository.GetOrdersByStatusPreparingAsync(branchId);

            return orders.Select(o => new OrderDto
            {
                Id = o.Id,
                BranchId = o.BranchId,
                UserId = o.UserId,
                PatientId = o.PatientId,
                IsPatientOrder = o.IsPatientOrder,
                OrderDate = o.OrderDate,
                ReceiveDate = o.ReceiveDate,
                ReceiveTime = o.ReceiveTime,
                ReceiveType = o.ReceiveType,
                Type = o.Type,
                Status = o.Status,
                CustomerName = o.CustomerName,
                CustomerPhone = o.CustomerPhone,
                CustomerAddress = o.CustomerAddress,
                Total = o.Total,
                ShippingFee = o.ShippingFee,
                FoodToolFee = o.FoodToolFee,
                PaymentMethod = o.PaymentMethod,
                IsPaid = o.IsPaid,
                WalletAmountUsed = o.WalletAmountUsed,
                Code = o.Code,
                Note = o.Note,
                // Patient information
                PatientName = o.Patient?.FullName,
                PatientMedicalRecordNumber = o.Patient?.MedicalRecordNumber,
                PatientRoomNumber = o.Patient?.RoomNumber,
                PatientBedNumber = o.Patient?.BedNumber,
                AttendingPhysician = o.Patient?.AttendingPhysician,
                RequiresDietarySupervision = o.Patient?.RequiresDietarySupervision ?? false,
                // Branch and location information
                BranchName = o.Branch?.Name ?? "",
                // Map OrderDetails with detailed Food information for kitchen
                OrderDetails = o.OrderDetails?.Select(od => new OrderDetailsDto
                {
                    Id = od.Id,
                    MenuId = od.MenuId,
                    OrderId = od.OrderId,
                    FoodId = od.FoodId,
                    Qty = od.Qty,
                    Price = od.Price,
                    Total = od.Total,
                    Note = od.Note,
                    FoodName = od.Food?.Name,
                    MenuName = od.Menu?.Name,
                    CreatedAt = od.CreatedAt,
                    UpdatedAt = od.LastModifiedAt,
                    // Map detailed Food information
                    Food = od.Food != null ? new FoodDto
                    {
                        Id = od.Food.Id,
                        Name = od.Food.Name,
                        BranchId = od.Food.BranchId,
                        CategoryId = od.Food.CategoryId,
                        Description = od.Food.Description,
                        IsSetDish = od.Food.IsSetDish,
                        IsAddOn = od.Food.IsAddOn,
                        ForPatient = od.Food.ForPatient,
                        PriceForGuest = od.Food.PriceForGuest,
                        PriceForPatient = od.Food.PriceForPatient,
                        PriceForStaff = od.Food.PriceForStaff,
                        DiseaseCategoryId = od.Food.DiseaseCategoryId,
                        Sort = od.Food.Sort,
                        CreatedAt = od.Food.CreatedAt,
                        UpdatedAt = od.Food.LastModifiedAt,
                        CreatedBy = od.Food.CreatedBy,
                        UpdatedBy = od.Food.LastModifiedBy,
                        ImageUrl = od.Food.Image,
                        // Map the food category information
                        Category = od.Food.Category != null ? new FoodCategoryDto
                        {
                            Id = od.Food.Category.Id,
                            Name = od.Food.Category.Name,
                            ImageUrl = od.Food.Category.Image ?? "",
                            Sort = od.Food.Category.Sort ?? 0,
                            BranchId = od.Food.Category.BranchId
                        } : null,
                        SetDishDetails = null // Can be populated if needed
                    } : null
                }).ToList() ?? new List<OrderDetailsDto>()
            }).ToList();
        }


    }
}
