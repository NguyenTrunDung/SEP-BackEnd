using HOMMS.Domain.Dtos;
using HOMMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Application.Interfaces
{
    public interface IRevenueService
    {


        /// <summary>
        /// Gets total revenue of a branch for a specific day
        /// </summary>
        /// <param name="branchId">Branch ID</param>
        /// <param name="date">Date</param> 
        /// <returns>total revenue of a branch for a specific day</returns>
        Task<IEnumerable<RevenueDto>> GetRevenueByDayAsync(int branchId, DateTime date);




        /// <summary>
        /// Gets total revenue of a branch for a specific week
        /// </summary>
        /// <param name="branchId">Branch ID</param>
        /// <param name="date">Date</param> 
        /// <returns>total revenue of a branch for a specific week</returns>
        Task<IEnumerable<RevenueDto>> GetRevenueByWeekAsync(int branchId, DateTime date);




        /// <summary>
        /// Gets total revenue of a branch for a specific month
        /// </summary>
        /// <param name="branchId">Branch ID</param>
        /// <param name="date">Date</param> 
        /// <returns>total revenue of a branch for a specific month</returns>
        Task<IEnumerable<RevenueDto>> GetRevenueByMonthAsync(int branchId, DateTime date);




        /// <summary>
        /// Gets total revenue of a branch for a specific year
        /// </summary>
        /// <param name="branchId">Branch ID</param>
        /// <param name="date">Date</param> 
        /// <returns>total revenue of a branch for a specific year</returns>
        Task<IEnumerable<RevenueDto>> GetRevenueByYearAsync(int branchId, DateTime date);





    }
}
