using HOMMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HOMMS.Infrastructure.Configurations
{
    /// <summary>
    /// Entity Framework configuration for BranchUser junction entity
    /// </summary>
    public class BranchUserConfiguration : IEntityTypeConfiguration<BranchUser>
    {
        public void Configure(EntityTypeBuilder<BranchUser> builder)
        {
            // Table name
            builder.ToTable("BranchUsers");
            
            // Primary key
            builder.HasKey(bu => bu.Id);
            
            // Alternate composite key
            builder.HasAlternateKey(bu => new { bu.BranchId, bu.UserId });
            
            // Relationships
            builder.HasOne(bu => bu.Branch)
                   .WithMany(b => b.BranchUsers)
                   .HasForeignKey(bu => bu.BranchId)
                   .OnDelete(DeleteBehavior.Cascade);
                   
            builder.HasOne(bu => bu.User)
                   .WithMany(u => u.BranchUsers)
                   .HasForeignKey(bu => bu.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
                   
            // Indexes
            builder.HasIndex(bu => bu.UserId);
            builder.HasIndex(bu => new { bu.UserId, bu.IsDefault });
        }
    }
} 