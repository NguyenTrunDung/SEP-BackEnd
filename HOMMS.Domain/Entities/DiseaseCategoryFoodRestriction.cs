using HOMMS.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace HOMMS.Domain.Entities
{
    /// <summary>
    /// Represents specific food restrictions for disease categories
    /// Defines which foods are prohibited for patients with specific medical conditions
    /// </summary>
    public class DiseaseCategoryFoodRestriction : BaseAuditableEntity<int>, IBranchEntity
    {
        /// <summary>
        /// Gets or sets the branch ID this restriction belongs to
        /// </summary>
        public int BranchId { get; set; }
        
        /// <summary>
        /// Gets or sets the disease category ID
        /// </summary>
        [Required]
        public int DiseaseCategoryId { get; set; }
        
        /// <summary>
        /// Gets or sets the food ID that is restricted
        /// For nutritional meals, this can be null (0) since they don't reference existing foods
        /// </summary>
        public int? FoodId { get; set; }
        
        /// <summary>
        /// Gets or sets the restriction level (1=Advisory, 2=Warning, 3=Prohibited, 4=Dangerous)
        /// </summary>
        public int RestrictionLevel { get; set; } = 3;
        
        /// <summary>
        /// Gets or sets the reason for this restriction
        /// </summary>
        [Required]
        [StringLength(500)]
        public string Reason { get; set; } = string.Empty;
        
        /// <summary>
        /// Gets or sets alternative food recommendations
        /// </summary>
        [StringLength(500)]
        public string? AlternativeRecommendations { get; set; }
        
        /// <summary>
        /// Gets or sets whether this restriction is active
        /// </summary>
        public bool IsActive { get; set; } = true;
        
        /// <summary>
        /// Gets or sets whether this restriction requires physician override to allow the food
        /// </summary>
        public bool RequiresPhysicianOverride { get; set; } = false;
        
        /// <summary>
        /// Gets or sets the meal times when this restriction applies
        /// Comma-separated values: "Sáng,Trưa,Tối" or specific times like "Sáng,Trưa"
        /// If null or empty, applies to all meal times
        /// </summary>
        [StringLength(100)]
        public string? MealTime { get; set; }

        [StringLength(100)]
        public string? Name { get; set; }
        
        /// <summary>
        /// Gets or sets the price of the nutritional meal
        /// Only used when FoodId is null (nutritional meals)
        /// </summary>
        [Range(0, double.MaxValue)]
        public decimal? Price { get; set; }
        
        // Navigation properties
        
        /// <summary>
        /// Gets or sets the branch this restriction belongs to
        /// </summary>
        public virtual Branch? Branch { get; set; }
        
        /// <summary>
        /// Gets or sets the disease category this restriction applies to
        /// </summary>
        public virtual DiseaseCategory? DiseaseCategory { get; set; }
        
        /// <summary>
        /// Gets or sets the food that is restricted
        /// </summary>
        public virtual Food? Food { get; set; }
    }
} 