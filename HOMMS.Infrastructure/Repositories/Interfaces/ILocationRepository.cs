using HOMMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Infrastructure.Repositories.Interfaces
{
    public interface ILocationRepository
    {
        /// <summary>
        /// Retrieves all locations from the database
        /// </summary>
        /// <returns>A collection of locations</returns>
        Task<IEnumerable<Location>> GetAllLocationsAsync();

        /// <summary>
        /// Adds a new location to the database
        /// </summary>
        /// <param name="location">The location to create</param>
        /// <returns>True if the creation was successful; otherwise false</returns>
        Task<bool> CreateLocationAsync(Location location);

        /// <summary>
        /// Checks if a location name exists in a specific area
        /// </summary>
        /// <param name="name">The location name</param>
        /// <param name="areaId">The area ID</param>
        /// <returns>A locaiton in area</returns>
        Task<Location?> GetLocationInAreaAsync(string name, int areaId);

        /// <summary>
        /// Upates an existing location in the database
        /// </summary>
        /// <param name="id">The ID of the location to update</param>
        /// <param name="name">The new name to assign</param>
        /// <param name="areaId">The area ID to which the location belongs</param>
        /// <returns>True if the update was successful; otherwise false</returns>
        Task<bool> UpdateLocationAsync(int id, string name, int areaId);

        /// <summary>
        /// Deletes a location by its ID
        /// </summary>
        /// <param name="id">The location to delete</param>
        /// <returns>True if the deletion was successful; otherwise false</returns>
        Task<bool> DeleteLocationAsync(int id);
    }
}
