using HOMMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Infrastructure.Configurations
{
    public class OrderDetailConfiguration : IEntityTypeConfiguration<OrderDetail>
    {
        public void Configure(EntityTypeBuilder<OrderDetail> builder)
        {
            builder.ToTable("OrderDetails");

            builder.HasKey(od => od.Id);

            builder.Property(od => od.OrderId)
                .IsRequired();

            builder.Property(od => od.FoodId)
                .IsRequired(false);

            builder.Property(od => od.MenuId)
                .IsRequired(false);

            builder.Property(od => od.Qty)
                .IsRequired();

            builder.Property(od => od.Price)
                .HasPrecision(18, 2)
                .IsRequired(false);

            builder.Property(od => od.Total)
                .HasPrecision(18, 2)
                .IsRequired(false);

            builder.Property(od => od.Note)
                .HasMaxLength(255);

            builder.HasOne(od => od.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(od => od.Food)
                .WithMany()
                .HasForeignKey(od => od.FoodId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(od => od.Menu)
                .WithMany()
                .HasForeignKey(od => od.MenuId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
