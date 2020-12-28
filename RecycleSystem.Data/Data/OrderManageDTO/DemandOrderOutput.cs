using System;
using System.Collections.Generic;
using System.Text;

namespace RecycleSystem.Data.Data.OrderManageDTO
{
    public class DemandOrderOutput
    {
        public int Id { get; set; }
        public string Oid { get; set; }
        public string Enterpriser { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Num { get; set; }
        public string Unit { get; set; }
        public string Receiver { get; set; }
        public string EnterpriseName { get; set; }
        public DateTime? AddTime { get; set; }
        public int? Status { get; set; }
        public bool DelFlag { get; set; }
        public string CategoryId { get; set; }
        public string Reason { get; set; }
    }
}
