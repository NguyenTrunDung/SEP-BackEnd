using HOMMS.Domain.Entities.Base;
using System;
using System.ComponentModel.DataAnnotations;

namespace HOMMS.Domain.Entities
{
    /// <summary>
    /// Represents a menu detail (relationship between menu and food)
    /// </summary>
    public class MenuDetail : BaseAuditableEntity<int>
    {
        /// <summary>
        /// Gets or sets the menu ID this detail belongs to
        /// </summary>
        public int MenuId { get; set; }
        
        /// <summary>
        /// Gets or sets the food ID for this detail
        /// </summary>
        public int FoodId { get; set; }
        
        /// <summary>
        /// Gets or sets the quantity available
        /// </summary>
        public int? Qty { get; set; }
        
        /// <summary>
        /// Gets or sets the quantity sold
        /// </summary>
        public int? Sold { get; set; }
        
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
        /// Gets or sets the discount price
        /// </summary>
        public int? DiscountPrice { get; set; }
        
        /// <summary>
        /// Gets or sets the status of this detail
        /// </summary>
        public bool? Status { get; set; }
        
        /// <summary>
        /// Gets or sets the discount from date/time
        /// </summary>
        [StringLength(10)]
        public string? DiscountFrom { get; set; }
        
        /// <summary>
        /// Gets or sets the discount to date/time
        /// </summary>
        [StringLength(10)]
        public string? DiscountTo { get; set; }
        
        /// <summary>
        /// Gets or sets whether quantity is limited
        /// </summary>
        public bool? IsQty { get; set; }
        
        /// <summary>
        /// Gets or sets the menu navigation property
        /// </summary>
        public virtual Menu? Menu { get; set; }
        
        /// <summary>
        /// Gets or sets the food navigation property
        /// </summary>
        public virtual Food? Food { get; set; }
    }
} 