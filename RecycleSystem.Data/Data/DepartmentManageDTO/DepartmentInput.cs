using System;
using System.Collections.Generic;
using System.Text;

namespace RecycleSystem.Data.Data.DepartmentManageDTO
{
    public class DepartmentInput
    {
        public string DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string LeaderId { get; set; }
        public string Description { get; set; }
        public string ParentId { get; set; }
    }
}
