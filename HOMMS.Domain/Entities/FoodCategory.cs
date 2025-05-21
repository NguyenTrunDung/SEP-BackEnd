using HOMMS.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HOMMS.Domain.Entities
{
    /// <summary>
    /// Represents a food category
    /// </summary>
    public class FoodCategory : BaseAuditableEntity<int>, IBranchEntity
    {
        /// <summary>
        /// Gets or sets the name of the food category
        /// </summary>
        [Required]
        [StringLength(255)]
        public string Name { get; set; }
        
        /// <summary>
        /// Gets or sets the branch ID this category belongs to
        /// </summary>
        public int BranchId { get; set; }
        
        /// <summary>
        /// Gets or sets the image URL for this category
        /// </summary>
        public string? Image { get; set; }
        
        /// <summary>
        /// Gets or sets the sort order for this category
        /// </summary>
        public int? Sort { get; set; }
        
        /// <summary>
        /// Gets or sets whether this category is active
        /// </summary>
        public bool Active { get; set; } = true;
        
        /// <summary>
        /// Gets or sets the branch navigation property
        /// </summary>
        public virtual Branch? Branch { get; set; }
        
        /// <summary>
        /// Gets or sets the foods in this category
        /// </summary>
        public virtual ICollection<Food> Foods { get; set; } = new List<Food>();
    }
} 