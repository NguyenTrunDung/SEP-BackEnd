namespace HOMMS.Domain.Enums
{
    /// <summary>
    /// Represents the severity level of a disease category
    /// Used to determine strictness of dietary requirements
    /// </summary>
    public enum DiseaseSeverityLevel
    {
        /// <summary>
        /// Low severity - dietary guidelines are advisory
        /// </summary>
        Low = 1,
        
        /// <summary>
        /// Medium severity - dietary guidelines should be followed
        /// </summary>
        Medium = 2,
        
        /// <summary>
        /// High severity - strict dietary requirements must be enforced
        /// </summary>
        High = 3,
        
        /// <summary>
        /// Critical severity - any deviation could be life-threatening
        /// </summary>
        Critical = 4
    }
} 