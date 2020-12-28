using System;
using System.Collections.Generic;

#nullable disable

namespace RecycleSystem.DataEntity.Entities
{
    public partial class RoleInfo
    {
        public int Id { get; set; }
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
        public bool? DelFlag { get; set; }
        public DateTime? DelTime { get; set; }
        public DateTime? AddTime { get; set; }
    }
}
