namespace HOMMS.Domain.Enums
{
    /// <summary>
    /// Represents the payment methods available for orders (simplified for POC)
    /// </summary>
    public enum OrderPaymentMethod
    {
        /// <summary>
        /// Cash payment on delivery/pickup
        /// </summary>
        Cash = 1,
        
        /// <summary>
        /// Payment from user wallet balance
        /// </summary>
        Wallet = 2,
        
        /// <summary>
        /// Free order (promotional, staff meal, etc.)
        /// </summary>
        Free = 3,

        /// <summary>
        /// Online payment via VNPay gateway
        /// </summary>
        Vnpay = 4
    }
} 