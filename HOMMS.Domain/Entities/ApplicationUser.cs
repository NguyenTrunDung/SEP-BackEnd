using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace HOMMS.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Address { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? LastModifiedAt { get; set; }

        public bool IsActive { get; set; } = true;

        [StringLength(200)]
        public string? ProfilePictureUrl { get; set; }

        // Navigation properties can be added here if needed
        // For example:
        // public virtual ICollection<Order> Orders { get; set; } = new HashSet<Order>();
    }
} 