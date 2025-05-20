using System;

namespace HOMMS.Domain.Entities.Base
{
    /// <summary>
    /// Base class for entities that require audit information
    /// </summary>
    /// <typeparam name="TKey">The type of the entity's primary key</typeparam>
    public abstract class AuditableEntity<TKey> : BaseEntity<TKey>, IAuditableEntity
    {
        /// <summary>
        /// Gets or sets the date and time when this entity was created
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// Gets or sets the identifier of the user who created this entity
        /// </summary>
        public string? CreatedBy { get; set; }
        
        /// <summary>
        /// Gets or sets the date and time when this entity was last modified
        /// </summary>
        public DateTime? LastModifiedAt { get; set; }
        
        /// <summary>
        /// Gets or sets the identifier of the user who last modified this entity
        /// </summary>
        public string? LastModifiedBy { get; set; }
    }

    /// <summary>
    /// Base class for auditable entities with an integer primary key
    /// </summary>
    public abstract class AuditableEntity : AuditableEntity<int>
    {
    }
} 