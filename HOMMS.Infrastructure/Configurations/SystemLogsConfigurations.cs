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
    public class SystemLogsConfigurations : IEntityTypeConfiguration<SystemLog>
    {
        public void Configure(EntityTypeBuilder<SystemLog> builder)
        {
            builder.ToTable("SystemLogs");
            // Primary key
            builder.HasKey(f => f.Id);



            builder.HasIndex(f => f.BranchId);

            builder.HasOne(f => f.Branch)
                  .WithMany(b => b.SystemLogs)
                  .HasForeignKey(f => f.BranchId)
                  .OnDelete(DeleteBehavior.Restrict);


            builder.Property(f => f.UserId)
                              .IsRequired(false);


            builder.Property(f => f.Note)
                   .HasMaxLength(255)
                   .IsRequired(false);


            builder.Property(f => f.CreatedAt)
                  .IsRequired();

            builder.Property(f => f.LastModifiedAt)
                 .IsRequired(false);

        }
    }
}
