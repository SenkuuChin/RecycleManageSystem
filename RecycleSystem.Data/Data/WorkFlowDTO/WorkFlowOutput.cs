using System;
using System.Collections.Generic;
using System.Text;

namespace RecycleSystem.Data.Data.WorkFlowDTO
{
    public class WorkFlowOutput
    {
        public int Id { get; set; }
        public string InstanceId { get; set; }
        public int TypeId { get; set; }
        public string UserId { get; set; }
        public int? Status { get; set; }
        public string Reason { get; set; }
        public string CurrentReviewer { get; set; }
        public string EnterpriseName { get; set; }
        public DateTime? AddTime { get; set; }
        public string OrderID { get; set; }
        public bool isRead { get; set; }
        public string Receiver { get; set; }
    }
}
