using HOMMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Infrastructure.Repositories.Interfaces
{
    public interface IRevenueRepository : IRepository<Order, int>
    {

        /// <summary>
        /// Gets total revenue of a branch for a specific day
        /// </summary>
        /// <param name="branchId">Branch ID</param>
        /// <param name="date">Date</param> 
        /// <returns>total revenue of a branch for a specific day</returns>
        Task<IEnumerable<Order>> GetRevenueByDayAsync(int branchId, DateTime date);

        /// <summary>
        /// Gets total revenue of a branch for a specific week
        /// </summary>
        /// <param name="branchId">Branch ID</param>
        /// <param name="date">Date</param> 
        /// <returns>total revenue of a branch for a specific week</returns>

        Task<IEnumerable<Order>> GetRevenueByWeekAsync(int branchId, DateTime date);

        /// <summary>
        /// Gets total revenue of a branch for a specific Month
        /// </summary>
        /// <param name="branchId">Branch ID</param>
        /// <param name="date">Date</param> 
        /// <returns>total revenue of a branch for a specific Month</returns>
        Task<IEnumerable<Order>> GetRevenueByMonthAsync(int branchId, DateTime date);

        /// <summary>
        /// Gets total revenue of a branch for a specific year 
        /// </summary>
        /// <param name="branchId">Branch ID</param>
        /// <param name="date">Date</param> 
        /// <returns>total revenue of a branch for a specific year</returns>
        Task<IEnumerable<Order>> GetRevenueByYearAsync(int branchId, DateTime date);


    }
}
