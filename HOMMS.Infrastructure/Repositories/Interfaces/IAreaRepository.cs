using HOMMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Infrastructure.Repositories.Interfaces
{
    public interface IAreaRepository : IRepository<Area, int>
    {
        /// <summary>
        /// Retrieves all areas
        /// </summary>
        /// <returns>A collection of areas</returns>
        Task<IEnumerable<Area>> GetAllAreasAsync();

        /// <summary>
        /// Adds a new area to the database.
        /// </summary>
        /// <param name="area">The new area to create</param>
        /// <returns>True if the creation was successful; otherwise false</returns>
        Task<bool> CreateAreaAsync(Area area);

        /// <summary>
        /// Updates an existing area in the database.
        /// </summary>
        /// <param name="id">The ID of the area to update</param>
        /// <param name="name">The new name to assign</param>
        /// <returns>True if the update was successful; otherwise false</returns>
        Task<bool> UpdateAreaAsync(int id, string name);

        /// <summary>
        /// Deletes an area by its ID.
        /// </summary>
        /// <param name="id">The area to delete</param>
        /// <returns>True if the deletion was successful; otherwise false</returns>
        Task<bool> DeleteAreaAsync(int id);
    }
}
