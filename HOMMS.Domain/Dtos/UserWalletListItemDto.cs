namespace HOMMS.Domain.Dtos
{
    public class UserWalletListItemDto
    {
        public string UserId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public long Balance { get; set; }
    }
} 