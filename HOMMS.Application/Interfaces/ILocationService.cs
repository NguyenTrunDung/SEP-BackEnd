using HOMMS.Domain.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Application.Interfaces
{
    public interface ILocationService
    {
        Task<IEnumerable<LocationDto>> GetAllLocationsAsync();
        Task<bool> CreateLocationAsync(LocationDto locationDto);
        Task<bool> UpdateLocationAsync(int id, LocationDto locationDto);
        Task<bool> DeleteLocationAsync(int id);
    }
}
