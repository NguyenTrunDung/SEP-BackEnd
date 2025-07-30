using HOMMS.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace HOMMS.Domain.Entities
{
    /// <summary>
    /// Represents a patient in the hospital system
    /// Patient data can be synced from external hospital management systems
    /// </summary>
    public class Patient : BaseAuditableEntity<string>, IBranchEntity
    {
        /// <summary>
        /// Gets or sets the branch ID where this patient is currently admitted
        /// </summary>
        public int BranchId { get; set; }
        
        /// <summary>
        /// Gets or sets the patient's hospital medical record number (from external system)
        /// </summary>
        [Required]
        [StringLength(50)]
        public string MedicalRecordNumber { get; set; } = string.Empty;
        
        /// <summary>
        /// Gets or sets the patient's full name
        /// </summary>
        [Required]
        [StringLength(200)]
        public string FullName { get; set; } = string.Empty;
        
        /// <summary>
        /// Gets or sets the patient's date of birth
        /// </summary>
        public DateTime? DateOfBirth { get; set; }
        
        /// <summary>
        /// Gets or sets the patient's gender
        /// </summary>
        [StringLength(10)]
        public string? Gender { get; set; }
        
        /// <summary>
        /// Gets or sets the patient's room number
        /// </summary>
        [StringLength(20)]
        public string? RoomNumber { get; set; }
        
        /// <summary>
        /// Gets or sets the patient's bed number
        /// </summary>
        [StringLength(10)]
        public string? BedNumber { get; set; }
        
        /// <summary>
        /// Gets or sets the patient admission date
        /// </summary>
        public DateTime? AdmissionDate { get; set; }
        
        /// <summary>
        /// Gets or sets the patient discharge date (for discharged patients)
        /// </summary>
        public DateTime? DischargeDate { get; set; }
        
        /// <summary>
        /// Gets or sets the attending physician name
        /// </summary>
        [StringLength(100)]
        public string? AttendingPhysician { get; set; }
        
        /// <summary>
        /// Gets or sets whether the patient requires special dietary supervision
        /// </summary>
        public bool RequiresDietarySupervision { get; set; } = false;
        
        /// <summary>
        /// Gets or sets whether this patient is currently active (not discharged)
        /// </summary>
        public bool IsActive { get; set; } = true;
        
        /// <summary>
        /// Gets or sets the external system ID for synchronization
        /// </summary>
        [StringLength(100)]
        public string? ExternalSystemId { get; set; }
        
        /// <summary>
        /// Gets or sets the last synchronization date with external system
        /// </summary>
        public DateTime? LastSyncAt { get; set; }
        
        /// <summary>
        /// Gets or sets notes about the patient
        /// </summary>
        [StringLength(1000)]
        public string? Notes { get; set; }



        public int? departmentId { get; set; }
        public virtual Department? department { get; set; }

        // Navigation properties

        /// <summary>
        /// Gets or sets the branch this patient belongs to
        /// </summary>
        public virtual Branch? Branch { get; set; }
        
        /// <summary>
        /// Gets or sets the user account associated with this patient (optional)
        /// Patient can order without user account, or staff can order on their behalf
        /// </summary>
        public virtual ApplicationUser? User { get; set; }
        
        /// <summary>
        /// Gets or sets the disease categories assigned to this patient
        /// </summary>
        public virtual ICollection<PatientDiseaseCategory> PatientDiseaseCategories { get; set; } = new List<PatientDiseaseCategory>();
        
        /// <summary>
        /// Gets or sets the orders placed for this patient
        /// </summary>
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
} 