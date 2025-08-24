using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Domain.Dtos
{
    public class OrderDetailsDto
    {
        public int Id { get; set; }
        public int? MenuId { get; set; }
        public int? OrderId { get; set; }
        public int? FoodId { get; set; }
        public int? Qty { get; set; }
        public int? Price { get; set; }
        public int? Total { get; set; }
        public string? Note { get; set; }
        public string? FoodName { get; set; }
        public string? MenuName { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        
        // For detailed food information in kitchen orders
        public FoodDto? Food { get; set; }
        
        // Patient information for patient orders
        public PatientInfoDto? PatientInfo { get; set; }
    }
    
    // DTO for patient information in order details
    public class PatientInfoDto
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string MedicalRecordNumber { get; set; } = string.Empty;
        public string? RoomNumber { get; set; }
        public string? BedNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public int? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public bool IsActive { get; set; }
    }
}
