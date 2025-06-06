using HOMMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HOMMS.Infrastructure.Configurations
{
    public class DiseaseCategoryConfiguration : IEntityTypeConfiguration<DiseaseCategory>
    {
        public void Configure(EntityTypeBuilder<DiseaseCategory> builder)
        {
            builder.ToTable("DiseaseCategories");
            
            builder.HasKey(x => x.Id);
            
            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);
                
            builder.Property(x => x.Code)
                .IsRequired()
                .HasMaxLength(20);
                
            builder.Property(x => x.Description)
                .HasMaxLength(1000);
                
            builder.Property(x => x.DietaryRestrictions)
                .HasMaxLength(1000);
                
            builder.Property(x => x.RecommendedFoods)
                .HasMaxLength(1000);
                
            builder.Property(x => x.ColorCode)
                .HasMaxLength(7);
            
            // Configure relationships
            builder.HasOne(x => x.Branch)
                .WithMany(x => x.DiseaseCategories)
                .HasForeignKey(x => x.BranchId)
                .OnDelete(DeleteBehavior.Restrict);
            
            // Configure indexes
            builder.HasIndex(x => new { x.BranchId, x.Code })
                .IsUnique()
                .HasDatabaseName("IX_DiseaseCategories_BranchId_Code");
                
            builder.HasIndex(x => x.BranchId)
                .HasDatabaseName("IX_DiseaseCategories_BranchId");
                
            builder.HasIndex(x => x.IsActive)
                .HasDatabaseName("IX_DiseaseCategories_IsActive");
        }
    }
} 