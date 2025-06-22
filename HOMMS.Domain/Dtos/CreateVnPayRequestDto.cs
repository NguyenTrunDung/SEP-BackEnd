using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Domain.Dtos
{
    public class CreateVnPayRequestDto
    {
        public int OrderId { get; set; }
        public int Amount { get; set; }
    }
}
