using HOMMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HOMMS.Infrastructure.Configurations
{
    /// <summary>
    /// Entity Framework configuration for FoodCategory entity
    /// </summary>
    public class FoodCategoryConfiguration : IEntityTypeConfiguration<FoodCategory>
    {
        public void Configure(EntityTypeBuilder<FoodCategory> builder)
        {
            // Table name
            builder.ToTable("Food_Categories");
            
            // Primary key
            builder.HasKey(c => c.Id);
            
            // Properties
            builder.Property(c => c.Name)
                   .IsRequired()
                   .HasMaxLength(255);
                   
            builder.Property(c => c.Image)
                   .HasMaxLength(255);
                   
            builder.Property(c => c.Sort)
                   .IsRequired(false);
                   
            builder.Property(c => c.Active)
                   .HasDefaultValue(true);
                   
            // Relationships
            builder.HasOne(c => c.Branch)
                   .WithMany(b => b.FoodCategories)
                   .HasForeignKey(c => c.BranchId)
                   .OnDelete(DeleteBehavior.Restrict);
                   
            builder.HasMany(c => c.Foods)
                   .WithOne(f => f.Category)
                   .HasForeignKey(f => f.CategoryId)
                   .IsRequired(false)
                   .OnDelete(DeleteBehavior.SetNull);
                   
            // Indexes
            builder.HasIndex(c => new { c.BranchId, c.Name });
            builder.HasIndex(c => c.BranchId);
            
            // Audit columns
            builder.Property(c => c.CreatedAt)
                   .IsRequired();
                   
            builder.Property(c => c.CreatedBy)
                   .HasMaxLength(450)
                   .IsRequired(false);
                   
            builder.Property(c => c.LastModifiedAt)
                   .IsRequired(false);
                   
            builder.Property(c => c.LastModifiedBy)
                   .HasMaxLength(450)
                   .IsRequired(false);
                   
            // Soft delete
            builder.Property(c => c.IsDeleted)
                   .HasDefaultValue(false);
        }
    }
}