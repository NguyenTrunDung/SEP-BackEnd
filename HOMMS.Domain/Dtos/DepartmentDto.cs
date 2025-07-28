using HOMMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Domain.Dtos
{
    public class DepartmentDto
    {

        public int Id { get; set; }
        public string? Name { get; set; }
        public int? Sort { get; set; }
        public bool IsActive { get; set; }
        public int BranchId { get; set; }

         

    }


    public class CreateDepartmentDto
    {

      
        public string? Name { get; set; }
        public int? Sort { get; set; }
        public bool IsActive { get; set; }
        public int BranchId { get; set; }



    }


    //public class UpdateDepartmentDto: CreateDepartmentDto
    //{

    //    public int Id { get; set; }
      


    //}
}
