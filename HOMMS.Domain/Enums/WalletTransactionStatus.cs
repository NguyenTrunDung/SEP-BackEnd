namespace HOMMS.Domain.Enums
{
    /// <summary>
    /// Represents the status of wallet transactions
    /// </summary>
    public enum WalletTransactionStatus
    {
        /// <summary>
        /// Transaction is pending processing
        /// </summary>
        Pending = 1,
        
        /// <summary>
        /// Transaction has been completed successfully
        /// </summary>
        Completed = 2,
        
        /// <summary>
        /// Transaction failed to process
        /// </summary>
        Failed = 3,
        
        /// <summary>
        /// Transaction has been cancelled
        /// </summary>
        Cancelled = 4,
        
        /// <summary>
        /// Transaction is being processed
        /// </summary>
        Processing = 5,
        
        /// <summary>
        /// Transaction has been reversed
        /// </summary>
        Reversed = 6
    }
} 