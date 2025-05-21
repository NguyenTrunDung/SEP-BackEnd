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
                   .IsRequired()
                   .HasMaxLength(20);
                   
            builder.Property(b => b.Address)
                   .HasMaxLength(250);
                   
            builder.Property(b => b.Phone)
                   .HasMaxLength(20);
                   
            builder.Property(b => b.Email)
                   .HasMaxLength(100);
                   
            // Indexes
            builder.HasIndex(b => b.Code)
                   .IsUnique();
                   
            // Relationships
            builder.HasMany(b => b.BranchUsers)
                   .WithOne(bu => bu.Branch)
                   .HasForeignKey(bu => bu.BranchId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Seed data
            builder.HasData(
                new Branch
                {
                    Id = 5,
                    Name = "Coteccons",
                    Code = "coteccons",
                    Address = "Coteccons",
                    Phone = "0919000000",
                    IsActive = true,
                    BranchId = 5,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Branch
                {
                    Id = 12,
                    Name = "Bệnh viện Hoàn Mỹ Cửu Long Canteen",
                    Code = "cthoanmy",
                    Address = "Cần Thơ",
                    Phone = "0123456789",
                    IsActive = true,
                    BranchId = 12,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Branch
                {
                    Id = 13,
                    Name = "BV Nhi Đồng Canteen",
                    Code = "bvnhi",
                    Address = "TP Hồ Chí Minh",
                    Phone = "0123456789",
                    IsActive = true,
                    BranchId = 13,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Branch
                {
                    Id = 14,
                    Name = "Becamex",
                    Code = "becamex",
                    Address = "Đại lộ Bình Dương, khu Gò Cát, Lái Thiêu, Thuận An, Bình Dương",
                    Phone = "0919111111",
                    IsActive = true,
                    BranchId = 14,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );
        }
    }
} 