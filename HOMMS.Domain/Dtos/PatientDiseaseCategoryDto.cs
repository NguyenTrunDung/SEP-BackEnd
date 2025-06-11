using System;
using System.ComponentModel.DataAnnotations;

namespace HOMMS.Domain.Dtos
{
    /// <summary>
    /// DTO for patient disease category relationships
    /// </summary>
    public class PatientDiseaseCategoryDto
    {
        public int Id { get; set; }
        public int BranchId { get; set; }
        public string PatientId { get; set; } = string.Empty;
        public int DiseaseCategoryId { get; set; }
        
        public DateTime? DiagnosedDate { get; set; }
        public int? PatientSeverityLevel { get; set; }
        public string? PatientSpecificNotes { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? ExpiryDate { get; set; }
        
        // Audit information - who assigned and modified this disease category
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public string? LastModifiedBy { get; set; }
        
        // Display properties
        public string PatientName { get; set; } = string.Empty;
        public string PatientCode { get; set; } = string.Empty;
        public string DiseaseCategoryName { get; set; } = string.Empty;
        public string DiseaseCategoryCode { get; set; } = string.Empty;
        public string? RoomNumber { get; set; }
        public string? BedNumber { get; set; }
        
        // Computed display properties
        public string AssignedByUserName => CreatedBy ?? "System";
        public string LastModifiedByUserName => LastModifiedBy ?? CreatedBy ?? "System";
    }
    
    /// <summary>
    /// DTO for assigning disease categories to patients
    /// </summary>
    public class AssignPatientDiseaseCategoryDto
    {
        [Required]
        public string PatientId { get; set; } = string.Empty;
        
        [Required]
        public int DiseaseCategoryId { get; set; }
        
        public DateTime? DiagnosedDate { get; set; }
        
        [Range(1, 4)]
        public int? PatientSeverityLevel { get; set; }
        
        [StringLength(1000)]
        public string? PatientSpecificNotes { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public DateTime? ExpiryDate { get; set; }
    }
    
    /// <summary>
    /// DTO for patient dietary information including all disease categories
    /// </summary>
    public class PatientDietaryInfoDto
    {
        public string PatientId { get; set; } = string.Empty;
        public string PatientName { get; set; } = string.Empty;
        public string? PatientCode { get; set; }
        public string? RoomNumber { get; set; }
        public string? BedNumber { get; set; }
        public bool RequiresDietarySupervision { get; set; }
        
        public List<PatientDiseaseCategoryDto> DiseaseCategories { get; set; } = new List<PatientDiseaseCategoryDto>();
        public List<int> RestrictedFoodIds { get; set; } = new List<int>();
        public List<string> DietaryRestrictions { get; set; } = new List<string>();
        public List<string> RecommendedFoods { get; set; } = new List<string>();
        public int HighestSeverityLevel { get; set; }
        public bool RequiresApproval { get; set; }
    }
} 