using System;
using System.Collections.Generic;
using System.Text;

namespace RecycleSystem.Data.Data.LogDTO
{
    public class LoginLogOutput
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Ip { get; set; }
        public string SystemInfo { get; set; }
        public string BrowserInfo { get; set; }
        public bool? IsLoginSuccess { get; set; }
        public DateTime? HappenTime { get; set; }
    }
}
