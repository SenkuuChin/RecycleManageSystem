using System;
using System.Collections.Generic;
using System.Text;

namespace RecycleSystem.Data.Data.LoginDTO
{
    public class LoginInput
    {
        public string UserId { get; set; }
        public string Password { get; set; }
        public string VerifyCode { get; set; }
        public string IP { get; set; }
        public string BrowerInfo { get; set; }
        public string OSVersion { get; set; }
    }
}
