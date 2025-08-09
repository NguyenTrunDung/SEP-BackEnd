using HOMMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HOMMS.Infrastructure.Configurations
{
    /// <summary>
    /// Entity Framework configuration for Branch entity
    /// </summary>
    public class BranchConfiguration : IEntityTypeConfiguration<Branch>
    {
        public void Configure(EntityTypeBuilder<Branch> builder)
        {
            // Table name
            builder.ToTable("Branches");
            
            // Primary key
            builder.HasKey(b => b.Id);
            
            // Properties
            builder.Property(b => b.Name)
                   .IsRequired()
                   .HasMaxLength(100);
                   
            builder.Property(b => b.Code)
                   .HasMaxLength(20);
                   
            builder.Property(b => b.Address)
                   .IsRequired()
                   .HasMaxLength(200);
                   
            builder.Property(b => b.Phone)
                   .IsRequired()
                   .HasMaxLength(20);
                   
            builder.Property(b => b.Email)
                   .HasMaxLength(100);
                   
            builder.Property(b => b.Description)
                   .HasMaxLength(500);
                   
            // Only enforce uniqueness for Name (not Code since it's now optional)
            builder.HasIndex(b => b.Name)
                   .IsUnique()
                   .HasFilter("[IsDeleted] = 0");
                   
            // Relationships
            builder.HasMany(b => b.BranchUsers)
                   .WithOne(bu => bu.Branch)
                   .HasForeignKey(bu => bu.BranchId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Query filter for soft delete
            builder.HasQueryFilter(b => !b.IsDeleted);
        }
    }
} 