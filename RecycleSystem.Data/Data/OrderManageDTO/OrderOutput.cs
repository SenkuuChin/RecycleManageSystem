using System;
using System.Collections.Generic;
using System.Text;

namespace RecycleSystem.Data.Data.OrderManageDTO
{
   public class OrderOutput
    {
        public int Id { get; set; }
        public string InstanceId { get; set; }
        public string EnterpriseID { get; set; }
        public string Name { get; set; }
        public string CategoryId { get; set; }
        public string Num { get; set; }
        public string ReceiverId { get; set; }
        public DateTime? AddTime { get; set; }
        public DateTime? FinishedTime { get; set; }
        public string Unit { get; set; }
        public int? Status { get; set; }
        public string Url { get; set; }
        public string EnterpriseName { get; set; }
        public string CategoryName { get; set; }
        public string Receiver { get; set; }
        public string OriginalOrder { get; set; }
    }
}
