using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Domain.Dtos
{
    public class BranchDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Code { get; set; }
        public string Address { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public string? ManagerName { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// DTO for creating a new branch
    /// </summary>
    public class CreateBranchDto
    {
        [Required(ErrorMessage = "Branch name is required")]
        [StringLength(100, ErrorMessage = "Branch name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "Branch code cannot exceed 20 characters")]
        public string? Code { get; set; }

        [Required(ErrorMessage = "Branch address is required")]
        [StringLength(200, ErrorMessage = "Branch address cannot exceed 200 characters")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required")]
        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
        public string Phone { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string? Email { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public string? ManagerId { get; set; }
    }

    /// <summary>
    /// DTO for updating an existing branch
    /// </summary>
    public class UpdateBranchDto
    {
        [Required(ErrorMessage = "Branch name is required")]
        [StringLength(100, ErrorMessage = "Branch name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "Branch code cannot exceed 20 characters")]
        public string? Code { get; set; }

        [Required(ErrorMessage = "Branch address is required")]
        [StringLength(200, ErrorMessage = "Branch address cannot exceed 200 characters")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required")]
        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
        public string Phone { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string? Email { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        public bool IsActive { get; set; }

        public string? ManagerId { get; set; }
    }
}
