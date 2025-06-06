using HOMMS.Domain.Entities.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HOMMS.Domain.Entities
{
    /// <summary>
    /// Represents a disease category with specific dietary requirements and restrictions
    /// Used to manage patient food selections based on medical conditions
    /// </summary>
    public class DiseaseCategory : BaseAuditableEntity<int>, IBranchEntity
    {
        /// <summary>
        /// Gets or sets the branch ID this disease category belongs to
        /// </summary>
        public int BranchId { get; set; }
        
        /// <summary>
        /// Gets or sets the name of the disease category
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        /// <summary>
        /// Gets or sets the code identifier for this disease category
        /// </summary>
        [Required]
        [StringLength(20)]
        public string Code { get; set; } = string.Empty;
        
        /// <summary>
        /// Gets or sets the description of dietary requirements for this disease category
        /// </summary>
        [StringLength(1000)]
        public string? Description { get; set; }
        
        /// <summary>
        /// Gets or sets the dietary restrictions (what foods to avoid)
        /// </summary>
        [StringLength(1000)]
        public string? DietaryRestrictions { get; set; }
        
        /// <summary>
        /// Gets or sets the recommended foods for this disease category
        /// </summary>
        [StringLength(1000)]
        public string? RecommendedFoods { get; set; }
        
        /// <summary>
        /// Gets or sets whether this category is active
        /// </summary>
        public bool IsActive { get; set; } = true;
        
        /// <summary>
        /// Gets or sets the severity level (1=Low, 2=Medium, 3=High, 4=Critical)
        /// Higher severity means stricter dietary requirements
        /// </summary>
        public int SeverityLevel { get; set; } = 1;
        
        /// <summary>
        /// Gets or sets whether patients with this disease category require approval for food orders
        /// </summary>
        public bool RequiresApproval { get; set; } = false;
        
        /// <summary>
        /// Gets or sets the color code for UI display (hex color)
        /// </summary>
        [StringLength(7)]
        public string? ColorCode { get; set; }
        
        /// <summary>
        /// Gets or sets the sort order for display
        /// </summary>
        public int SortOrder { get; set; } = 0;
        
        // Navigation properties
        
        /// <summary>
        /// Gets or sets the branch this disease category belongs to
        /// </summary>
        public virtual Branch? Branch { get; set; }
        
        /// <summary>
        /// Gets or sets the foods associated with this disease category
        /// </summary>
        public virtual ICollection<Food> Foods { get; set; } = new List<Food>();
        
        /// <summary>
        /// Gets or sets the patients associated with this disease category
        /// </summary>
        public virtual ICollection<PatientDiseaseCategory> PatientDiseaseCategories { get; set; } = new List<PatientDiseaseCategory>();
        
        /// <summary>
        /// Gets or sets the disease category restrictions (foods not allowed)
        /// </summary>
        public virtual ICollection<DiseaseCategoryFoodRestriction> FoodRestrictions { get; set; } = new List<DiseaseCategoryFoodRestriction>();
    }
} 