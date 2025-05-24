using HOMMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HOMMS.Infrastructure.Configurations
{
    public class BranchUserRoleConfiguration : IEntityTypeConfiguration<BranchUserRole>
    {
        public void Configure(EntityTypeBuilder<BranchUserRole> builder)
        {
            builder.HasOne(bur => bur.User)
                .WithMany()
                .HasForeignKey(bur => bur.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(bur => bur.BranchRole)
                .WithMany(br => br.BranchUserRoles)
                .HasForeignKey(bur => bur.BranchRoleId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(bur => bur.Branch)
                .WithMany()
                .HasForeignKey(bur => bur.BranchId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent multiple cascade paths
        }
    }
} 