using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HOMMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HOMMS.Infrastructure.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");

            builder.HasKey(o => o.Id);

            builder.Property(o => o.OrderDate)
                .IsRequired();

            builder.Property(o => o.Status)
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(o => o.CustomerName)
                .HasMaxLength(100);

            builder.Property(o => o.CustomerPhone)
                .HasMaxLength(20);

            builder.Property(o => o.Code)
                .HasMaxLength(50);

            builder.HasOne(o => o.Branch)
                .WithMany()
                .HasForeignKey(o => o.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(o => o.BranchUser)
                .WithMany()
                .HasForeignKey(o => o.BranchUserId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(o => o.OrderDetails)
                .WithOne(od => od.Order)
                .HasForeignKey(od => od.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(o => new { o.BranchId, o.Status });
        }
    }
}
