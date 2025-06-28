using Microsoft.AspNetCore.Http;
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
        /// Sort order for displaying categories
        /// </summary>
        public int Sort { get; set; } = 0;

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
} 