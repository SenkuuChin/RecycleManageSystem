using System;
using System.Collections.Generic;
using System.Text;

namespace RecycleSystem.Data.Data.RoleManageDTO
{
    public class RoleOutput
    {
        public int Id { get; set; }
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
        public bool? DelFlag { get; set; }
        public DateTime? AddTime { get; set; }
    }
}
