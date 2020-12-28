using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace RecycleSystem.Data.Data.UserManageDTO
{
    public class UserInput
    {
        public int Id { get; set; }
        [Description("用户ID")]
        public string UserId { get; set; }
        [Description("用户名")]
        public string UserName { get; set; }
        [Description("性别")]
        public bool? Gender { get; set; }
        public int UserTypeId { get; set; }
        [Description("电话")]
        public string Tel { get; set; }
        [Description("邮箱地址")]
        public string Email { get; set; }
        public string DepartmentId { get; set; }
        public string EnterpriseName { get; set; }
    }
}
