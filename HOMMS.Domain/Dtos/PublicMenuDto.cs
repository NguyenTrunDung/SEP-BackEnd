using HOMMS.Domain.Entities;
using System;
using System.Collections.Generic;

namespace HOMMS.Domain.Dtos
{
    /// <summary>
    /// DTO for displaying branch information
    /// </summary>
    public class BranchDto
    {
        /// <summary>
        /// Gets or sets the branch ID
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Gets or sets the branch name
        /// </summary>
        public string? Name { get; set; }
        
        /// <summary>
        /// Gets or sets the branch code
        /// </summary>
        public string? Code { get; set; }
        
        /// <summary>
        /// Gets or sets the branch address
        /// </summary>
        public string? Address { get; set; }
        
        /// <summary>
        /// Gets or sets the branch phone
        /// </summary>
        public string? Phone { get; set; }
        
        /// <summary>
        /// Gets or sets the branch email
        /// </summary>
        public string? Email { get; set; }
    }
    
    /// <summary>
    /// DTO for displaying menu information
    /// </summary>
    public class MenuDto
    {
        /// <summary>
        /// Gets or sets the menu ID
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Gets or sets the menu name
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Gets or sets the menu date
        /// </summary>
        public DateTime Date { get; set; }
        
        /// <summary>
        /// Gets or sets the time of day (e.g., "Breakfast", "Lunch", "Dinner")
        /// </summary>
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
        /// Gets or sets the branch ID
        /// </summary>
        public int BranchId { get; set; }
        
        /// <summary>
        /// Gets or sets the branch information
        /// </summary>
        public BranchDto? Branch { get; set; }
        
        /// <summary>
        /// Gets or sets the menu details
        /// </summary>
        public List<MenuDetailDto>? MenuDetails { get; set; } = new List<MenuDetailDto>();
        
        /// <summary>
        /// Gets or sets the foods by category
        /// </summary>
        public Dictionary<FoodCategoryDto, List<MenuDetailDto>>? FoodsByCategory { get; set; } = new Dictionary<FoodCategoryDto, List<MenuDetailDto>>();

    
    }
    
    /// <summary>
    /// DTO for displaying all menus for all branches
    /// </summary>
    public class AllBranchesMenusDto
    {
        /// <summary>
        /// Gets or sets the date
        /// </summary>
        public DateTime Date { get; set; }
        
        /// <summary>
        /// Gets or sets the branches with their menus
        /// </summary>
        public List<BranchMenusDto> Branches { get; set; } = new List<BranchMenusDto>();
    }
    
    /// <summary>
    /// DTO for displaying a branch with its menus
    /// </summary>
    public class BranchMenusDto
    {
        /// <summary>
        /// Gets or sets the branch information
        /// </summary>
        public BranchDto Branch { get; set; }
        
        /// <summary>
        /// Gets or sets the menus for this branch
        /// </summary>
        public List<MenuDto>? Menus { get; set; } = new List<MenuDto>();
    }
} 