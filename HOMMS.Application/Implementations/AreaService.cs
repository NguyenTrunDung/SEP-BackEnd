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
    /// Implementation of area service
    /// </summary>
    public class AreaService : IAreaService
    {
        private readonly IAreaRepository _areaRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AreaService(
            IAreaRepository areaRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _areaRepository = areaRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<AreaDto>> GetAreasByBranchAsync(int branchId)
        {
            var areas = await _areaRepository.GetAreasByBranchAsync(branchId);
            return _mapper.Map<IEnumerable<AreaDto>>(areas);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<AreaDto>> GetActiveAreasByBranchAsync(int branchId)
        {
            var areas = await _areaRepository.GetActiveAreasByBranchAsync(branchId);
            return _mapper.Map<IEnumerable<AreaDto>>(areas);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<AreaDto>> GetAreasWithLocationsByBranchAsync(int branchId)
        {
            var areas = await _areaRepository.GetAreasWithLocationsByBranchAsync(branchId);
            return _mapper.Map<IEnumerable<AreaDto>>(areas);
        }

        /// <inheritdoc/>
        public async Task<AreaDto?> GetByIdAsync(int id)
        {
            var area = await _areaRepository.GetByIdAsync(id);
            return area != null ? _mapper.Map<AreaDto>(area) : null;
        }

        /// <inheritdoc/>
        public async Task<AreaDto?> GetWithLocationsAsync(int id)
        {
            var area = await _areaRepository.GetAreaWithLocationsAsync(id);
            return area != null ? _mapper.Map<AreaDto>(area) : null;
        }

        /// <inheritdoc/>
        public async Task<AreaDto> CreateAsync(CreateAreaDto dto)
        {
            // Validate uniqueness
            var isUnique = await _areaRepository.IsAreaNameUniqueAsync(dto.BranchId, dto.Name);
            if (!isUnique)
            {
                throw new InvalidOperationException($"Area name '{dto.Name}' already exists in this branch.");
            }

            var area = _mapper.Map<Area>(dto);
            area.CreatedAt = DateTime.UtcNow;

            var created = await _areaRepository.AddAsync(area);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<AreaDto>(created);
        }

        /// <inheritdoc/>
        public async Task<AreaDto?> UpdateAsync(int id, UpdateAreaDto dto)
        {
            var existingArea = await _areaRepository.GetByIdAsync(id);
            if (existingArea == null)
            {
                return null;
            }

            // Validate uniqueness
            var isUnique = await _areaRepository.IsAreaNameUniqueAsync(dto.BranchId, dto.Name, id);
            if (!isUnique)
            {
                throw new InvalidOperationException($"Area name '{dto.Name}' already exists in this branch.");
            }

            // Map updates to existing entity
            _mapper.Map(dto, existingArea);
            existingArea.LastModifiedAt = DateTime.UtcNow;

            // Await the UpdateAsync call to ensure proper execution
            await _areaRepository.UpdateAsync(existingArea);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<AreaDto>(existingArea);
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteAsync(int id)
        {
            var area = await _areaRepository.GetByIdAsync(id);
            if (area == null)
            {
                return false;
            }

            // Soft delete
            area.IsDeleted = true;
            area.DeletedAt = DateTime.UtcNow;

            await _areaRepository.UpdateAsync(area);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        /// <inheritdoc/>
        public async Task<bool> IsNameUniqueAsync(int branchId, string name, int? excludeId = null)
        {
            return await _areaRepository.IsAreaNameUniqueAsync(branchId, name, excludeId);
        }
    }
} 