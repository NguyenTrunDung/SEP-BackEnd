namespace HOMMS.Domain.Enums
{
    /// <summary>
    /// Represents the restriction level for foods in relation to disease categories
    /// </summary>
    public enum FoodRestrictionLevel
    {
        /// <summary>
        /// Advisory - food is not recommended but can be consumed with caution
        /// </summary>
        Advisory = 1,
        
        /// <summary>
        /// Warning - food should be avoided but not strictly prohibited
        /// </summary>
        Warning = 2,
        
        /// <summary>
        /// Prohibited - food is not allowed for this disease category
        /// </summary>
        Prohibited = 3,
        
        /// <summary>
        /// Dangerous - food could cause serious health complications
        /// </summary>
        Dangerous = 4
    }
} 