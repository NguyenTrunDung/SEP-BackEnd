using HOMMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HOMMS.Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for menu operations
    /// </summary>
    public interface IMenuRepository : IRepository<Menu, int>
    {
        /// <summary>
        /// Gets menus for a specific branch on a specific date
        /// </summary>
        /// <param name="branchId">Branch ID</param>
        /// <param name="date">Date to filter</param>
        /// <returns>Menus matching the criteria</returns>
        Task<IEnumerable<Menu>> GetMenusByBranchAndDateAsync(int branchId, DateTime date);
        
        /// <summary>
        /// Gets all active menus across all branches for a specific date
        /// This method bypasses the branch filter
        /// </summary>
        /// <param name="date">Date to filter</param>
        /// <returns>All active menus for the date grouped by branch</returns>
        Task<Dictionary<Branch, IEnumerable<Menu>>> GetMenusForAllBranchesAsync(DateTime date);
        
        /// <summary>
        /// Gets menu with its menu details
        /// </summary>
        /// <param name="menuId">Menu ID</param>
        /// <returns>Menu with details</returns>
        Task<Menu> GetMenuWithDetailsAsync(int menuId);
        
        /// <summary>
        /// Gets all menus with their details for a specific branch on a specific date
        /// </summary>
        /// <param name="branchId">Branch ID</param>
        /// <param name="date">Date to filter</param>
        /// <returns>Menus with their details</returns>
        Task<IEnumerable<Menu>> GetMenusWithDetailsByBranchAndDateAsync(int branchId, DateTime date);
        
        /// <summary>
        /// Gets menus for a specific branch on a specific date and time of day
        /// </summary>
        /// <param name="branchId">Branch ID</param>
        /// <param name="date">Date to filter</param>
        /// <param name="timeOfDay">Time of day to filter</param>
        /// <returns>Menus matching the criteria</returns>
        Task<IEnumerable<Menu>> GetMenusByBranchDateAndTimeOfDayAsync(int branchId, DateTime date, string timeOfDay);
        
        /// <summary>
        /// Gets menu details organized by food category for a specific menu
        /// </summary>
        /// <param name="menuId">Menu ID</param>
        /// <returns>Dictionary of food categories with their menu details</returns>
        Task<Dictionary<FoodCategory, IEnumerable<MenuDetail>>> GetMenuDetailsByCategoryAsync(int menuId);
    }
} 