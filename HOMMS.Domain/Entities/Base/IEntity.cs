using System;

namespace HOMMS.Domain.Entities.Base
{
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