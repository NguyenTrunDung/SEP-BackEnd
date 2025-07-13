using System.ComponentModel.DataAnnotations;

namespace HOMMS.Domain.Dtos
{
    /// <summary>
    /// DTO for Disease Category entity
    /// </summary>
    public class DiseaseCategoryDto
    {
        public int Id { get; set; }
        public int BranchId { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [StringLength(20)]
        public string Code { get; set; } = string.Empty;
        
        [StringLength(1000)]
        public string? Description { get; set; }
        
        [StringLength(1000)]
        public string? DietaryRestrictions { get; set; }
        
        [StringLength(1000)]
        public string? RecommendedFoods { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        [Range(1, 4)]
        public int SeverityLevel { get; set; } = 1;
        
        public bool RequiresApproval { get; set; } = false;
        
        [StringLength(7)]
        public string? ColorCode { get; set; }
        
        public int SortOrder { get; set; } = 0;
        
        // Audit information
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public string? LastModifiedBy { get; set; }
        
        // Navigation/Display properties
        public string? BranchName { get; set; }
        public int? TotalPatients { get; set; }
        public int? TotalFoodRestrictions { get; set; }
    }
    
    /// <summary>
    /// DTO for creating a new Disease Category - simplified to require only Name
    /// </summary>
    public class CreateDiseaseCategoryDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        // Optional fields with default values
        [StringLength(20)]
        public string? Code { get; set; }
        
        [StringLength(1000)]
        public string? Description { get; set; }
        
        [StringLength(1000)]
        public string? DietaryRestrictions { get; set; }
        
        [StringLength(1000)]
        public string? RecommendedFoods { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        [Range(1, 4)]
        public int SeverityLevel { get; set; } = 1;
        
        public bool RequiresApproval { get; set; } = false;
        
        [StringLength(7)]
        public string? ColorCode { get; set; }
        
        public int SortOrder { get; set; } = 0;
    }
    
    /// <summary>
    /// DTO for updating an existing Disease Category - simplified to require only Name
    /// </summary>
    public class UpdateDiseaseCategoryDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        // Optional fields with default values
        [StringLength(1000)]
        public string? Description { get; set; }
        
        [StringLength(1000)]
        public string? DietaryRestrictions { get; set; }
        
        [StringLength(1000)]
        public string? RecommendedFoods { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        [Range(1, 4)]
        public int SeverityLevel { get; set; } = 1;
        
        public bool RequiresApproval { get; set; } = false;
        
        [StringLength(7)]
        public string? ColorCode { get; set; }
        
        public int SortOrder { get; set; } = 0;
    }
}