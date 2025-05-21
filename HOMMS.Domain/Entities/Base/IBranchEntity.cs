namespace HOMMS.Domain.Entities.Base
{
    /// <summary>
    /// Interface for entities that belong to a specific branch (tenant)
    /// </summary>
    public interface IBranchEntity
    {
        /// <summary>
        /// Gets or sets the branch ID this entity belongs to
        /// </summary>
        int BranchId { get; set; }
    }
} 