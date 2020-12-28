using System;
using System.Collections.Generic;

#nullable disable

namespace RecycleSystem.DataEntity.Entities
{
    public partial class DemandOrderInfo
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
        public bool DelFlag { get; set; }
    }
}
