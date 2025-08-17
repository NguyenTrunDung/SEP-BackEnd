using HOMMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HOMMS.Infrastructure.Configurations
{
    public class BranchRoleConfiguration : IEntityTypeConfiguration<BranchRole>
    {
        public void Configure(EntityTypeBuilder<BranchRole> builder)
        {
            // Configure the relationship with BranchUserRoles
            builder.HasMany(br => br.BranchUserRoles)
                .WithOne(bur => bur.BranchRole)
                .HasForeignKey(bur => bur.BranchRoleId);

            // Note: BranchRole no longer has a direct relationship with Branch
            // The association is now through BranchUserRole junction table
        }
    }
} 