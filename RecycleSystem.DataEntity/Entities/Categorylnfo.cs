using System;
using System.Collections.Generic;

#nullable disable

namespace RecycleSystem.DataEntity.Entities
{
    public partial class Categorylnfo
    {
        public int Id { get; set; }
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
        public decimal? CurrentPrice { get; set; }
        public string Unit { get; set; }
        public bool? DelFlag { get; set; }
        public DateTime? AddTime { get; set; }
    }
}
