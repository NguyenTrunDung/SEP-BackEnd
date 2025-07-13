using System.ComponentModel.DataAnnotations;

namespace HOMMS.Domain.Dtos
{
    /// <summary>
    /// DTO for Disease Category Food Restriction entity
    /// </summary>
    public class DiseaseCategoryFoodRestrictionDto
    {
        public int Id { get; set; }
        public int BranchId { get; set; }
        public int DiseaseCategoryId { get; set; }
        public int FoodId { get; set; }
        
        [Range(1, 4)]
        public int RestrictionLevel { get; set; }
        
        [Required]
        [StringLength(500)]
        public string Reason { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? AlternativeRecommendations { get; set; }
        
        public bool IsActive { get; set; } = true;
        public bool RequiresPhysicianOverride { get; set; } = false;
        
        // Audit information
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public string? LastModifiedBy { get; set; }
        
        // Navigation/Display properties
        public string? BranchName { get; set; }
        public string? DiseaseCategoryName { get; set; }
        public string? DiseaseCategoryCode { get; set; }
        public string? FoodName { get; set; }
        public decimal? FoodPrice { get; set; }
        public string? RestrictionLevelName { get; set; }
        public string? RestrictionLevelColor { get; set; }
        
        // Computed properties
        public string RestrictionLevelDisplay => RestrictionLevel switch
        {
            1 => "Advisory",
            2 => "Warning", 
            3 => "Prohibited",
            4 => "Dangerous",
            _ => "Unknown"
        };
        
        public string RestrictionLevelColorCode => RestrictionLevel switch
        {
            1 => "#52c41a", // Green
            2 => "#faad14", // Yellow
            3 => "#ff7a45", // Orange
            4 => "#ff4d4f", // Red
            _ => "#d9d9d9"  // Gray
        };
    }
    
    /// <summary>
    /// DTO for creating a new Disease Category Food Restriction
    /// </summary>
    public class CreateDiseaseCategoryFoodRestrictionDto
    {
        [Required]
        public int DiseaseCategoryId { get; set; }
        
        [Required]
        public int FoodId { get; set; }
        
        [Required]
        [Range(1, 4)]
        public int RestrictionLevel { get; set; } = 3;
        
        [Required]
        [StringLength(500)]
        public string Reason { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? AlternativeRecommendations { get; set; }
        
        public bool IsActive { get; set; } = true;
        public bool RequiresPhysicianOverride { get; set; } = false;
    }
    
    /// <summary>
    /// DTO for updating an existing Disease Category Food Restriction
    /// </summary>
    public class UpdateDiseaseCategoryFoodRestrictionDto
    {
        [Required]
        [Range(1, 4)]
        public int RestrictionLevel { get; set; }
        
        [Required]
        [StringLength(500)]
        public string Reason { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? AlternativeRecommendations { get; set; }
        
        public bool IsActive { get; set; } = true;
        public bool RequiresPhysicianOverride { get; set; } = false;
    }
    
    /// <summary>
    /// DTO for bulk food restriction operations
    /// </summary>
    public class BulkDiseaseCategoryFoodRestrictionDto
    {
        [Required]
        public int DiseaseCategoryId { get; set; }
        
        [Required]
        public List<int> FoodIds { get; set; } = new();
        
        [Required]
        [Range(1, 4)]
        public int RestrictionLevel { get; set; } = 3;
        
        [Required]
        [StringLength(500)]
        public string Reason { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? AlternativeRecommendations { get; set; }
        
        public bool IsActive { get; set; } = true;
        public bool RequiresPhysicianOverride { get; set; } = false;
    }
    
    /// <summary>
    /// DTO for restriction level statistics
    /// </summary>
    public class RestrictionLevelStatsDto
    {
        public int RestrictionLevel { get; set; }
        public string RestrictionLevelName { get; set; } = string.Empty;
        public string RestrictionLevelColor { get; set; } = string.Empty;
        public int TotalRestrictions { get; set; }
        public int ActiveRestrictions { get; set; }
        public int AffectedPatients { get; set; }
    }
} 