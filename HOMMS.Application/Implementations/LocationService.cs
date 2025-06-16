using AutoMapper;
using HOMMS.Application.Interfaces;
using HOMMS.Domain.Dtos;
using HOMMS.Domain.Entities;
using HOMMS.Infrastructure.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Application.Implementations
{
    public class LocationService : ILocationService
    {
        private readonly ILocationRepository _locationRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public LocationService(ILocationRepository locationRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _locationRepository = locationRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<LocationDto>> GetAllLocationsAsync()
        {
            var locations = await _locationRepository.GetAllLocationsAsync();
            return _mapper.Map<IEnumerable<LocationDto>>(locations);
        }

        public async Task<bool> CreateLocationAsync(LocationDto locationDto)
        {
            var location = _mapper.Map<Location>(locationDto);
            return await _locationRepository.CreateLocationAsync(location);
        }

        public async Task<bool> UpdateLocationAsync(int id, LocationDto locationDto)
        {
            return await _locationRepository.UpdateLocationAsync(id, locationDto.Name, locationDto.AreaId);
        }

        public async Task<bool> DeleteLocationAsync(int id)
        {
            return await _locationRepository.DeleteLocationAsync(id);
        }
    }
}
