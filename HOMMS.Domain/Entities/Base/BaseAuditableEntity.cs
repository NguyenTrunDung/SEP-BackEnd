using System;

namespace HOMMS.Domain.Entities.Base
{
    /// <summary>
    /// Base class for entities with audit information
    /// </summary>
    /// <typeparam name="TKey">The type of the entity's primary key</typeparam>
    public abstract class AuditableEntity<TKey> : BaseEntity<TKey>, IAuditableEntity
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? CreatedBy { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public string? LastModifiedBy { get; set; }
    }

    /// <summary>
    /// Base class for entities with an integer primary key and audit information
    /// </summary>
    public abstract class AuditableEntity : AuditableEntity<int>
    {
    }

    /// <summary>
    /// Complete base entity class with identifier, audit information, and soft delete support
    /// </summary>
    /// <typeparam name="TKey">The type of the entity's primary key</typeparam>
    public abstract class BaseAuditableEntity<TKey> : AuditableEntity<TKey>, ISoftDeletable
    {
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }
    }

    /// <summary>
    /// Complete base entity class with integer identifier, audit information, and soft delete support
    /// </summary>
    public abstract class BaseAuditableEntity : BaseAuditableEntity<int>
    {
    }
}