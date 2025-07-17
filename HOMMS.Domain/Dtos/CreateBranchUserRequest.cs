using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOMMS.Domain.Dtos
{
    public class CreateBranchUserRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string? PhoneNumber { get; set; }
        public string Password { get; set; }
        public int BranchId { get; set; }
        public int BranchRoleId { get; set; }
    }
    public class UpdateUserRequest
    {
        public string UserId { get; set; }
        public int BranchId { get; set; }
        public int BranchRoleId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public bool IsActive { get; set; }
    }

    public class UserDto
    {
        public string UserId { get; set; }
        public string FirstName { get; set; } = default!;  
        public string LastName { get; set; } = default!;
        public string FullName { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string UserName { get; set; } = default!;
        public int BranchId { get; set; }
        public int BranchRoleId { get; set; }
        public string BranchRoleName { get; set; }
        public bool IsActive { get; set; }

    }
}
