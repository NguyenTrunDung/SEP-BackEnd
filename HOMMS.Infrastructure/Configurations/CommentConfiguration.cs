
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
    public class CommentConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {

            // Primary key
            builder.HasKey(f => f.Id);

            builder.Property(f => f.Star)
               .IsRequired(false);


            builder.Property(f => f.CommentLines)
                 .HasMaxLength(255)
                 .IsRequired(false);


            builder.HasOne(r => r.Order)
                   .WithMany(o => o.comment)
                   .HasForeignKey(r => r.OrderId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.ApplicationUser)
                   .WithMany(u => u.comment)
                   .HasForeignKey(r => r.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.Branch)
                   .WithMany(b => b.comment)
                   .HasForeignKey(r => r.BranchId)
                   .OnDelete(DeleteBehavior.Restrict);



            builder.HasIndex(f => f.OrderId);
            builder.HasIndex(f => f.UserId);
            builder.HasIndex(f => f.BranchId);



            // Audit columns
            builder.Property(f => f.CreatedAt)
                   .IsRequired();

            builder.Property(f => f.CreatedBy)
                   .HasMaxLength(450)
                   .IsRequired(false);

            builder.Property(f => f.LastModifiedAt)
                   .IsRequired(false);

            builder.Property(f => f.LastModifiedBy)
                   .HasMaxLength(450)
                   .IsRequired(false);

            // Soft delete
            builder.Property(f => f.IsDeleted)
                   .HasDefaultValue(false);


        }


    }
}
