using HOMMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HOMMS.Infrastructure.Configurations
{
    /// <summary>
    /// Entity Framework configuration for Menu entity
    /// </summary>
    public class MenuConfiguration : IEntityTypeConfiguration<Menu>
    {
        public void Configure(EntityTypeBuilder<Menu> builder)
        {
            // Table name
            builder.ToTable("Menus");
            
            // Primary key
            builder.HasKey(m => m.Id);
            
            // Properties
            builder.Property(m => m.Date)
                   .IsRequired();
                   
            builder.Property(m => m.TimeOfDay)
                   .HasMaxLength(20)
                   .IsRequired(false);
                   
            builder.Property(m => m.IsTime)
                   .HasDefaultValue(false);
                   
            builder.Property(m => m.TimeFrom)
                   .IsRequired(false);
                   
            builder.Property(m => m.TimeTo)
                   .IsRequired(false);
                   
            // Computed columns
            builder.Property(m => m.Name)
                   .HasComputedColumnSql("CONCAT([TimeOfDay], ' - ', CONVERT(VARCHAR(10), [Date], 120))")
                   .IsRequired(false);
                   
            // Relationships
            builder.HasOne(m => m.Branch)
                   .WithMany(b => b.Menus)
                   .HasForeignKey(m => m.BranchId)
                   .OnDelete(DeleteBehavior.Restrict);
                   
            // Indexes
            builder.HasIndex(m => new { m.BranchId, m.Date });
            builder.HasIndex(m => m.BranchId);
            builder.HasIndex(m => m.Date);
            
            // Audit columns
            builder.Property(m => m.CreatedAt)
                   .IsRequired();
                   
            builder.Property(m => m.CreatedBy)
                   .HasMaxLength(450)
                   .IsRequired(false);
                   
            builder.Property(m => m.LastModifiedAt)
                   .IsRequired(false);
                   
            builder.Property(m => m.LastModifiedBy)
                   .HasMaxLength(450)
                   .IsRequired(false);
                   
            // Soft delete
            builder.Property(m => m.IsDeleted)
                   .HasDefaultValue(false);
        }
    }
} 