using System;

namespace HOMMS.Domain.Entities.Base
{
    /// <summary>
    /// Complete base entity class with identifier, audit information, and soft delete support
    /// </summary>
    /// <typeparam name="TKey">The type of the entity's primary key</typeparam>
    public abstract class BaseAuditableEntity<TKey> : AuditableEntity<TKey>, ISoftDeletable
    {
        /// <summary>
        /// Gets or sets a value indicating whether this entity has been deleted
        /// </summary>
        public bool IsDeleted { get; set; } = false;
        
        /// <summary>
        /// Gets or sets the date and time when this entity was deleted
        /// </summary>
        public DateTime? DeletedAt { get; set; }
        
        /// <summary>
        /// Gets or sets the identifier of the user who deleted this entity
        /// </summary>
        public string? DeletedBy { get; set; }
    }

    /// <summary>
    /// Complete base entity class with integer identifier, audit information, and soft delete support
    /// </summary>
    public abstract class BaseAuditableEntity : BaseAuditableEntity<int>
    {
    }
} 