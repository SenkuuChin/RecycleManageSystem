using System;
using System.Collections.Generic;

#nullable disable

namespace RecycleSystem.DataEntity.Entities
{
    public partial class WorkFlowStep
    {
        public int Id { get; set; }
        public string InstanceId { get; set; }
        public int? TypeId { get; set; }
        public string ReviewerId { get; set; }
        public string BackContent { get; set; }
        public int? ReviewStatus { get; set; }
        public DateTime? ReviewTime { get; set; }
        public string NextReviewer { get; set; }
        public bool? isRead { get; set; }
    }
}
