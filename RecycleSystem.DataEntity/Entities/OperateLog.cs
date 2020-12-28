using System;
using System.Collections.Generic;

#nullable disable

namespace RecycleSystem.DataEntity.Entities
{
    public partial class OperateLog
    {
        public int Id { get; set; }
        public string OperatorId { get; set; }
        public string Info { get; set; }
        public DateTime? AddTime { get; set; }
    }
}
