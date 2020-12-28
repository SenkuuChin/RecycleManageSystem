using System;
using System.Collections.Generic;

#nullable disable

namespace RecycleSystem.DataEntity.Entities
{
    public partial class PowerInfo
    {
        public int Id { get; set; }
        public string PowerId { get; set; }
        public string PowerName { get; set; }
        public string Description { get; set; }
        public string ActionUrl { get; set; }
        public string ParentId { get; set; }
    }
}
