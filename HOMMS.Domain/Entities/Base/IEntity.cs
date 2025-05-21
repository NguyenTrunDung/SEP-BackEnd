using System;

namespace HOMMS.Domain.Entities.Base
{
    /// <summary>
    /// Base interface for all entities, providing a strongly typed identifier
    /// </summary>
    /// <typeparam name="TKey">The type of the entity's primary key</typeparam>
    public interface IEntity<TKey>
    {
        /// <summary>
        /// Gets or sets the primary key for this entity
        /// </summary>
        TKey Id { get; set; }
    }

    /// <summary>
    /// Default implementation of IEntity using int as the key type
    /// </summary>
    public interface IEntity : IEntity<int>
    {
    }
    
    /// <summary>
    /// Interface for entities that support soft deletion
    /// </summary>
    public interface ISoftDeletable
    {
        /// <summary>
        /// Gets or sets a value indicating whether this entity has been deleted
        /// </summary>
        bool IsDeleted { get; set; }
        
        /// <summary>
        /// Gets or sets the date and time when this entity was deleted
        /// </summary>
        DateTime? DeletedAt { get; set; }
        
        /// <summary>
        /// Gets or sets the identifier of the user who deleted this entity
        /// </summary>
        string? DeletedBy { get; set; }
    }
} 