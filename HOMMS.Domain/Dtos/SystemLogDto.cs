using HOMMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Domain.Dtos
{
    public class SystemLogDto
    {
      
        public int? UserId { get; set; }

        public string? Note { get; set; }

        public DateTime Date { get; set; }

       

    }

    public class AddSystemLogDto
    {
       
        public int BranchId { get; set; }
        public int? UserId { get; set; }

        public string? Note { get; set; }

        public DateTime Date { get; set; }
    }


}
