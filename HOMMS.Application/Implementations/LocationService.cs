using AutoMapper;
using HOMMS.Application.Interfaces;
using HOMMS.Domain.Dtos;
using HOMMS.Domain.Entities;
using HOMMS.Infrastructure.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HOMMS.Application.Implementations
{
    /// <summary>
    /// Implementation of location service
    /// </summary>
    public class LocationService : ILocationService
    {
        private readonly ILocationRepository _locationRepository;
        private readonly IAreaRepository _areaRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public LocationService(
            ILocationRepository locationRepository,
            IAreaRepository areaRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _locationRepository = locationRepository;
            _areaRepository = areaRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<LocationDto>> GetLocationsByAreaAsync(int areaId)
        {
            var locations = await _locationRepository.GetLocationsByAreaAsync(areaId);
            return _mapper.Map<IEnumerable<LocationDto>>(locations);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<LocationDto>> GetActiveLocationsByAreaAsync(int areaId)
        {
            var locations = await _locationRepository.GetActiveLocationsByAreaAsync(areaId);
            return _mapper.Map<IEnumerable<LocationDto>>(locations);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<LocationDto>> GetLocationsByBranchAsync(int branchId)
        {
            var locations = await _locationRepository.GetLocationsByBranchAsync(branchId);
            return _mapper.Map<IEnumerable<LocationDto>>(locations);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<LocationDto>> GetActiveLocationsByBranchAsync(int branchId)
        {
            var locations = await _locationRepository.GetActiveLocationsByBranchAsync(branchId);
            return _mapper.Map<IEnumerable<LocationDto>>(locations);
        }

        /// <inheritdoc/>
        public async Task<LocationDto?> GetByIdAsync(int id)
        {
            var location = await _locationRepository.GetByIdAsync(id);
            return location != null ? _mapper.Map<LocationDto>(location) : null;
        }

        /// <inheritdoc/>
        public async Task<LocationDto?> GetWithAreaAsync(int id)
        {
            var location = await _locationRepository.GetLocationWithAreaAsync(id);
            return location != null ? _mapper.Map<LocationDto>(location) : null;
        }

        /// <inheritdoc/>
        public async Task<LocationDto> CreateAsync(CreateLocationDto dto)
        {
            // Validate that area exists and belongs to the same branch
            var area = await _areaRepository.GetByIdAsync(dto.AreaId);
            if (area == null || area.BranchId != dto.BranchId)
            {
                throw new InvalidOperationException("Invalid area or area does not belong to the specified branch.");
            }

            // Validate location name uniqueness within area
            var isNameUnique = await _locationRepository.IsLocationNameUniqueAsync(dto.AreaId, dto.Name);
            if (!isNameUnique)
            {
                throw new InvalidOperationException($"Location name '{dto.Name}' already exists in this area.");
            }

            // Validate room number uniqueness within branch (if provided)
            if (!string.IsNullOrWhiteSpace(dto.RoomNumber))
            {
                var isRoomNumberUnique = await _locationRepository.IsRoomNumberUniqueAsync(dto.BranchId, dto.RoomNumber);
                if (!isRoomNumberUnique)
                {
                    throw new InvalidOperationException($"Room number '{dto.RoomNumber}' already exists in this branch.");
                }
            }

            var location = _mapper.Map<Location>(dto);
            location.CreatedAt = DateTime.UtcNow;

            var created = await _locationRepository.AddAsync(location);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<LocationDto>(created);
        }

        /// <inheritdoc/>
        public async Task<LocationDto?> UpdateAsync(int id, UpdateLocationDto dto)
        {
            var existingLocation = await _locationRepository.GetByIdAsync(id);
            if (existingLocation == null)
            {
                return null;
            }

            // Validate that area exists and belongs to the same branch
            var area = await _areaRepository.GetByIdAsync(dto.AreaId);
            if (area == null || area.BranchId != dto.BranchId)
            {
                throw new InvalidOperationException("Invalid area or area does not belong to the specified branch.");
            }

            // Validate location name uniqueness within area
            var isNameUnique = await _locationRepository.IsLocationNameUniqueAsync(dto.AreaId, dto.Name, id);
            if (!isNameUnique)
            {
                throw new InvalidOperationException($"Location name '{dto.Name}' already exists in this area.");
            }

            // Validate room number uniqueness within branch (if provided)
            if (!string.IsNullOrWhiteSpace(dto.RoomNumber))
            {
                var isRoomNumberUnique = await _locationRepository.IsRoomNumberUniqueAsync(dto.BranchId, dto.RoomNumber, id);
                if (!isRoomNumberUnique)
                {
                    throw new InvalidOperationException($"Room number '{dto.RoomNumber}' already exists in this branch.");
                }
            }

            // Map updates to existing entity
            _mapper.Map(dto, existingLocation);
            existingLocation.LastModifiedAt = DateTime.UtcNow;

            await _locationRepository.UpdateAsync(existingLocation);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<LocationDto>(existingLocation);
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteAsync(int id)
        {
            var location = await _locationRepository.GetByIdAsync(id);
            if (location == null)
            {
                return false;
            }

            // Soft delete
            location.IsDeleted = true;
            location.DeletedAt = DateTime.UtcNow;

            await _locationRepository.UpdateAsync(location);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        /// <inheritdoc/>
        public async Task<bool> IsNameUniqueAsync(int areaId, string name, int? excludeId = null)
        {
            return await _locationRepository.IsLocationNameUniqueAsync(areaId, name, excludeId);
        }

        /// <inheritdoc/>
        public async Task<bool> IsRoomNumberUniqueAsync(int branchId, string roomNumber, int? excludeId = null)
        {
            return await _locationRepository.IsRoomNumberUniqueAsync(branchId, roomNumber, excludeId);
        }
    }
} 