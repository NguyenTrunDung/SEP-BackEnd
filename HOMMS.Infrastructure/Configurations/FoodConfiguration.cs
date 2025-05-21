using HOMMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HOMMS.Infrastructure.Configurations
{
    /// <summary>
    /// Entity Framework configuration for Food entity
    /// </summary>
    public class FoodConfiguration : IEntityTypeConfiguration<Food>
    {
        public void Configure(EntityTypeBuilder<Food> builder)
        {
            // Table name
            builder.ToTable("Foods");
            
            // Primary key
            builder.HasKey(f => f.Id);
            
            // Properties
            builder.Property(f => f.Name)
                   .IsRequired()
                   .HasMaxLength(255);
                   
            builder.Property(f => f.Description)
                   .HasMaxLength(255)
                   .IsRequired(false);
                   
            builder.Property(f => f.Image)
                   .HasMaxLength(255)
                   .IsRequired(false);
                   
            builder.Property(f => f.IsSetDish)
                   .HasDefaultValue(false);
                   
            builder.Property(f => f.IsAddOn)
                   .HasDefaultValue(false);
                   
            builder.Property(f => f.ForPatient)
                   .HasDefaultValue(false);
                   
            builder.Property(f => f.PriceForGuest)
                   .IsRequired(false);
                   
            builder.Property(f => f.PriceForPatient)
                   .IsRequired(false);
                   
            builder.Property(f => f.PriceForStaff)
                   .IsRequired(false);
                   
            builder.Property(f => f.DiseaseCategoryId)
                   .IsRequired(false);
                   
            builder.Property(f => f.Sort)
                   .IsRequired(false);
                   
            // Relationships
            builder.HasOne(f => f.Branch)
                   .WithMany(b => b.Foods)
                   .HasForeignKey(f => f.BranchId)
                   .OnDelete(DeleteBehavior.Restrict);
                   
            builder.HasOne(f => f.Category)
                   .WithMany(c => c.Foods)
                   .HasForeignKey(f => f.CategoryId)
                   .IsRequired(false)
                   .OnDelete(DeleteBehavior.SetNull);
                   
            // Ignore calculated property
            builder.Ignore(f => f.CurrentPrice);
                   
            // Indexes
            builder.HasIndex(f => new { f.BranchId, f.Name });
            builder.HasIndex(f => f.BranchId);
            builder.HasIndex(f => f.CategoryId);
            
            // Audit columns
            builder.Property(f => f.CreatedAt)
                   .IsRequired();
                   
            builder.Property(f => f.CreatedBy)
                   .HasMaxLength(450)
                   .IsRequired(false);
                   
            builder.Property(f => f.LastModifiedAt)
                   .IsRequired(false);
                   
            builder.Property(f => f.LastModifiedBy)
                   .HasMaxLength(450)
                   .IsRequired(false);
                   
            // Soft delete
            builder.Property(f => f.IsDeleted)
                   .HasDefaultValue(false);
        }
    }
} 