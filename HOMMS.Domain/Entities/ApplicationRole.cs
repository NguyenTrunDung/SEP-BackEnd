using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace HOMMS.Domain.Entities
{
    public class ApplicationRole : IdentityRole
    {
        [StringLength(200)]
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? LastModifiedAt { get; set; }

        public bool IsActive { get; set; } = true;

        // You can add additional role-specific properties here
    }
} 