using System;
using System.Collections.Generic;

#nullable disable

namespace RecycleSystem.DataEntity.Entities
{
    public partial class OrderInfo
    {
        public int Id { get; set; }
        public string InstanceId { get; set; }
        public string EnterpriseId { get; set; }
        public string Name { get; set; }
        public string OriginalOrderId { get; set; }
        public string CategoryId { get; set; }
        public string Num { get; set; }
        public string ReceiverId { get; set; }
        public DateTime? AddTime { get; set; }
        public DateTime? FinishedTime { get; set; }
        public string Unit { get; set; }
        public int? Status { get; set; }
        public string Url { get; set; }
    }
}
