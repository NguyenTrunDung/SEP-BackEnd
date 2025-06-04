using HOMMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HOMMS.Infrastructure.Configurations
{
    public class UserWalletTransactionConfiguration : IEntityTypeConfiguration<UserWalletTransaction>
    {
        public void Configure(EntityTypeBuilder<UserWalletTransaction> builder)
        {
            // Table name
            builder.ToTable("UserWalletTransactions");

            // Primary key
            builder.HasKey(t => t.Id);

            // Properties
            builder.Property(t => t.UserId)
                .IsRequired()
                .HasMaxLength(450); // Match AspNetUsers Id length

            builder.Property(t => t.BranchId)
                .IsRequired();

            builder.Property(t => t.TransactionType)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(t => t.Amount)
                .IsRequired()
                .HasColumnType("bigint"); // VND amounts as whole numbers

            builder.Property(t => t.BalanceAfter)
                .HasColumnType("bigint"); // VND amounts as whole numbers

            builder.Property(t => t.Description)
                .IsRequired()
                .HasMaxLength(500);

            // Relationships
            builder.HasOne(t => t.User)
                .WithMany(u => u.WalletTransactions)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(t => t.Branch)
                .WithMany()
                .HasForeignKey(t => t.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.Order)
                .WithMany(o => o.WalletTransactions)
                .HasForeignKey(t => t.OrderId)
                .OnDelete(DeleteBehavior.SetNull);

            // Indexes
            builder.HasIndex(t => t.UserId)
                .HasDatabaseName("IX_UserWalletTransactions_UserId");

            builder.HasIndex(t => t.BranchId)
                .HasDatabaseName("IX_UserWalletTransactions_BranchId");

            builder.HasIndex(t => t.CreatedAt)
                .HasDatabaseName("IX_UserWalletTransactions_CreatedAt");

            builder.HasIndex(t => t.TransactionType)
                .HasDatabaseName("IX_UserWalletTransactions_TransactionType");

            // Composite index for user transactions by date
            builder.HasIndex(t => new { t.UserId, t.CreatedAt })
                .HasDatabaseName("IX_UserWalletTransactions_UserId_CreatedAt");
        }
    }
} 