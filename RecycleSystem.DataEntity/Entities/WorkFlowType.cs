using System;
using System.Collections.Generic;

#nullable disable

namespace RecycleSystem.DataEntity.Entities
{
    public partial class WorkFlowType
    {
        public int Id { get; set; }
        public string TypeName { get; set; }
        public bool? DelFlag { get; set; }
        public DateTime? AddTime { get; set; }
    }
}
