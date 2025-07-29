using System.ComponentModel.DataAnnotations;

namespace HOMMS.Domain.Dtos
{
    /// <summary>
    /// DTO for patient information display
    /// </summary>
    public class PatientDto
    {
        public string Id { get; set; } = string.Empty;
        public int BranchId { get; set; }

        public int departmentId { get; set; }
        public string MedicalRecordNumber { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? RoomNumber { get; set; }
        public string? BedNumber { get; set; }
        public DateTime? AdmissionDate { get; set; }
        public DateTime? DischargeDate { get; set; }
        public string? AttendingPhysician { get; set; }
        public bool RequiresDietarySupervision { get; set; }
        public bool IsActive { get; set; }
        public string? ExternalSystemId { get; set; }
        public DateTime? LastSyncAt { get; set; }
        public string? Notes { get; set; }       

        // Related data for display
        public string? UserId { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public int DiseaseCategoryCount { get; set; }
        public List<PatientDiseaseCategoryDto> DiseaseCategories { get; set; } = new();
        
        // Computed properties
        public int? Age => DateOfBirth?.CalculateAge();
        public bool IsCurrentlyAdmitted => IsActive && AdmissionDate.HasValue && !DischargeDate.HasValue;
        public string DisplayLocation => !string.IsNullOrEmpty(RoomNumber) 
            ? $"Room {RoomNumber}" + (!string.IsNullOrEmpty(BedNumber) ? $" - Bed {BedNumber}" : "")
            : "";
    }
    
    /// <summary>
    /// DTO for creating new patients
    /// </summary>
    public class CreatePatientDto
    {
        [Required]
        [StringLength(450)]
        public string Id { get; set; } = string.Empty; // External system ID

        public int BranchId { get; set; }

        public int departmentId { get; set; }

        [Required]
        [StringLength(50)]
        public string MedicalRecordNumber { get; set; } = string.Empty;
        
        [Required]
        [StringLength(200)]
        public string FullName { get; set; } = string.Empty;
        
        public DateTime? DateOfBirth { get; set; }
        
        [StringLength(10)]
        public string? Gender { get; set; }
        
        [StringLength(20)]
        public string? RoomNumber { get; set; }
        
        [StringLength(10)]
        public string? BedNumber { get; set; }
        
        public DateTime? AdmissionDate { get; set; }

        public DateTime? DischargeDate { get; set; }

        [StringLength(100)]
        public string? AttendingPhysician { get; set; }
        
        public bool RequiresDietarySupervision { get; set; } = false;
        
        [StringLength(100)]
        public string? ExternalSystemId { get; set; }
        
        [StringLength(1000)]
        public string? Notes { get; set; }


       
    }
    
    /// <summary>
    /// DTO for updating patient information
    /// </summary>
    public class UpdatePatientDto
    {
        public int BranchId { get; set; }
        public int departmentId { get; set; }

        [StringLength(50)]
        public string? MedicalRecordNumber { get; set; }
        
        [StringLength(200)]
        public string? FullName { get; set; }
        
        public DateTime? DateOfBirth { get; set; }
        
        [StringLength(10)]
        public string? Gender { get; set; }
        
        [StringLength(20)]
        public string? RoomNumber { get; set; }
        
        [StringLength(10)]
        public string? BedNumber { get; set; }
        
        public DateTime? AdmissionDate { get; set; }
        
        public DateTime? DischargeDate { get; set; }
        
        [StringLength(100)]
        public string? AttendingPhysician { get; set; }
        
        public bool? RequiresDietarySupervision { get; set; }
        
        public bool? IsActive { get; set; }
        
        [StringLength(1000)]
        public string? Notes { get; set; }

       
    }
    
    /// <summary>
    /// DTO for patient synchronization from external systems
    /// </summary>
    public class PatientSyncDto
    {
        public int BranchId { get; set; }
        public int departmentId { get; set; }

        [Required]
        public string ExternalSystemId { get; set; } = string.Empty;
        
        [Required]
        public string MedicalRecordNumber { get; set; } = string.Empty;
        
        [Required]
        public string FullName { get; set; } = string.Empty;
        
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? RoomNumber { get; set; }
        public string? BedNumber { get; set; }
        public DateTime? AdmissionDate { get; set; }
        public DateTime? DischargeDate { get; set; }
        public string? AttendingPhysician { get; set; }
        public bool RequiresDietarySupervision { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public string? Notes { get; set; }
        public DateTime SyncTimestamp { get; set; } = DateTime.UtcNow;

     
    }
    
    /// <summary>
    /// DTO for patient list with basic information
    /// </summary>
    public class PatientListDto
    {
        public string Id { get; set; } = string.Empty;
        public int BranchId { get; set; }
        public int departmentId { get; set; }
        public string MedicalRecordNumber { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? RoomNumber { get; set; }
        public string? BedNumber { get; set; }
        public string? AttendingPhysician { get; set; }
        public bool RequiresDietarySupervision { get; set; }
        public bool IsActive { get; set; }
        public int DiseaseCategoryCount { get; set; }
        public DateTime? AdmissionDate { get; set; }
        public DateTime? DischargeDate { get; set; }
        public string DisplayLocation => !string.IsNullOrEmpty(RoomNumber) 
            ? $"Room {RoomNumber}" + (!string.IsNullOrEmpty(BedNumber) ? $" - Bed {BedNumber}" : "")
            : "Not assigned";
    
    }
    
    /// <summary>
    /// DTO for linking user accounts to patient records
    /// </summary>
    public class LinkPatientUserDto
    {
        [Required]
        public string PatientId { get; set; } = string.Empty;
        
        [Required]
        public string UserId { get; set; } = string.Empty;
        
        public string? Notes { get; set; }
    }
}

/// <summary>
/// Extension methods for patient DTOs
/// </summary>
public static class PatientDtoExtensions
{
    /// <summary>
    /// Calculate age from date of birth
    /// </summary>
    public static int CalculateAge(this DateTime dateOfBirth)
    {
        var today = DateTime.Today;
        var age = today.Year - dateOfBirth.Year;
        if (dateOfBirth.Date > today.AddYears(-age)) age--;
        return age;
    }
} 