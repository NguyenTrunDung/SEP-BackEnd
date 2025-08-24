using HOMMS.Application.BaseServices;
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
    public class OrderDetailService: BaseService, IOrderDetailService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderDetailService(IUnitOfWork unitOfWork, IBranchContext branchContext) : base(branchContext)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<OrderDetailsDto>> GetOrderDetailsByOrderIdAsync(int orderId)
        {
            var orderDetails = await _unitOfWork.OrderDetailsRepository
                .GetAll()
                .Include(od => od.Food)
                    .ThenInclude(f => f.Category)
                .Include(od => od.Menu)
                .Include(od => od.Order)
                    .ThenInclude(o => o.Patient)
                        .ThenInclude(p => p.department) // Include department information
                .Where(od => od.OrderId == orderId)
                .ToListAsync();

            return orderDetails.Select(od => new OrderDetailsDto
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
                // Include patient information if available - ensure consistent population
                PatientInfo = od.Order?.Patient != null ? new PatientInfoDto
                {
                    Id = od.Order.Patient.Id,
                    FullName = od.Order.Patient.FullName,
                    MedicalRecordNumber = od.Order.Patient.MedicalRecordNumber,
                    RoomNumber = od.Order.Patient.RoomNumber,
                    BedNumber = od.Order.Patient.BedNumber,
                    DateOfBirth = od.Order.Patient.DateOfBirth,
                    Gender = od.Order.Patient.Gender,
                    DepartmentId = od.Order.Patient.departmentId,
                    DepartmentName = od.Order.Patient.department?.Name,
                    IsActive = od.Order.Patient.IsActive
                } : null,
                // Map detailed Food information if available (same as kitchen endpoint)
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
            }).ToList();
        }

        [Obsolete("Use OrderService.GetOrdersByStatusPreparingAsync instead")]
        public Task<List<OrderDto>> GetOrderDetailsByStatusPrepare()
        {
            // This method is obsolete and the functionality is moved to OrderService
            throw new NotImplementedException("Use OrderService.GetOrdersByStatusPreparingAsync instead");
        }
        
        /// <summary>
        /// Gets order details with comprehensive patient information for kitchen view
        /// </summary>
        /// <param name="orderId">The order ID</param>
        /// <returns>Order details with patient information</returns>
        public async Task<List<OrderDetailsDto>> GetOrderDetailsWithPatientInfoAsync(int orderId)
        {
            var orderDetails = await _unitOfWork.OrderDetailsRepository
                .GetAll()
                .Include(od => od.Food)
                    .ThenInclude(f => f.Category)
                .Include(od => od.Menu)
                .Include(od => od.Order)
                    .ThenInclude(o => o.Patient)
                        .ThenInclude(p => p.department) // Include department information
                .Where(od => od.OrderId == orderId)
                .ToListAsync();

            return orderDetails.Select(od => new OrderDetailsDto
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
                // Enhanced patient information for kitchen view
                PatientInfo = od.Order?.Patient != null ? new PatientInfoDto
                {
                    Id = od.Order.Patient.Id,
                    FullName = od.Order.Patient.FullName,
                    MedicalRecordNumber = od.Order.Patient.MedicalRecordNumber,
                    RoomNumber = od.Order.Patient.RoomNumber,
                    BedNumber = od.Order.Patient.BedNumber,
                    DateOfBirth = od.Order.Patient.DateOfBirth,
                    Gender = od.Order.Patient.Gender,
                    DepartmentId = od.Order.Patient.departmentId,
                    DepartmentName = od.Order.Patient.department?.Name,
                    IsActive = od.Order.Patient.IsActive
                } : null,
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
                    Category = od.Food.Category != null ? new FoodCategoryDto
                    {
                        Id = od.Food.Category.Id,
                        Name = od.Food.Category.Name,
                        ImageUrl = od.Food.Category.Image ?? "",
                        Sort = od.Food.Category.Sort ?? 0,
                        BranchId = od.Food.Category.BranchId
                    } : null,
                    SetDishDetails = null
                } : null
            }).ToList();
        }
    }
}
