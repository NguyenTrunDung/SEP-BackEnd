using HOMMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HOMMS.Infrastructure.Configurations
{
    /// <summary>
    /// Entity Framework configuration for Area entity
    /// </summary>
    public class AreaConfiguration : IEntityTypeConfiguration<Area>
    {
        public void Configure(EntityTypeBuilder<Area> builder)
        {
            // Table name
            builder.ToTable("Areas");
            
            // Primary key
            builder.HasKey(a => a.Id);
            
            // Properties
            builder.Property(a => a.Name)
                   .IsRequired()
                   .HasMaxLength(100);
                   
            builder.Property(a => a.Description)
                   .HasMaxLength(500)
                   .IsRequired(false);
                   
            builder.Property(a => a.Sort)
                   .IsRequired(false);
                   
            builder.Property(a => a.IsActive)
                   .IsRequired()
                   .HasDefaultValue(true);
                   
            // Relationships
            builder.HasOne(a => a.Branch)
                   .WithMany() // Branch doesn't have Areas navigation property yet
                   .HasForeignKey(a => a.BranchId)
                   .OnDelete(DeleteBehavior.Restrict);
                   
            builder.HasMany(a => a.Locations)
                   .WithOne(l => l.Area)
                   .HasForeignKey(l => l.AreaId)
                   .OnDelete(DeleteBehavior.Restrict);
                   
            // Indexes
            builder.HasIndex(a => new { a.BranchId, a.Name })
                   .IsUnique();
            builder.HasIndex(a => a.BranchId);
            builder.HasIndex(a => new { a.BranchId, a.Sort });
            
            // Audit columns
            builder.Property(a => a.CreatedAt)
                   .IsRequired();
                   
            builder.Property(a => a.CreatedBy)
                   .HasMaxLength(450)
                   .IsRequired(false);
                   
            builder.Property(a => a.LastModifiedAt)
                   .IsRequired(false);
                   
            builder.Property(a => a.LastModifiedBy)
                   .HasMaxLength(450)
                   .IsRequired(false);
                   
            // Soft delete
            builder.Property(a => a.IsDeleted)
                   .IsRequired()
                   .HasDefaultValue(false);
                   
            builder.Property(a => a.DeletedAt)
                   .IsRequired(false);
                   
            builder.Property(a => a.DeletedBy)
                   .HasMaxLength(450)
                   .IsRequired(false);
        }
    }
} 