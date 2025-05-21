using HOMMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HOMMS.Infrastructure.Configurations
{
    /// <summary>
    /// Entity Framework configuration for MenuDetail entity
    /// </summary>
    public class MenuDetailConfiguration : IEntityTypeConfiguration<MenuDetail>
    {
        public void Configure(EntityTypeBuilder<MenuDetail> builder)
        {
            // Table name
            builder.ToTable("Menu_Details");
            
            // Primary key
            builder.HasKey(md => md.Id);
            
            // Properties
            builder.Property(md => md.Qty)
                   .IsRequired(false);
                   
            builder.Property(md => md.Sold)
                   .IsRequired(false);
                   
            builder.Property(md => md.PriceForGuest)
                   .IsRequired(false);
                   
            builder.Property(md => md.PriceForPatient)
                   .IsRequired(false);
                   
            builder.Property(md => md.PriceForStaff)
                   .IsRequired(false);
                   
            builder.Property(md => md.DiscountPrice)
                   .IsRequired(false);
                   
            builder.Property(md => md.Status)
                   .HasDefaultValue(true)
                   .IsRequired(false);
                   
            builder.Property(md => md.DiscountFrom)
                   .HasMaxLength(10)
                   .IsRequired(false);
                   
            builder.Property(md => md.DiscountTo)
                   .HasMaxLength(10)
                   .IsRequired(false);
                   
            builder.Property(md => md.IsQty)
                   .HasDefaultValue(false)
                   .IsRequired(false);
                   
            // Relationships
            builder.HasOne(md => md.Menu)
                   .WithMany(m => m.MenuDetails)
                   .HasForeignKey(md => md.MenuId)
                   .IsRequired(false)
                   .OnDelete(DeleteBehavior.Cascade);
                   
            builder.HasOne(md => md.Food)
                   .WithMany(f => f.MenuDetails)
                   .HasForeignKey(md => md.FoodId)
                   .IsRequired(false)
                   .OnDelete(DeleteBehavior.Cascade);
                   
            // Indexes
            builder.HasIndex(md => md.MenuId);
            builder.HasIndex(md => md.FoodId);
            builder.HasIndex(md => new { md.MenuId, md.FoodId }).IsUnique();
            
            // Audit columns
            builder.Property(md => md.CreatedAt)
                   .IsRequired();
                   
            builder.Property(md => md.CreatedBy)
                   .HasMaxLength(450)
                   .IsRequired(false);
                   
            builder.Property(md => md.LastModifiedAt)
                   .IsRequired(false);
                   
            builder.Property(md => md.LastModifiedBy)
                   .HasMaxLength(450)
                   .IsRequired(false);
                   
            // Soft delete
            builder.Property(md => md.IsDeleted)
                   .HasDefaultValue(false);
        }
    }
} 