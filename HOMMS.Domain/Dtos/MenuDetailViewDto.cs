using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Domain.Dtos
{
    public class MenuDetailViewDto
    {
        public int Id { get; set; }
        public bool IsTime { get; set; }
        public TimeSpan? TimeFrom { get; set; }
        public TimeSpan? TimeTo { get; set; }
        public DateTime Date { get; set; }
        public int BranchId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public string? TimeOfDay { get; set; }
        public string? Name { get; set; }

        public List<MenuDetailsDto> Details { get; set; } = new();
    }
    public class UpdateMenuDto
    {
        public int Id { get; set; }   // ID của Menu
        public DateTime Date { get; set; }
        public string? TimeOfDay { get; set; }
        public bool IsTime { get; set; }
        public TimeSpan? TimeFrom { get; set; }
        public TimeSpan? TimeTo { get; set; }
        public string? Name { get; set; }
        public int BranchId { get; set; } // ID của chi nhánh

        public List<MenuDetailsDto> Details { get; set; } = new(); // update toàn bộ danh sách
    }
    public class CreateMenuDto
    {
        public int Id { get; set; }   // ID của Menu
        public DateTime Date { get; set; }
        public string? TimeOfDay { get; set; }
        public bool IsTime { get; set; }
        public TimeSpan? TimeFrom { get; set; }
        public TimeSpan? TimeTo { get; set; }
        public string? Name { get; set; }
        public string? CreatedBy { get; set; }

        public int BranchId { get; set; } // ID của chi nhánh

        public List<MenuDetailsDto> Details { get; set; } = new(); // update toàn bộ danh sách
    }

    /// <summary>
    /// DTO for menu template selection (simplified version for quick selection)
    /// </summary>
    public class MenuTemplateDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string? TimeOfDay { get; set; }
        public bool IsTime { get; set; }
        public TimeSpan? TimeFrom { get; set; }
        public TimeSpan? TimeTo { get; set; }
        public int TotalDishes { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public int BranchId { get; set; }
        
        /// <summary>
        /// Summary of categories and dish counts for preview
        /// </summary>
        public List<CategorySummaryDto> CategorySummary { get; set; } = new();
    }

    /// <summary>
    /// Summary of a category with dish count for template preview
    /// </summary>
    public class CategorySummaryDto
    {
        public int? CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int DishCount { get; set; }
    }

    /// <summary>
    /// DTO for copy menu request
    /// </summary>
    public class CopyMenuRequestDto
    {
        public DateTime NewDate { get; set; }
        public string? NewName { get; set; }
    }
}
