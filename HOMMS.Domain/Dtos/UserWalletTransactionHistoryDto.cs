using System.Collections.Generic;

namespace HOMMS.Domain.Dtos
{
    public class UserWalletTransactionHistoryDto
    {
        public IEnumerable<UserWalletTransactionInfoDto> Transactions { get; set; } = new List<UserWalletTransactionInfoDto>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
} 