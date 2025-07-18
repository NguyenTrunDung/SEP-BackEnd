using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace HOMMS.Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Generic repository interface for common CRUD operations
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TKey">Primary key type</typeparam>
    public interface IRepository<TEntity, TKey> where TEntity : class
    {
        /// <summary>
        /// Gets all entities
        /// </summary>
        /// <returns>All entities</returns>
        Task<IEnumerable<TEntity>> GetAllAsync();
        
        /// <summary>
        /// Gets entities by a predicate
        /// </summary>
        /// <param name="predicate">Filter expression</param>
        /// <returns>Filtered entities</returns>
        Task<IEnumerable<TEntity>> GetByAsync(Expression<Func<TEntity, bool>> predicate);
        
        /// <summary>
        /// Gets an entity by its ID
        /// </summary>
        /// <param name="id">Entity ID</param>
        /// <returns>Entity if found, null otherwise</returns>
        Task<TEntity> GetByIdAsync(TKey id);
        
        /// <summary>
        /// Adds a new entity
        /// </summary>
        /// <param name="entity">Entity to add</param>
        /// <returns>Added entity</returns>
        Task<TEntity> AddAsync(TEntity entity);
        
        /// <summary>
        /// Updates an existing entity
        /// </summary>
        /// <param name="entity">Entity to update</param>
        /// <returns>Updated entity</returns>
        Task<TEntity> UpdateAsync(TEntity entity);
        
        /// <summary>
        /// Removes an entity by its ID
        /// </summary>
        /// <param name="id">Entity ID</param>
        /// <returns>True if successful, false otherwise</returns>
        Task<bool> DeleteAsync(TKey id);
        
        /// <summary>
        /// Removes an entity
        /// </summary>
        /// <param name="entity">Entity to remove</param>
        /// <returns>True if successful, false otherwise</returns>
        Task<bool> DeleteAsync(TEntity entity);
        
        /// <summary>
        /// Counts entities
        /// </summary>
        /// <returns>Total count of entities</returns>
        Task<int> CountAsync();
        
        /// <summary>
        /// Checks if any entity satisfies a condition
        /// </summary>
        /// <param name="predicate">Condition to check</param>
        /// <returns>True if any entity satisfies the condition, false otherwise</returns>
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate);
    }
} 