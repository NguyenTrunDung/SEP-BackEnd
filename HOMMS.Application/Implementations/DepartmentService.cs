using AutoMapper;
using HOMMS.Application.Interfaces;
using HOMMS.Domain.Dtos;
using HOMMS.Domain.Entities;
using HOMMS.Infrastructure.Repositories.Implementations;
using HOMMS.Infrastructure.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Application.Implementations
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IDepartmentRepository _repository;
        private readonly ILocationRepository _locationRepository;


        public DepartmentService(IUnitOfWork unitOfWork, IMapper mapper, IDepartmentRepository departmentRepository)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _repository = departmentRepository;
        }

        public async Task<DepartmentDto> CreateAsync(CreateDepartmentDto dto)
        {

            var area = await _locationRepository.GetByIdAsync(dto.LocationId);
            if (area == null || area.BranchId != dto.BranchId)
            {
                throw new InvalidOperationException("Invalid location or location does not belong to the specified branch.");
            }

            var isUnique = await _repository.IsDepartmentNameUniqueAsync(dto.BranchId, dto.Name);
            if (!isUnique)
            {
                throw new InvalidOperationException($"Department name '{dto.Name}' already exists in this branch.");
            }

            var dep = _mapper.Map<Department>(dto);
            dep.CreatedAt = DateTime.UtcNow;

            var created = await _repository.AddAsync(dep);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<DepartmentDto>(created);

        }

        public async Task<bool> DeleteAsync(int id)
        {
            var dep = await _repository.GetByIdAsync(id);
            if (dep == null)
            {
                return false;
            }

            // Soft delete
            dep.IsDeleted = true;
            dep.DeletedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(dep);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<DepartmentDto>> GetActiveDepartmentByBranchAsync(int branchId)
        {
           var dep = await _repository.GetActiveDepartmentByBranchAsync(branchId);
            return _mapper.Map<IEnumerable<DepartmentDto>>(dep);
        }

        public async Task<IEnumerable<DepartmentDto>> GetDepartmentByBranchAsync(int branchId)
        {
            var dep = await _repository.GetDepartmentByBranchAsync(branchId);
            return _mapper.Map<IEnumerable<DepartmentDto>>(dep);
        }

        public async Task<bool> IsNameUniqueAsync(int branchId, string name, int? excludeId = null)
        {
            return await _repository.IsDepartmentNameUniqueAsync(branchId, name, excludeId);
        }

        public async Task<DepartmentDto?> UpdateAsync(int id, CreateDepartmentDto dto)
        {
            var existingDep = await _repository.GetByIdAsync(id);
            if (existingDep == null)
            {
                return null;
            }

            var area = await _locationRepository.GetByIdAsync(dto.LocationId);
            if (area == null || area.BranchId != dto.BranchId)
            {
                throw new InvalidOperationException("Invalid location or location does not belong to the specified branch.");
            }

            // Validate uniqueness
            var isUnique = await _repository.IsDepartmentNameUniqueAsync(dto.BranchId, dto.Name, id);
            if (!isUnique)
            {
                throw new InvalidOperationException($"Department name '{dto.Name}' already exists in this branch.");
            }

            // Map updates to existing entity
            _mapper.Map(dto, existingDep);
            existingDep.LastModifiedAt = DateTime.UtcNow;

            // Await the UpdateAsync call to ensure proper execution
            await _repository.UpdateAsync(existingDep);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<DepartmentDto>(existingDep);
        }

        public async Task<DepartmentDto?> GetByIdAsync(int id)
        {
            var dep = await _repository.GetByIdAsync(id);
            return dep != null ? _mapper.Map<DepartmentDto>(dep) : null;
        }

    }
}
