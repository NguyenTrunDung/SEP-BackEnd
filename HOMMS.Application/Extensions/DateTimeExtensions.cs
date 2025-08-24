using HOMMS.Common.Helpers;

namespace HOMMS.Application.Extensions
{
    /// <summary>
    /// Extension methods for DateTime operations in services
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Set audit timestamps when creating an entity
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="entity">Entity to update</param>
        /// <param name="userId">User ID performing the action</param>
        /// <returns>Updated entity</returns>
        public static T SetCreatedAudit<T>(this T entity, string? userId = null) 
            where T : HOMMS.Domain.Entities.Base.IAuditableEntity
        {
            entity.CreatedAt = DateTime.UtcNow;
            entity.CreatedBy = userId;
            return entity;
        }
        
        /// <summary>
        /// Set audit timestamps when updating an entity
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="entity">Entity to update</param>
        /// <param name="userId">User ID performing the action</param>
        /// <returns>Updated entity</returns>
        public static T SetModifiedAudit<T>(this T entity, string? userId = null) 
            where T : HOMMS.Domain.Entities.Base.IAuditableEntity
        {
            entity.LastModifiedAt = DateTime.UtcNow;
            entity.LastModifiedBy = userId;
            return entity;
        }
        
        /// <summary>
        /// Set deleted audit timestamps for soft delete
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="entity">Entity to soft delete</param>
        /// <param name="userId">User ID performing the action</param>
        /// <returns>Updated entity</returns>
        public static T SetDeletedAudit<T>(this T entity, string? userId = null) 
            where T : HOMMS.Domain.Entities.Base.ISoftDeletable
        {
            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;
            entity.DeletedBy = userId;
            return entity;
        }
        
        /// <summary>
        /// Filter entities by Vietnam date range (converts to UTC for database query)
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="query">Query to filter</param>
        /// <param name="vietnamDate">Date in Vietnam timezone</param>
        /// <param name="dateSelector">Selector for date field</param>
        /// <returns>Filtered query</returns>
        public static IQueryable<T> FilterByVietnamDate<T>(
            this IQueryable<T> query, 
            DateTime vietnamDate, 
            Func<T, DateTime> dateSelector)
        {
            var startOfDayUtc = TimeZoneHelper.GetStartOfDayUtc(vietnamDate);
            var endOfDayUtc = TimeZoneHelper.GetEndOfDayUtc(vietnamDate);
            
            return query.Where(x => 
                dateSelector(x) >= startOfDayUtc && 
                dateSelector(x) <= endOfDayUtc);
        }
        
        /// <summary>
        /// Filter entities by Vietnam date range for nullable DateTime
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="query">Query to filter</param>
        /// <param name="vietnamDate">Date in Vietnam timezone</param>
        /// <param name="dateSelector">Selector for nullable date field</param>
        /// <returns>Filtered query</returns>
        public static IQueryable<T> FilterByVietnamDate<T>(
            this IQueryable<T> query, 
            DateTime vietnamDate, 
            Func<T, DateTime?> dateSelector)
        {
            var startOfDayUtc = TimeZoneHelper.GetStartOfDayUtc(vietnamDate);
            var endOfDayUtc = TimeZoneHelper.GetEndOfDayUtc(vietnamDate);
            
            return query.Where(x => 
                dateSelector(x) != null &&
                dateSelector(x) >= startOfDayUtc && 
                dateSelector(x) <= endOfDayUtc);
        }
        
        /// <summary>
        /// Filter entities by Vietnam date range
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="query">Query to filter</param>
        /// <param name="startDate">Start date in Vietnam timezone</param>
        /// <param name="endDate">End date in Vietnam timezone</param>
        /// <param name="dateSelector">Selector for date field</param>
        /// <returns>Filtered query</returns>
        public static IQueryable<T> FilterByVietnamDateRange<T>(
            this IQueryable<T> query, 
            DateTime startDate, 
            DateTime endDate,
            Func<T, DateTime> dateSelector)
        {
            var startOfRangeUtc = TimeZoneHelper.GetStartOfDayUtc(startDate);
            var endOfRangeUtc = TimeZoneHelper.GetEndOfDayUtc(endDate);
            
            return query.Where(x => 
                dateSelector(x) >= startOfRangeUtc && 
                dateSelector(x) <= endOfRangeUtc);
        }
    }
}
