using System;
using System.Collections.Generic;

#nullable disable

namespace RecycleSystem.DataEntity.Entities
{
    public partial class RevenueBill
    {
        public int Id { get; set; }
        public string Zid { get; set; }
        public string Oid { get; set; }
        public string Name { get; set; }
        public string Num { get; set; }
        public string Unit { get; set; }
        public DateTime? AddTime { get; set; }
        public decimal? Money { get; set; }
    }
}
