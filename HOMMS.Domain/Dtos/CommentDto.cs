using HOMMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Domain.Dtos
{
    public class CommentDto
    {

        public int Id { get; set; }

        public int? Star { get; set; }

       
        public string? CommentLines { get; set; }

        public int? FoodId { get; set; }

        public int? OrderId { get; set; }
        

        public string UserId { get; set; }
       

        public int BranchId { get; set; }

        public string Image {  get; set; }

        public DateTime CreatedAt { get; set; }


    }


    public class CRUDCommentDto
    {

        

        public int? Star { get; set; }


        public string? CommentLines { get; set; }

        public int? FoodId { get; set; }

        public int? OrderId { get; set; }


        public string UserId { get; set; }


        public int BranchId { get; set; }



    }


}
