using HOMMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HOMMS.Infrastructure.Persistence.Configurations
{
    public class OrderDetailsConfiguration : IEntityTypeConfiguration<OrderDetails>
    {
        public void Configure(EntityTypeBuilder<OrderDetails> builder)
        {
            // Table name
            builder.ToTable("OrderDetails");

            // Primary Key
            builder.HasKey(x => x.Id);

            // Relationships
            builder.HasOne(x => x.Order)
                   .WithMany(o => o.OrderDetails)
                   .HasForeignKey(x => x.OrderId)
                   .OnDelete(DeleteBehavior.Cascade)
                   .IsRequired(false); // <-- thêm dòng này
            // When order is deleted, delete its details

            builder.HasOne(x => x.Food)
                   .WithMany()
                   .HasForeignKey(x => x.FoodId)
                   .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

            builder.HasOne(x => x.Menu)
                   .WithMany()
                   .HasForeignKey(x => x.MenuId)
                   .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

            // Property configurations
            builder.Property(x => x.Qty)
                   .HasColumnType("int");

            builder.Property(x => x.Price)
                   .HasColumnType("int");

            builder.Property(x => x.Total)
                   .HasColumnType("int");

            builder.Property(x => x.Note)
                   .HasMaxLength(500);
        }
    }
}
