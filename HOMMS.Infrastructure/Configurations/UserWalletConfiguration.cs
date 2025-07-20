using HOMMS.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Infrastructure.Configurations
{
    public class UserWalletConfiguration : IEntityTypeConfiguration<UserWallet>
    {
        public void Configure(EntityTypeBuilder<UserWallet> builder)
        {
            // Table name
            builder.ToTable("UserWallets");

            // Primary key
            builder.HasKey(w => w.Id);

            // Properties
            builder.Property(w => w.UserId)
                .IsRequired()
                .HasMaxLength(450); // Match AspNetUsers Id length

            builder.Property(w => w.Amount)
                .IsRequired()
                .HasColumnType("decimal(18,0)"); // VND stored as decimal

            // Relationships
            builder.HasOne(w => w.User)
     .WithOne() // Nếu không muốn tạo navigation ngược
     .HasForeignKey<UserWallet>(w => w.UserId)
     .OnDelete(DeleteBehavior.Cascade);


            // Indexes
            builder.HasIndex(w => w.UserId)
                .HasDatabaseName("IX_UserWallets_UserId");

            builder.HasIndex(w => w.Amount)
                .HasDatabaseName("IX_UserWallets_Amount");

            builder.HasIndex(w => w.CreatedAt)
                .HasDatabaseName("IX_UserWallets_CreatedAt");
        }
    }
}
