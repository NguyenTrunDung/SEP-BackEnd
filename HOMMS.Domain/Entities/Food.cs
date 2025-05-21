using HOMMS.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HOMMS.Domain.Entities
{
    /// <summary>
    /// Represents a food item
    /// </summary>
    public class Food : BaseAuditableEntity<int>, IBranchEntity
    {
        /// <summary>
        /// Gets or sets the name of the food
        /// </summary>
        [Required]
        [StringLength(255)]
        public string Name { get; set; }
        
        /// <summary>
        /// Gets or sets the branch ID this food belongs to
        /// </summary>
        public int BranchId { get; set; }
        
        /// <summary>
        /// Gets or sets the category ID this food belongs to
        /// </summary>
        public int? CategoryId { get; set; }
        
        /// <summary>
        /// Gets or sets the description of the food
        /// </summary>
        [StringLength(255)]
        public string? Description { get; set; }
        
        /// <summary>
        /// Gets or sets whether this food is a set dish
        /// </summary>
        public bool IsSetDish { get; set; }
        
        /// <summary>
        /// Gets or sets whether this food is an add-on
        /// </summary>
        public bool IsAddOn { get; set; }
        
        /// <summary>
        /// Gets or sets whether this food is for patients
        /// </summary>
        public bool ForPatient { get; set; }
        
        /// <summary>
        /// Gets or sets the price for guests
        /// </summary>
        public int? PriceForGuest { get; set; }
        
        /// <summary>
        /// Gets or sets the price for patients
        /// </summary>
        public int? PriceForPatient { get; set; }
        
        /// <summary>
        /// Gets or sets the price for staff
        /// </summary>
        public int? PriceForStaff { get; set; }
        
        /// <summary>
        /// Gets or sets the disease category ID
        /// </summary>
        public int? DiseaseCategoryId { get; set; }
        
        /// <summary>
        /// Gets or sets the image URL for this food
        /// </summary>
        public string? Image { get; set; }
        
        /// <summary>
        /// Gets or sets the sort order for this food
        /// </summary>
        public int? Sort { get; set; }
        
        /// <summary>
        /// Gets or sets the branch navigation property
        /// </summary>
        public virtual Branch? Branch { get; set; }
        
        /// <summary>
        /// Gets or sets the category navigation property
        /// </summary>
        public virtual FoodCategory? Category { get; set; }
        
        /// <summary>
        /// Gets or sets the menu details for this food
        /// </summary>
        public virtual ICollection<MenuDetail> MenuDetails { get; set; } = new List<MenuDetail>();

        /// <summary>
        /// Gets the current price based on user type
        /// </summary>
        [NotMapped]
        public int? CurrentPrice => PriceForGuest;
    }
} 