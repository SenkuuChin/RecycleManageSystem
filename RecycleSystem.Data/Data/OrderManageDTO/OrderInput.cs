using System;
using System.Collections.Generic;
using System.Text;

namespace RecycleSystem.Data.Data.OrderManageDTO
{
    public class OrderInput
    {
        public int Id { get; set; }
        public string InstanceId { get; set; }
        public string EnterpriseId { get; set; }
        public string Name { get; set; }
        public string CategoryId { get; set; }
        public string Num { get; set; }
        public string ReceiverId { get; set; }
        public DateTime? AddTime { get; set; }
        public string Unit { get; set; }
        public int? Status { get; set; }
        public string Url { get; set; }


        //用来记录操作人
        public string OperationID { get; set; }
    }
}
