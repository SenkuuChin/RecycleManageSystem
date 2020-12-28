using RecycleSystem.Data.Data.LoginDTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace RecycleSystem.IService
{
    public interface IAccountService
    {
        LoginOutput Login(LoginInput loginInput);
    }
}
