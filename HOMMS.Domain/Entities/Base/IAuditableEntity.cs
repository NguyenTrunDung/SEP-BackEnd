namespace HOMMS.Domain.Entities.Base
{
    /// <summary>
    /// Base class for all entities with a typed identifier
    /// </summary>
    /// <typeparam name="TKey">The type of the entity's primary key</typeparam>
    public abstract class BaseEntity<TKey> : IEntity<TKey>
    {
        /// <summary>
        /// Gets or sets the primary key for this entity
        /// </summary>
        public TKey Id { get; set; } = default!;
    }

    /// <summary>
    /// Base class for entities with an integer primary key
    /// </summary>
    public abstract class BaseEntity : BaseEntity<int>, IEntity
    {
    }
} 