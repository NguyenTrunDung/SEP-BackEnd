using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HOMMS.Common.Helpers
{
    /// <summary>
    /// Request model for creating and updating food categories with image upload support
    /// </summary>
    public class FoodCategoryCreateRequest
    {
        /// <summary>
        /// Category name (required)
        /// </summary>
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Optional image URL (for cases where image is already uploaded)
        /// </summary>
        public string? ImageUrl { get; set; }

        /// <summary>
        /// Sort order for displaying categories (optional - will be auto-assigned if not provided)
        /// </summary>
        public int? Sort { get; set; }

        /// <summary>
        /// Branch ID this category belongs to
        /// </summary>
        [Required]
        public int BranchId { get; set; }

        /// <summary>
        /// Image file for upload (optional)
        /// </summary>
        public IFormFile? Image { get; set; }
    }

    /// <summary>
    /// Request model for reordering food categories
    /// </summary>
    public class ReorderFoodCategoriesRequest
    {
        /// <summary>
        /// List of category IDs with their new sort orders
        /// </summary>
        [Required]
        public List<CategoryOrderItem> CategoryOrders { get; set; } = new();

        /// <summary>
        /// Branch ID for validation
        /// </summary>
        [Required]
        public int BranchId { get; set; }
    }

    /// <summary>
    /// Represents a category with its new sort order
    /// </summary>
    public class CategoryOrderItem
    {
        /// <summary>
        /// Category ID
        /// </summary>
        [Required]
        public int CategoryId { get; set; }

        /// <summary>
        /// New sort order (0-based index)
        /// </summary>
        [Required]
        public int Sort { get; set; }
    }

    /// <summary>
    /// Request model for moving a single category to a specific position
    /// </summary>
    public class MoveFoodCategoryRequest
    {
        /// <summary>
        /// Category ID to move
        /// </summary>
        [Required]
        public int CategoryId { get; set; }

        /// <summary>
        /// New position (0-based index)
        /// </summary>
        [Required]
        public int NewPosition { get; set; }

        /// <summary>
        /// Branch ID for validation
        /// </summary>
        [Required]
        public int BranchId { get; set; }
    }
} 