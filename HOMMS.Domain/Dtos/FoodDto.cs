using System;
using System.Collections.Generic;

namespace HOMMS.Domain.Dtos
{
    /// <summary>
    /// DTO for displaying food category information
    /// </summary>
    public class FoodCategoryDto
    {
        /// <summary>
        /// Gets or sets the category ID
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Gets or sets the category name
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Gets or sets the image URL for this category
        /// </summary>
        public string ImageUrl { get; set; }
        
        /// <summary>
        /// Gets or sets the sort order for this category
        /// </summary>
        public int Sort { get; set; }
    }
    
    /// <summary>
    /// DTO for displaying food information
    /// </summary>
    public class FoodDto
    {
        /// <summary>
        /// Gets or sets the food ID
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Gets or sets the food name
        /// </summary>
        public string? Name { get; set; }
        
        /// <summary>
        /// Gets or sets the food description
        /// </summary>
        public string? Description { get; set; }
        
        /// <summary>
        /// Gets or sets the category ID
        /// </summary>
        public int? CategoryId { get; set; }
        
        /// <summary>
        /// Gets or sets the food category
        /// </summary>
        public FoodCategoryDto? Category { get; set; }
        
        /// <summary>
        /// Gets or sets the image URL for this food
        /// </summary>
        public string? ImageUrl { get; set; }
        
        /// <summary>
        /// Gets or sets whether this food is a set dish
        /// </summary>
        public bool IsSetDish { get; set; }
        
        /// <summary>
        /// Gets or sets whether this food is an add-on
        /// </summary>
        public bool IsAddOn { get; set; }
        
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
        /// Gets or sets the sort order for this food
        /// </summary>
        public int? Sort { get; set; }
    }
    
    /// <summary>
    /// DTO for displaying menu detail information
    /// </summary>
    public class MenuDetailDto
    {
        /// <summary>
        /// Gets or sets the menu detail ID
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Gets or sets the menu ID
        /// </summary>
        public int MenuId { get; set; }
        
        /// <summary>
        /// Gets or sets the food ID
        /// </summary>
        public int FoodId { get; set; }
        
        /// <summary>
        /// Gets or sets the food information
        /// </summary>
        public FoodDto? Food { get; set; }
        
        /// <summary>
        /// Gets or sets the quantity available
        /// </summary>
        public int? Quantity { get; set; }
        
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
        /// Gets or sets whether this food is available
        /// </summary>
        public bool IsAvailable => Status == true && (Quantity == null || (Quantity > Sold));
    }

   
} 