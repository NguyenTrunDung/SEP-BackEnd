using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using HOMMS.Domain.Enums;

namespace HOMMS.Domain.Dtos
{
    /// <summary>
    /// DTO for order display
    /// </summary>
    public class OrderDto
    {
        public int Id { get; set; }
        public int BranchId { get; set; }
        public string? UserId { get; set; }
        public string? PatientId { get; set; }
        public bool IsPatientOrder { get; set; }
        
        public DateTime OrderDate { get; set; }
        public DateTime? ReceiveDate { get; set; }
        public string? ReceiveTime { get; set; }
        public string? ReceiveType { get; set; }
        public string? Type { get; set; }
        public string? Status { get; set; }
        
        // Customer information
        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }
        public string? CustomerAddress { get; set; }
        
        // Pricing
        public int? Total { get; set; }
        public int? ShippingFee { get; set; }
        public int? FoodToolFee { get; set; }
        public OrderPaymentMethod PaymentMethod { get; set; }
        public bool IsPaid { get; set; }
        public long? WalletAmountUsed { get; set; }
        
        public string? Code { get; set; }
        public string? Note { get; set; }
        
        // Patient-specific information (when IsPatientOrder = true)
        public string? PatientName { get; set; }
        public string? PatientMedicalRecordNumber { get; set; }
        public string? PatientRoomNumber { get; set; }
        public string? PatientBedNumber { get; set; }
        public string? AttendingPhysician { get; set; }
        public bool RequiresDietarySupervision { get; set; }
        
        // Related data
        public string BranchName { get; set; } = string.Empty;
        public List<OrderDetailsDto> OrderDetails { get; set; } = new List<OrderDetailsDto>();
        
        // Computed properties
        public string OrderTypeDisplay => IsPatientOrder ? "Đơn hàng bệnh nhân" : "Đơn hàng khách hàng";
        public string LocationDisplay => !string.IsNullOrEmpty(PatientRoomNumber) 
            ? $"Phòng {PatientRoomNumber}" + (!string.IsNullOrEmpty(PatientBedNumber) ? $" - Giường {PatientBedNumber}" : "")
            : CustomerAddress ?? "";
    }

    /// <summary>
    /// DTO for update patient orders
    /// </summary>
    public class UpdateOrderDto
    {

        public bool IsPatientOrder { get; set; } = false;

        public DateTime OrderDate { get; set; }
        public DateTime? ReceiveDate { get; set; }
        public string? ReceiveTime { get; set; }
        public string? ReceiveType { get; set; }
        public string? Type { get; set; }
        public string? Status { get; set; }

        // Customer information
        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }
        public string? CustomerAddress { get; set; }

        // Pricing
        public int? Total { get; set; }
        public int? ShippingFee { get; set; }
        public int? FoodToolFee { get; set; }
        public OrderPaymentMethod PaymentMethod { get; set; }
        public bool IsPaid { get; set; }
        public long? WalletAmountUsed { get; set; }

        public string? Code { get; set; }
        public string? Note { get; set; }

     
      
        

    }



    /// <summary>
    /// DTO for creating patient orders
    /// </summary>
    public class CreatePatientOrderDto
    {
      
        public int BranchId { get; set; }
        public string? UserId { get; set; }
        public string? PatientId { get; set; }
        public bool IsPatientOrder { get; set; } = true;

        public DateTime OrderDate { get; set; }
        public DateTime? ReceiveDate { get; set; }
        public string? ReceiveTime { get; set; }
        public string? ReceiveType { get; set; }
        public string? Type { get; set; }
        public string? Status { get; set; }

        // Customer information
        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }
        public string? CustomerAddress { get; set; }

        // Pricing
        public int? Total { get; set; }
        public int? ShippingFee { get; set; }
        public int? FoodToolFee { get; set; }
        public OrderPaymentMethod PaymentMethod { get; set; }
        public bool IsPaid { get; set; }
        public long? WalletAmountUsed { get; set; }

        public string? Code { get; set; }
        public string? Note { get; set; }

        // Patient-specific information (when IsPatientOrder = true)
        public string? PatientName { get; set; }
        public string? PatientMedicalRecordNumber { get; set; }
        public string? PatientRoomNumber { get; set; }
        public string? PatientBedNumber { get; set; }
        public string? AttendingPhysician { get; set; }
        public bool RequiresDietarySupervision { get; set; }

        // Related data
        public string BranchName { get; set; } = string.Empty;
        public List<OrderDetailsDto> OrderDetails { get; set; } = new List<OrderDetailsDto>();

        // Computed properties
        public string OrderTypeDisplay => IsPatientOrder ? "Đơn hàng bệnh nhân" : "Đơn hàng khách hàng";
        public string LocationDisplay => !string.IsNullOrEmpty(PatientRoomNumber)
            ? $"Phòng {PatientRoomNumber}" + (!string.IsNullOrEmpty(PatientBedNumber) ? $" - Giường {PatientBedNumber}" : "")
            : CustomerAddress ?? "";
    }
    
    /// <summary>
    /// DTO for creating order details
    /// </summary>
    public class CreateOrderDetailDto
    {
        [Required]
        public int FoodId { get; set; }
        
        [Required]
       
        public int Quantity { get; set; }
        
        public string? Note { get; set; }
    }
    
    /// <summary>
    /// DTO for order summary with dietary validation
    /// </summary>
    public class PatientOrderSummaryDto
    {
        public CreatePatientOrderDto Order { get; set; } = new CreatePatientOrderDto();
        public List<DietaryValidationResultDto> DietaryValidations { get; set; } = new List<DietaryValidationResultDto>();
        public bool RequiresPhysicianApproval { get; set; }
        public int TotalWarnings { get; set; }
        public int TotalProhibitions { get; set; }
        public decimal EstimatedTotal { get; set; }
    }
    
    /// <summary>
    /// DTO for dietary validation results
    /// </summary>
    public class DietaryValidationResultDto
    {
        public int FoodId { get; set; }
        public string FoodName { get; set; } = string.Empty;
        public string ValidationStatus { get; set; } = string.Empty; // Safe, Advisory, Warning, Prohibited, Dangerous
        public List<string> RestrictionReasons { get; set; } = new List<string>();
        public List<string> Alternatives { get; set; } = new List<string>();
        public bool RequiresPhysicianOverride { get; set; }
    }
}
