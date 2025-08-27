using HOMMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HOMMS.Infrastructure.Configurations
{
    public class DiseaseCategoryFoodRestrictionConfiguration : IEntityTypeConfiguration<DiseaseCategoryFoodRestriction>
    {
        public void Configure(EntityTypeBuilder<DiseaseCategoryFoodRestriction> builder)
        {
            builder.ToTable("DiseaseCategoryFoodRestrictions");
            
            builder.HasKey(x => x.Id);
            
            builder.Property(x => x.Reason)
                .IsRequired()
                .HasMaxLength(500);
                
            builder.Property(x => x.AlternativeRecommendations)
                .HasMaxLength(500);
                
            builder.Property(x => x.MealTime)
                .HasMaxLength(100)
                .IsRequired(false);

            builder.Property(x => x.Name)
                .HasMaxLength(100)
                .IsRequired(false);
                
            builder.Property(x => x.Price)
                .HasPrecision(10, 2)
                .IsRequired(false);
            
            // Configure relationships
            builder.HasOne(x => x.Branch)
                .WithMany()
                .HasForeignKey(x => x.BranchId)
                .OnDelete(DeleteBehavior.Restrict);
                
            builder.HasOne(x => x.DiseaseCategory)
                .WithMany(x => x.FoodRestrictions)
                .HasForeignKey(x => x.DiseaseCategoryId)
                .OnDelete(DeleteBehavior.Cascade);
                
            builder.HasOne(x => x.Food)
                .WithMany(x => x.FoodRestrictions)
                .HasForeignKey(x => x.FoodId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false); // Make FoodId nullable
            
            // Configure indexes
            // Note: Unique constraint on DiseaseCategoryId + FoodId might not work well with nullable FoodId
            // Consider removing this unique constraint or modifying it based on business requirements
            // builder.HasIndex(x => new { x.DiseaseCategoryId, x.FoodId })
            //     .IsUnique()
            //     .HasDatabaseName("IX_DiseaseCategoryFoodRestrictions_DiseaseCategoryId_FoodId");
                
            builder.HasIndex(x => x.BranchId)
                .HasDatabaseName("IX_DiseaseCategoryFoodRestrictions_BranchId");
                
            builder.HasIndex(x => x.IsActive)
                .HasDatabaseName("IX_DiseaseCategoryFoodRestrictions_IsActive");
                
            builder.HasIndex(x => x.RestrictionLevel)
                .HasDatabaseName("IX_DiseaseCategoryFoodRestrictions_RestrictionLevel");
                
            builder.HasIndex(x => x.MealTime)
                .HasDatabaseName("IX_DiseaseCategoryFoodRestrictions_MealTime");
        }
    }
} 