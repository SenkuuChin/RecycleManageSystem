using System;
using System.Collections.Generic;
using System.Text;

namespace RecycleSystem.Data.Data.UserManageDTO
{
    public class UserOutput
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public bool? Gender { get; set; }
        public int? UserTypeId { get; set; }
        public string UserTypeName { get; set; }
        public string Tel { get; set; }
        public string Email { get; set; }
        public string DepartmentId { get; set; }
        public string Token { get; set; }
        public string EnterpriseName { get; set; }
        public bool? DelFlag { get; set; }
        public DateTime? AddTime { get; set; }

        public string DepartmentName { get; set; }
        public IEnumerable<string> RoleName { get; set; }
    }
}
