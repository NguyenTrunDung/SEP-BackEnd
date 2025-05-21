using HOMMS.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HOMMS.Domain.Entities
{
    /// <summary>
    /// Represents a menu for a specific branch on a specific day
    /// </summary>
    public class Menu : BaseAuditableEntity<int>, IBranchEntity
    {
        /// <summary>
        /// Gets or sets the branch ID this menu belongs to
        /// </summary>
        public int BranchId { get; set; }
        
        /// <summary>
        /// Gets or sets the date this menu is available
        /// </summary>
        [Required]
        public DateTime Date { get; set; }
        
        /// <summary>
        /// Gets or sets the time of day (e.g., "Breakfast", "Lunch", "Dinner")
        /// </summary>
        [StringLength(20)]
        public string? TimeOfDay { get; set; }
        
        /// <summary>
        /// Gets or sets whether this menu has time restrictions
        /// </summary>
        public bool IsTime { get; set; }
        
        /// <summary>
        /// Gets or sets the time from which this menu is available
        /// </summary>
        public TimeSpan? TimeFrom { get; set; }
        
        /// <summary>
        /// Gets or sets the time until which this menu is available
        /// </summary>
        public TimeSpan? TimeTo { get; set; }
        
        /// <summary>
        /// Gets or sets the branch navigation property
        /// </summary>
        public virtual Branch? Branch { get; set; }
        
        /// <summary>
        /// Gets or sets the menu details in this menu
        /// </summary>
        public virtual ICollection<MenuDetail> MenuDetails { get; set; } = new List<MenuDetail>();
        
        /// <summary>
        /// Gets or sets the name of the menu (computed by the database)
        /// </summary>
        public string? Name { get; set; }
    }
} 