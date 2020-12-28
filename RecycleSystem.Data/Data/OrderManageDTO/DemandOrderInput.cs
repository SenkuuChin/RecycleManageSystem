using System;
using System.Collections.Generic;
using System.Text;

namespace RecycleSystem.Data.Data.OrderManageDTO
{
   public class DemandOrderInput
    {
        public int Id { get; set; }
        public string Oid { get; set; }
        public string EnterpriseId { get; set; }
        public string Name { get; set; }
        public string CategoryId { get; set; }
        public string Num { get; set; }
        public string Unit { get; set; }
        public string UserId { get; set; }
        public DateTime? AddTime { get; set; }
        public int? Status { get; set; }

        //申请撤销和审批用
        public string Reason { get; set; }
        public int Decide { get; set; }
        public string BackContent { get; set; }
    }
}
