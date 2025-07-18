using System.ComponentModel.DataAnnotations;

namespace HOMMS.Domain.Dtos
{
    /// <summary>
    /// DTO for updating order payment status after VNPay payment processing
    /// </summary>
    public class UpdatePaymentStatusDto
    {
        /// <summary>
        /// Whether the payment was successful
        /// </summary>
        [Required]
        public bool IsPaid { get; set; }

        /// <summary>
        /// Updated order status (e.g., "Confirmed" for successful payment, "Cancelled" for failed payment)
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Status { get; set; } = string.Empty;
    }
} 