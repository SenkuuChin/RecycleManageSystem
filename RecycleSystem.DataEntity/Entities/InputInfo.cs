using System;
using System.Collections.Generic;

#nullable disable

namespace RecycleSystem.DataEntity.Entities
{
    public partial class InputInfo
    {
        public int Id { get; set; }
        public string InstanceId { get; set; }
        public string CategoryId { get; set; }
        public string Name { get; set; }
        public string Num { get; set; }
        public string Unit { get; set; }
        public string InputUser { get; set; }
        public DateTime? AddTime { get; set; }
    }
}
