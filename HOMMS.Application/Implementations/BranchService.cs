using HOMMS.Application.Interfaces;
using HOMMS.Domain.Dtos;
using HOMMS.Domain.Entities;
using HOMMS.Infrastructure.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using System;

namespace HOMMS.Application.Implementations
{
    public class BranchService : IBranchService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBranchContext _branchContext;
        private readonly IMapper _mapper;

        public BranchService(IUnitOfWork unitOfWork, IBranchContext branchContext, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _branchContext = branchContext;
            _mapper = mapper;
        }

        public async Task<List<BranchDto>> GetBranchesAsync(string userId)
        {
            var branches = await _unitOfWork.BranchRepository.GetUserBranchesAsync(userId);
            return _mapper.Map<List<BranchDto>>(branches);
        }

        public async Task<List<BranchDto>> GetActiveBranchesAsync()
        {
            var branches = await _unitOfWork.BranchRepository.GetActiveBranchesAsync();
            return _mapper.Map<List<BranchDto>>(branches);
        }

        public async Task<BranchDto> GetDefaultBranchAsync(string userId)
        {
            var branch = await _unitOfWork.BranchRepository.GetUserDefaultBranchAsync(userId);
            if (branch == null)
            {
                int branchId = _branchContext.GetCurrentBranchId();
                branch = await _unitOfWork.BranchRepository.GetByIdAsync(branchId);
            }
            if (branch == null)
            {
                var activeBranches = await _unitOfWork.BranchRepository.GetActiveBranchesAsync();
                branch = activeBranches.FirstOrDefault();
            }
            return branch == null ? null : _mapper.Map<BranchDto>(branch);
        }

        public async Task<BranchDto> GetByIdAsync(int branchId)
        {
            var branch = await _unitOfWork.BranchRepository.GetByIdAsync(branchId);
            return branch == null ? null : _mapper.Map<BranchDto>(branch);
        }

        public async Task SetUserDefaultBranchAsync(string userId, int branchId)
        {
            await _unitOfWork.BranchRepository.SetUserDefaultBranchAsync(userId, branchId);
            await _unitOfWork.SaveChangesAsync();
        }

        public int GetCurrentBranchId() => _branchContext.GetCurrentBranchId();
        public void SetCurrentBranchId(int branchId) => _branchContext.SetCurrentBranchId(branchId);
        
        /// <summary>
        /// Checks if a branch name is available (not used by another branch)
        /// </summary>
        /// <param name="name">Branch name to check</param>
        /// <param name="excludeId">Branch ID to exclude from check (for updates)</param>
        /// <returns>True if name is available, false if already taken</returns>
        public async Task<bool> IsBranchNameAvailableAsync(string name, int? excludeId = null)
        {
            return !await _unitOfWork.BranchRepository.ExistsByNameAsync(name, excludeId);
        }

        // New methods for controller support
        public async Task<List<BranchDto>> GetBranchesForUserAsync(string userId, bool isSystemAdmin)
        {
            if (isSystemAdmin)
            {
                // If user is SystemAdmin, return all active branches
                var allBranches = await _unitOfWork.BranchRepository.GetActiveBranchesAsync();
                return _mapper.Map<List<BranchDto>>(allBranches);
            }
            else
            {
                // Otherwise, return only branches assigned to the user
                var userBranches = await _unitOfWork.BranchRepository.GetUserBranchesAsync(userId);
                return _mapper.Map<List<BranchDto>>(userBranches);
            }
        }

        public async Task<BranchDto> GetCurrentBranchAsync()
        {
            var branchId = _branchContext.GetCurrentBranchId();
            var branch = await _unitOfWork.BranchRepository.GetByIdAsync(branchId);
            
            if (branch == null || !branch.IsActive)
            {
                var activeBranches = await _unitOfWork.BranchRepository.GetActiveBranchesAsync();
                branch = activeBranches.FirstOrDefault();
                
                if (branch != null)
                {
                    _branchContext.SetCurrentBranchId(branch.Id);
                }
            }
            
            return branch == null ? null : _mapper.Map<BranchDto>(branch);
        }

        public async Task<BranchDto> ValidateAndSetCurrentBranchAsync(int branchId, string userId, bool isSystemAdmin)
        {
            var branch = await _unitOfWork.BranchRepository.GetByIdAsync(branchId);
            if (branch == null || !branch.IsActive)
            {
                return null;
            }

            if (isSystemAdmin)
            {
                // Admin can set any branch
                _branchContext.SetCurrentBranchId(branchId);
                return _mapper.Map<BranchDto>(branch);
            }
            else
            {
                // Check if user has access to this branch
                var userBranches = await _unitOfWork.BranchRepository.GetUserBranchesAsync(userId);
                if (userBranches.Any() && !userBranches.Any(b => b.Id == branchId))
                {
                    return null; // User doesn't have access to this branch
                }
                
                // Set user default branch and context
                await _unitOfWork.BranchRepository.SetUserDefaultBranchAsync(userId, branchId);
                await _unitOfWork.SaveChangesAsync();
                _branchContext.SetCurrentBranchId(branchId);
                
                return _mapper.Map<BranchDto>(branch);
            }
        }

        // CRUD operations
        public async Task<BranchDto> CreateBranchAsync(CreateBranchDto createDto, string createdBy)
        {
            // Validate business rules - only check Name uniqueness
            if (await _unitOfWork.BranchRepository.ExistsByNameAsync(createDto.Name))
            {
                throw new InvalidOperationException($"Branch with name '{createDto.Name}' already exists.");
            }

            // Create the branch entity
            var branch = new Branch
            {
                Name = createDto.Name,
                Code = createDto.Code,
                Address = createDto.Address,
                Phone = createDto.Phone,
                Email = createDto.Email,
                Description = createDto.Description,
                IsActive = createDto.IsActive,
                ManagerId = createDto.ManagerId,
                CreatedBy = createdBy,
                CreatedAt = DateTime.UtcNow
            };

            var createdBranch = await _unitOfWork.BranchRepository.AddAsync(branch);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<BranchDto>(createdBranch);
        }

        public async Task<BranchDto> UpdateBranchAsync(int id, UpdateBranchDto updateDto, string updatedBy)
        {
            var existingBranch = await _unitOfWork.BranchRepository.GetByIdAsync(id);
            if (existingBranch == null)
            {
                throw new InvalidOperationException($"Branch with ID '{id}' not found.");
            }

            // Validate business rules - only check Name uniqueness
            if (await _unitOfWork.BranchRepository.ExistsByNameAsync(updateDto.Name, id))
            {
                throw new InvalidOperationException($"Branch with name '{updateDto.Name}' already exists.");
            }

            // Update the branch properties
            existingBranch.Name = updateDto.Name;
            existingBranch.Code = updateDto.Code;
            existingBranch.Address = updateDto.Address;
            existingBranch.Phone = updateDto.Phone;
            existingBranch.Email = updateDto.Email;
            existingBranch.Description = updateDto.Description;
            existingBranch.IsActive = updateDto.IsActive;
            existingBranch.ManagerId = updateDto.ManagerId;
            existingBranch.LastModifiedBy = updatedBy;
            existingBranch.LastModifiedAt = DateTime.UtcNow;

            var updatedBranch = await _unitOfWork.BranchRepository.UpdateAsync(existingBranch);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<BranchDto>(updatedBranch);
        }

        public async Task<bool> DeleteBranchAsync(int id, string deletedBy)
        {
            var existingBranch = await _unitOfWork.BranchRepository.GetByIdAsync(id);
            if (existingBranch == null)
            {
                return false;
            }

            // Check if branch has dependencies that prevent deletion
            // This is a business rule - you might want to check for related data
            
            existingBranch.IsDeleted = true;
            existingBranch.DeletedAt = DateTime.UtcNow;
            existingBranch.DeletedBy = deletedBy;

            await _unitOfWork.BranchRepository.UpdateAsync(existingBranch);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RestoreBranchAsync(int id, string restoredBy)
        {
            // Get the deleted branch
            var deletedBranch = await _unitOfWork.BranchRepository.GetByIdIncludingDeletedAsync(id);
            if (deletedBranch == null || !deletedBranch.IsDeleted)
            {
                return false;
            }

            // Check if there's already an active branch with the same name
            if (await _unitOfWork.BranchRepository.ExistsByNameAsync(deletedBranch.Name))
            {
                throw new InvalidOperationException($"Cannot restore branch. Another active branch with name '{deletedBranch.Name}' already exists.");
            }

            var success = await _unitOfWork.BranchRepository.RestoreAsync(id, restoredBy);
            if (success)
            {
                await _unitOfWork.SaveChangesAsync();
            }
            return success;
        }
    }
} 