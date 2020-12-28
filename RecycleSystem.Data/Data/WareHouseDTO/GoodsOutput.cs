using System;
using System.Collections.Generic;
using System.Text;

namespace RecycleSystem.Data.Data.WareHouseDTO
{
    public class GoodsOutput
    {
        public int Id { get; set; }
        public string InstanceId { get; set; }
        public string Category { get; set; }
        public string Name { get; set; }
        public string Num { get; set; }
        public string Unit { get; set; }
        public string InputUser { get; set; }
        public DateTime? AddTime { get; set; }
    }
}
