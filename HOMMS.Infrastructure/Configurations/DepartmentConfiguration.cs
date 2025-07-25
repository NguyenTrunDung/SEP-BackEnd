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
    public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            // Primary key
            builder.HasKey(f => f.Id);


            builder.Property(f => f.Name)
                 .HasMaxLength(255)
                 .IsRequired(false);

            builder.Property(l => l.Sort)
                 .IsRequired(false);

            builder.Property(l => l.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.HasOne(r => r.Branch)
                  .WithMany(b => b.department)
                  .HasForeignKey(r => r.BranchId)
                  .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(f => f.BranchId);

            // Audit columns
            builder.Property(l => l.CreatedAt)
                   .IsRequired();

            builder.Property(l => l.CreatedBy)
                   .HasMaxLength(450)
                   .IsRequired(false);

            builder.Property(l => l.LastModifiedAt)
                   .IsRequired(false);

            builder.Property(l => l.LastModifiedBy)
                   .HasMaxLength(450)
                   .IsRequired(false);

            // Soft delete
            builder.Property(l => l.IsDeleted)
                   .IsRequired()
                   .HasDefaultValue(false);

            builder.Property(l => l.DeletedAt)
                   .IsRequired(false);

            builder.Property(l => l.DeletedBy)
                   .HasMaxLength(450)
                   .IsRequired(false);


        }
    }
}

