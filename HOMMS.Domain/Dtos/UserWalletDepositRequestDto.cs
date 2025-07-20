using System.ComponentModel.DataAnnotations;

namespace HOMMS.Domain.Dtos
{
    public class UserWalletDepositRequestDto
    {
        [Required]
        public string UserId { get; set; } = string.Empty;
        [Required]
        [Range(1, long.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public long Amount { get; set; }
        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
    }
} 