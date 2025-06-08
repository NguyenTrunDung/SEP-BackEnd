using HOMMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HOMMS.Infrastructure.Configurations
{
    public class PatientDiseaseCategoryConfiguration : IEntityTypeConfiguration<PatientDiseaseCategory>
    {
        public void Configure(EntityTypeBuilder<PatientDiseaseCategory> builder)
        {
            builder.ToTable("PatientDiseaseCategories");
            
            builder.HasKey(x => x.Id);
            
            builder.Property(x => x.PatientId)
                .IsRequired()
                .HasMaxLength(450); // Standard for Identity user IDs
                
            builder.Property(x => x.PatientSpecificNotes)
                .HasMaxLength(1000);
            
            // Configure relationships
            builder.HasOne(x => x.Branch)
                .WithMany()
                .HasForeignKey(x => x.BranchId)
                .OnDelete(DeleteBehavior.Restrict);
                
            builder.HasOne(x => x.Patient)
                .WithMany(x => x.PatientDiseaseCategories)
                .HasForeignKey(x => x.PatientId)
                .OnDelete(DeleteBehavior.Cascade);
                
            builder.HasOne(x => x.DiseaseCategory)
                .WithMany(x => x.PatientDiseaseCategories)
                .HasForeignKey(x => x.DiseaseCategoryId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Configure indexes
            builder.HasIndex(x => new { x.PatientId, x.DiseaseCategoryId })
                .IsUnique()
                .HasDatabaseName("IX_PatientDiseaseCategories_PatientId_DiseaseCategoryId");
                
            builder.HasIndex(x => x.BranchId)
                .HasDatabaseName("IX_PatientDiseaseCategories_BranchId");
                
            builder.HasIndex(x => x.CreatedBy)
                .HasDatabaseName("IX_PatientDiseaseCategories_CreatedBy");
        }
    }
} 