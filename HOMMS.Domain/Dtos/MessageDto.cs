using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Domain.Dtos
{
    public class MessageDto
    {
        public String To {  get; set; }
        public String Subject { get; set; }
        public String Body {  get; set; }
        public MessageDto(string to, string subject, string body)
        {
            To = to;
            Subject = subject;
            Body = body;
        }
    }
}
