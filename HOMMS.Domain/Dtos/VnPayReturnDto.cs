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

        public VnPayReturnDto(bool isSuccess, string message)
        {
            IsSuccess = isSuccess;
            Message = message;
        }
    }
}
