using HOMMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HOMMS.Infrastructure.Configurations
{
    /// <summary>
    /// Entity Framework configuration for Patient entity
    /// </summary>
    public class PatientConfiguration : IEntityTypeConfiguration<Patient>
    {
        public void Configure(EntityTypeBuilder<Patient> builder)
        {
            builder.ToTable("Patients");
            
            builder.HasKey(x => x.Id);
            
            // Configure primary key
            builder.Property(x => x.Id)
                .HasMaxLength(450)
                .IsRequired();
                
            // Configure required properties
            builder.Property(x => x.MedicalRecordNumber)
                .IsRequired()
                .HasMaxLength(50);
                
            builder.Property(x => x.FullName)
                .IsRequired()
                .HasMaxLength(200);
            
            // Configure optional properties
            builder.Property(x => x.Gender)
                .HasMaxLength(10);
                
            builder.Property(x => x.RoomNumber)
                .HasMaxLength(20);
                
            builder.Property(x => x.BedNumber)
                .HasMaxLength(10);
                
            builder.Property(x => x.AttendingPhysician)
                .HasMaxLength(100);
                
            builder.Property(x => x.ExternalSystemId)
                .HasMaxLength(100);
                
            builder.Property(x => x.Notes)
                .HasMaxLength(1000);
            
            // Configure default values
            builder.Property(x => x.RequiresDietarySupervision)
                .HasDefaultValue(false);
                
            builder.Property(x => x.IsActive)
                .HasDefaultValue(true);
        
            // Configure relationships
            builder.HasOne(x => x.Branch)
                .WithMany(x => x.Patients)
                .HasForeignKey(x => x.BranchId)
                .OnDelete(DeleteBehavior.Restrict);


          
            builder.HasOne(x => x.department)
                .WithMany(x => x.Patients)
                .HasForeignKey(x => x.departmentId)
                .OnDelete(DeleteBehavior.SetNull);


            // Configure indexes
            builder.HasIndex(x => new { x.BranchId, x.MedicalRecordNumber })
                .IsUnique()
                .HasDatabaseName("IX_Patients_BranchId_MedicalRecordNumber");
                
            builder.HasIndex(x => x.ExternalSystemId)
                .HasDatabaseName("IX_Patients_ExternalSystemId");
                
            builder.HasIndex(x => x.IsActive)
                .HasDatabaseName("IX_Patients_IsActive");
                
            builder.HasIndex(x => x.BranchId)
                .HasDatabaseName("IX_Patients_BranchId");
                
            builder.HasIndex(x => new { x.BranchId, x.RoomNumber, x.BedNumber })
                .HasDatabaseName("IX_Patients_BranchId_Room_Bed");
        }
    }
} 