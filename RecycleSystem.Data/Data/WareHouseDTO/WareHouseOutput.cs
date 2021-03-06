﻿using System;
using System.Collections.Generic;
using System.Text;

namespace RecycleSystem.Data.Data.WareHouseDTO
{
    public class WareHouseOutput
    {
        public int Id { get; set; }
        public string InstanceId { get; set; }
        public string CategoryId { get; set; }
        public string Name { get; set; }
        public string Num { get; set; }
        public string Unit { get; set; }
        public bool? IsPress { get; set; }
        public bool? DelFlag { get; set; }
        public DateTime? AddTime { get; set; }
        public string CategoryName { get; set; }
    }
}
