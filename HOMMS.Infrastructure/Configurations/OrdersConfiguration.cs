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
    public class OrdersConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            // Table name
            builder.ToTable("Orders");

            // Primary key
            builder.HasKey(o => o.Id);

            // Properties
            builder.Property(o => o.BranchId).IsRequired();
            builder.Property(o => o.BranchUserId).IsRequired(false);
            builder.Property(o => o.OrderDate).IsRequired();
            builder.Property(o => o.ReceiveDate).IsRequired(false);
            builder.Property(o => o.ReceiveTime).HasMaxLength(10);
            builder.Property(o => o.ReceiveType).HasMaxLength(10);
            builder.Property(o => o.Type).HasMaxLength(20);
            builder.Property(o => o.Status).HasMaxLength(20);
            builder.Property(o => o.CustomerId).IsRequired(false);
            builder.Property(o => o.CustomerName).HasMaxLength(100);
            builder.Property(o => o.CustomerPhone).HasMaxLength(20);
            builder.Property(o => o.CustomerDob).HasMaxLength(10);
            builder.Property(o => o.CustomerAddress).HasMaxLength(255);
            builder.Property(o => o.HasVat).IsRequired(false);
            builder.Property(o => o.VatTaxCode).HasMaxLength(30);
            builder.Property(o => o.VatName).HasMaxLength(100);
            builder.Property(o => o.VatAddress).HasMaxLength(255);
            builder.Property(o => o.VatEmail).HasMaxLength(255);
            builder.Property(o => o.Total).IsRequired(false);
            builder.Property(o => o.ShippingFee).IsRequired(false);
            builder.Property(o => o.FoodTool).IsRequired(false);
            builder.Property(o => o.FoodToolFee).IsRequired(false);
            builder.Property(o => o.Printed).IsRequired(false);
            builder.Property(o => o.ConfirmedBy).IsRequired(false);
            builder.Property(o => o.TimeConfirmed).IsRequired(false);
            builder.Property(o => o.KitchenCompletionTime).IsRequired(false);
            builder.Property(o => o.DeliveryBy).IsRequired(false);
            builder.Property(o => o.DeliveryCompletion).IsRequired(false);
            builder.Property(o => o.Code).HasMaxLength(50);
            builder.Property(o => o.LocationId).IsRequired(false);
            builder.Property(o => o.Note).IsRequired(false);

            // Relationships
            builder.HasOne(o => o.Branch)
                   .WithMany()
                   .HasForeignKey(o => o.BranchId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(o => o.BranchUser)
                   .WithMany()
                   .HasForeignKey(o => o.BranchUserId)
                   .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(o => o.OrderDetails)
                   .WithOne()
                   .HasForeignKey(od => od.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Audit columns
            builder.Property(o => o.CreatedAt).IsRequired();
            builder.Property(o => o.CreatedBy).HasMaxLength(450).IsRequired(false);
            builder.Property(o => o.LastModifiedAt).IsRequired(false);
            builder.Property(o => o.LastModifiedBy).HasMaxLength(450).IsRequired(false);

            // Soft delete
            builder.Property(o => o.IsDeleted).HasDefaultValue(false);

            // Indexes
            builder.HasIndex(o => o.BranchId);
            builder.HasIndex(o => o.OrderDate);
            builder.HasIndex(o => o.Status);
        }
    }
}