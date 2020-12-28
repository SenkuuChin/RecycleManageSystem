using System;
using System.Collections.Generic;

#nullable disable

namespace RecycleSystem.DataEntity.Entities
{
    public partial class DepartmentInfo
    {
        public int Id { get; set; }
        public string DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string LeaderId { get; set; }
        public string Description { get; set; }
        public string ParentId { get; set; }
        public DateTime? AddTime { get; set; }
    }
}
