using HOMMS.Domain.Entities;
using HOMMS.Infrastructure.Data;
using HOMMS.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HOMMS.Infrastructure.Repositories.Implementations
{
    /// <summary>
    /// Repository implementation for branch (tenant) operations
    /// </summary>
    public class BranchRepository : Repository<Branch, int>, IBranchRepository
    {
        public BranchRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
        
        /// <inheritdoc/>
        public async Task<IEnumerable<Branch>> GetActiveBranchesAsync()
        {
            return await DbSet.Where(b => b.IsActive && !b.IsDeleted)
                             .OrderBy(b => b.Name)
                             .ToListAsync();
        }
        
        /// <inheritdoc/>
        public async Task<IEnumerable<Branch>> GetUserBranchesAsync(string userId)
        {
            return await DbContext.Set<BranchUser>()
                                 .Where(bu => bu.UserId == userId)
                                 .Select(bu => bu.Branch)
                                 .Where(b => b.IsActive && !b.IsDeleted)
                                 .OrderBy(b => b.Name)
                                 .ToListAsync();
        }
        
        /// <inheritdoc/>
        public async Task<Branch> GetUserDefaultBranchAsync(string userId)
        {
            var branchUser = await DbContext.Set<BranchUser>()
                                          .Include(bu => bu.Branch)
                                          .Where(bu => bu.UserId == userId && bu.IsDefault && !bu.Branch.IsDeleted)
                                          .FirstOrDefaultAsync();
                                      
            if (branchUser == null)
            {
                // If no default is set, get the first one
                branchUser = await DbContext.Set<BranchUser>()
                                          .Include(bu => bu.Branch)
                                          .Where(bu => bu.UserId == userId && !bu.Branch.IsDeleted)
                                          .OrderBy(bu => bu.BranchId)
                                          .FirstOrDefaultAsync();
            }
            
            return branchUser?.Branch;
        }
        
        /// <inheritdoc/>
        public async Task<bool> AddUserToBranchAsync(string userId, int branchId, bool isDefault = false)
        {
            try
            {
                // Check if the relationship already exists
                var existingBranchUser = await DbContext.Set<BranchUser>()
                                                     .FirstOrDefaultAsync(bu => bu.UserId == userId && bu.BranchId == branchId);
                                                     
                if (existingBranchUser != null)
                {
                    // Already exists, update IsDefault if needed
                    if (isDefault && !existingBranchUser.IsDefault)
                    {
                        await SetUserDefaultBranchAsync(userId, branchId);
                    }
                    return true;
                }
                
                // If this is the first branch or should be default, clear other defaults
                if (isDefault)
                {
                    var existingDefaults = await DbContext.Set<BranchUser>()
                                                       .Where(bu => bu.UserId == userId && bu.IsDefault)
                                                       .ToListAsync();
                                                   
                    foreach (var bu in existingDefaults)
                    {
                        bu.IsDefault = false;
                    }
                }
                
                // Add the new relationship
                var branchUser = new BranchUser
                {
                    UserId = userId,
                    BranchId = branchId,
                    IsDefault = isDefault
                };
                
                DbContext.Set<BranchUser>().Add(branchUser);
                await DbContext.SaveChangesAsync();
                
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        
        /// <inheritdoc/>
        public async Task<bool> RemoveUserFromBranchAsync(string userId, int branchId)
        {
            try
            {
                var branchUser = await DbContext.Set<BranchUser>()
                                             .FirstOrDefaultAsync(bu => bu.UserId == userId && bu.BranchId == branchId);
                                         
                if (branchUser == null)
                {
                    return false;
                }
                
                DbContext.Set<BranchUser>().Remove(branchUser);
                
                // If this was the default, set another one as default if available
                if (branchUser.IsDefault)
                {
                    var nextBranchUser = await DbContext.Set<BranchUser>()
                                                     .Where(bu => bu.UserId == userId && bu.BranchId != branchId)
                                                     .FirstOrDefaultAsync();
                                                 
                    if (nextBranchUser != null)
                    {
                        nextBranchUser.IsDefault = true;
                    }
                }
                
                await DbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        
        /// <inheritdoc/>
        public async Task<bool> SetUserDefaultBranchAsync(string userId, int branchId)
        {
            try
            {
                // Clear existing defaults
                var existingDefaults = await DbContext.Set<BranchUser>()
                                                   .Where(bu => bu.UserId == userId && bu.IsDefault)
                                                   .ToListAsync();
                                               
                foreach (var bu in existingDefaults)
                {
                    bu.IsDefault = false;
                }
                
                // Set the new default
                var branchUser = await DbContext.Set<BranchUser>()
                                             .FirstOrDefaultAsync(bu => bu.UserId == userId && bu.BranchId == branchId);
                                         
                if (branchUser == null)
                {
                    // User doesn't belong to this branch yet, add them
                    return await AddUserToBranchAsync(userId, branchId, true);
                }
                
                branchUser.IsDefault = true;
                await DbContext.SaveChangesAsync();
                
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <inheritdoc/>
        [Obsolete("Code is now optional and not used for uniqueness validation. Use ExistsByNameAsync instead.")]
        public async Task<bool> ExistsByCodeAsync(string code, int? excludeId = null)
        {
            // Handle null/empty code since it's now optional
            if (string.IsNullOrEmpty(code))
            {
                return false; // Empty/null codes don't conflict
            }
            
            var query = DbSet.Where(b => b.Code == code && !b.IsDeleted);
            
            if (excludeId.HasValue)
            {
                query = query.Where(b => b.Id != excludeId.Value);
            }
            
            return await query.AnyAsync();
        }

        /// <inheritdoc/>
        public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null)
        {
            var query = DbSet.Where(b => b.Name == name && !b.IsDeleted);
            
            if (excludeId.HasValue)
            {
                query = query.Where(b => b.Id != excludeId.Value);
            }
            
            return await query.AnyAsync();
        }

        /// <inheritdoc/>
        public async Task<Branch> GetByIdIncludingDeletedAsync(int id)
        {
            return await DbSet.IgnoreQueryFilters()
                             .FirstOrDefaultAsync(b => b.Id == id);
        }

        /// <summary>
        /// Override GetByIdAsync to exclude soft deleted branches
        /// </summary>
        public override async Task<Branch> GetByIdAsync(int id)
        {
            return await DbSet.Where(b => b.Id == id && !b.IsDeleted)
                             .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Override AddAsync to handle branch creation with proper audit trail
        /// </summary>
        public override async Task<Branch> AddAsync(Branch entity)
        {
            // Ensure BranchId is set for self-reference
            entity.BranchId = 0; // Will be set after insert
            
            DbSet.Add(entity);
            await DbContext.SaveChangesAsync();
            
            // Update BranchId to self-reference
            entity.BranchId = entity.Id;
            await DbContext.SaveChangesAsync();
            
            return entity;
        }

        /// <summary>
        /// Soft delete a branch by setting IsDeleted flag
        /// </summary>
        public override async Task<bool> DeleteAsync(int id)
        {
            var branch = await GetByIdAsync(id);
            if (branch == null)
            {
                return false;
            }

            branch.IsDeleted = true;
            branch.DeletedAt = DateTime.UtcNow;
            
            await DbContext.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Restore a soft deleted branch
        /// </summary>
        public async Task<bool> RestoreAsync(int id, string restoredBy)
        {
            var branch = await GetByIdIncludingDeletedAsync(id);
            if (branch == null || !branch.IsDeleted)
            {
                return false;
            }

            branch.IsDeleted = false;
            branch.DeletedAt = null;
            branch.DeletedBy = null;
            branch.LastModifiedAt = DateTime.UtcNow;
            branch.LastModifiedBy = restoredBy;
            
            await DbContext.SaveChangesAsync();
            return true;
        }
    }
} 