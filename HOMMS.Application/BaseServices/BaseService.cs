using HOMMS.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Application.BaseServices
{
    public abstract class BaseService
    {
        protected readonly IBranchContext BranchContext;
        protected BaseService(IBranchContext branchContext)
        {
            BranchContext = branchContext;
        }
        protected int EnsureBranchId(int? branchId)
        {
            return branchId.HasValue && branchId.Value != 0
                ? branchId.Value
                : BranchContext.GetCurrentBranchId();
        }
        protected int EnsureBranchId2()
        {
            return BranchContext.GetCurrentBranchId();
        }

        /// <summary>
        /// Generic method to create an entity with auto-incremented sort value and image upload.
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <typeparam name="TDto">DTO type</typeparam>
        /// <param name="dto">DTO to create from</param>
        /// <param name="image">Image file</param>
        /// <param name="webRootPath">Web root path</param>
        /// <param name="getMaxSortValueByBranchAsync">Func to get max sort value for branch</param>
        /// <param name="addAndSaveAsync">Func to add and save entity</param>
        /// <param name="createEntity">Func to create entity from dto, imagePath, newSort, branchId</param>
        /// <param name="createDto">Func to create DTO from entity</param>
        /// <returns>Created DTO</returns>
        protected async Task<TDto> CreateWithAutoSortAsync<TEntity, TDto>(
            TDto dto,
            IFormFile? image,
            string webRootPath,
            int? branchId,
            Func<int, Task<int>> getMaxSortValueByBranchAsync,
            Func<TEntity, Task> addAndSaveAsync,
            Func<TDto, string?, int, int, TEntity> createEntity,
            Func<TEntity, TDto> createDto)
        {
            string? imagePath = null;
            if (image != null)
            {
                imagePath = await HOMMS.Common.Helpers.UploadHandler.SaveImageAsync(image, webRootPath, "uploads");
            }

            int resolvedBranchId = EnsureBranchId(branchId);
            var maxSort = await getMaxSortValueByBranchAsync(resolvedBranchId);
            var newSort = maxSort + 1;

            var entity = createEntity(dto, imagePath, newSort, resolvedBranchId);
            await addAndSaveAsync(entity);
            return createDto(entity);
        }
    }
}
