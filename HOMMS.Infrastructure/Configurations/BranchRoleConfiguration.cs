using HOMMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HOMMS.Infrastructure.Configurations
{
    public class BranchRoleConfiguration : IEntityTypeConfiguration<BranchRole>
    {
        public void Configure(EntityTypeBuilder<BranchRole> builder)
        {
            builder.HasOne(br => br.Branch)
                .WithMany(b => b.BranchRoles)
                .HasForeignKey(br => br.BranchId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(br => br.BranchUserRoles)
                .WithOne(bur => bur.BranchRole)
                .HasForeignKey(bur => bur.BranchRoleId);
        }
    }
} 