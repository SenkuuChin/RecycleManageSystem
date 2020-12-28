using System;
using System.Collections.Generic;
using System.Text;

namespace RecycleSystem.Data.Data.FinanceDTO
{
    public class RevenueOutput
    {
        public int Id { get; set; }
        public string Zid { get; set; }
        public string Oid { get; set; }
        public string Name { get; set; }
        public string Num { get; set; }
        public string Unit { get; set; }
        public decimal? Money { get; set; }
        public DateTime? AddTime { get; set; }
    }
}
