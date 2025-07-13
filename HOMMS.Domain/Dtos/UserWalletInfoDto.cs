namespace HOMMS.Domain.Dtos
{
    /// <summary>
    /// DTO trả về thông tin ví người dùng cho API mới
    /// </summary>
    public class UserWalletInfoDto
    {
        public string UserId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public decimal? Balance { get; set; }
        public string? CustomerCode { get; set; }
        public bool IsCustomerAccount { get; set; }
        public bool IsCustomerEnabled { get; set; }
    }
} 