using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Domain.Dtos
{
    public class VnPayReturnDto
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public int? OrderId { get; set; }
        public string TransactionId { get; set; }
        public long? Amount { get; set; }
        public string ResponseCode { get; set; }

        public VnPayReturnDto(bool isSuccess, string message)
        {
            IsSuccess = isSuccess;
            Message = message;
        }

        public VnPayReturnDto(bool isSuccess, string message, int? orderId, string transactionId, long? amount, string responseCode)
        {
            IsSuccess = isSuccess;
            Message = message;
            OrderId = orderId;
            TransactionId = transactionId;
            Amount = amount;
            ResponseCode = responseCode;
        }
    }
}
