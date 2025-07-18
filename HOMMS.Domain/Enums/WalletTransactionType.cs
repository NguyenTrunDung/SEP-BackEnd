namespace HOMMS.Domain.Enums
{
    /// <summary>
    /// Represents the types of wallet transactions (simplified for POC)
    /// </summary>
    public enum WalletTransactionType
    {
        /// <summary>
        /// Money added to wallet by admin/manager
        /// </summary>
        Credit = 1,
        
        /// <summary>
        /// Money deducted for order payment
        /// </summary>
        OrderPayment = 2,
        
        /// <summary>
        /// Money refunded to wallet from cancelled order
        /// </summary>
        Refund = 3,
        
        /// <summary>
        /// Manual adjustment by admin/manager (can be positive or negative)
        /// </summary>
        Adjustment = 4
    }
} 