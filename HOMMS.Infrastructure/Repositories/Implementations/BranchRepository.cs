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
            return await DbSet.Where(b => b.IsActive)
                             .OrderBy(b => b.Name)
                             .ToListAsync();
        }
        
        /// <inheritdoc/>
        public async Task<IEnumerable<Branch>> GetUserBranchesAsync(string userId)
        {
            return await DbContext.Set<BranchUser>()
                                 .Where(bu => bu.UserId == userId)
                                 .Select(bu => bu.Branch)
                                 .Where(b => b.IsActive)
                                 .OrderBy(b => b.Name)
                                 .ToListAsync();
        }
        
        /// <inheritdoc/>
        public async Task<Branch> GetUserDefaultBranchAsync(string userId)
        {
            var branchUser = await DbContext.Set<BranchUser>()
                                          .Where(bu => bu.UserId == userId && bu.IsDefault)
                                          .FirstOrDefaultAsync();
                                      
            if (branchUser == null)
            {
                // If no default is set, get the first one
                branchUser = await DbContext.Set<BranchUser>()
                                          .Where(bu => bu.UserId == userId)
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
    }
} 