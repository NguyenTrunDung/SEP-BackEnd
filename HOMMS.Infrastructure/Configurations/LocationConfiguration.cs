using HOMMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HOMMS.Infrastructure.Configurations
{
    /// <summary>
    /// Entity Framework configuration for Location entity
    /// </summary>
    public class LocationConfiguration : IEntityTypeConfiguration<Location>
    {
        public void Configure(EntityTypeBuilder<Location> builder)
        {
            // Table name
            builder.ToTable("Locations");
            
            // Primary key
            builder.HasKey(l => l.Id);
            
            // Properties
            builder.Property(l => l.Name)
                   .IsRequired()
                   .HasMaxLength(100);
                   
            builder.Property(l => l.Description)
                   .HasMaxLength(500)
                   .IsRequired(false);
                   
            builder.Property(l => l.RoomNumber)
                   .HasMaxLength(20)
                   .IsRequired(false);
                   
            builder.Property(l => l.Capacity)
                   .IsRequired(false);
                   
            builder.Property(l => l.Sort)
                   .IsRequired(false);
                   
            builder.Property(l => l.IsActive)
                   .IsRequired()
                   .HasDefaultValue(true);
                   
            // Relationships
            builder.HasOne(l => l.Area)
                   .WithMany(a => a.Locations)
                   .HasForeignKey(l => l.AreaId)
                   .OnDelete(DeleteBehavior.Restrict);
                   
            builder.HasOne(l => l.Branch)
                   .WithMany() // Branch doesn't have Locations navigation property yet
                   .HasForeignKey(l => l.BranchId)
                   .OnDelete(DeleteBehavior.Restrict);
                   
            // Indexes
            builder.HasIndex(l => new { l.AreaId, l.Name })
                   .IsUnique();
            builder.HasIndex(l => l.AreaId);
            builder.HasIndex(l => l.BranchId);
            builder.HasIndex(l => new { l.AreaId, l.Sort });
            builder.HasIndex(l => new { l.BranchId, l.RoomNumber })
                   .IsUnique()
                   .HasFilter("[RoomNumber] IS NOT NULL");
            
            // Audit columns
            builder.Property(l => l.CreatedAt)
                   .IsRequired();
                   
            builder.Property(l => l.CreatedBy)
                   .HasMaxLength(450)
                   .IsRequired(false);
                   
            builder.Property(l => l.LastModifiedAt)
                   .IsRequired(false);
                   
            builder.Property(l => l.LastModifiedBy)
                   .HasMaxLength(450)
                   .IsRequired(false);
                   
            // Soft delete
            builder.Property(l => l.IsDeleted)
                   .IsRequired()
                   .HasDefaultValue(false);
                   
            builder.Property(l => l.DeletedAt)
                   .IsRequired(false);
                   
            builder.Property(l => l.DeletedBy)
                   .HasMaxLength(450)
                   .IsRequired(false);
        }
    }
} 