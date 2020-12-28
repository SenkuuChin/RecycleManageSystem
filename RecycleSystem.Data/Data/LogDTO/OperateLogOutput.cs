using System;
using System.Collections.Generic;
using System.Text;

namespace RecycleSystem.Data.Data.LogDTO
{
    public class OperateLogOutput
    {
        public int Id { get; set; }
        public string Operator { get; set; }
        public string Info { get; set; }
        public DateTime? AddTime { get; set; }
    }
}
