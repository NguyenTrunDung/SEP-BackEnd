using HOMMS.Domain.Entities.Base;
using System;
using System.ComponentModel.DataAnnotations;

namespace HOMMS.Domain.Entities
{
    /// <summary>
    /// Junction entity linking patients (customers) with their disease categories
    /// Allows tracking of patient medical conditions for dietary management
    /// </summary>
    public class PatientDiseaseCategory : BaseAuditableEntity<int>, IBranchEntity
    {
        /// <summary>
        /// Gets or sets the branch ID this relationship belongs to
        /// </summary>
        public int BranchId { get; set; }
        
        /// <summary>
        /// Gets or sets the patient (customer) ID
        /// </summary>
        [Required]
        public string PatientId { get; set; } = string.Empty;
        
        /// <summary>
        /// Gets or sets the disease category ID
        /// </summary>
        [Required]
        public int DiseaseCategoryId { get; set; }
        
        /// <summary>
        /// Gets or sets when this disease category was diagnosed for the patient
        /// </summary>
        public DateTime? DiagnosedDate { get; set; }
        
        /// <summary>
        /// Gets or sets the severity level for this specific patient (can override category default)
        /// </summary>
        public int? PatientSeverityLevel { get; set; }
        
        /// <summary>
        /// Gets or sets additional notes specific to this patient's condition
        /// </summary>
        [StringLength(1000)]
        public string? PatientSpecificNotes { get; set; }
        
        /// <summary>
        /// Gets or sets whether this disease category is currently active for the patient
        /// </summary>
        public bool IsActive { get; set; } = true;
        
        /// <summary>
        /// Gets or sets when this assignment expires (if temporary)
        /// </summary>
        public DateTime? ExpiryDate { get; set; }
        
        // Navigation properties
        
        /// <summary>
        /// Gets or sets the branch this relationship belongs to
        /// </summary>
        public virtual Branch? Branch { get; set; }
        
        /// <summary>
        /// Gets or sets the patient associated with this disease category
        /// </summary>
        public virtual Patient? Patient { get; set; }
        
        /// <summary>
        /// Gets or sets the disease category associated with this patient
        /// </summary>
        public virtual DiseaseCategory? DiseaseCategory { get; set; }
    }
} 