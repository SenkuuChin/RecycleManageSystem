using RecycleSystem.Data.Data.UserManageDTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace RecycleSystem.IService
{
    public interface IUserManageService
    {
        IEnumerable<UserOutput> GetUsers(int page,int limit,out int count,string queryInfo);
        IEnumerable<UserOutput> GetUsers();
        bool ModifyUserInfo(UserInput input,out string message);
        bool BanUser(int Id, out string message);
        bool RecoverPassword(IEnumerable<int> Id, out string message);
        UserOutput GetUserById(int Id);
        IEnumerable<UserTypeOutput> GetUserType();
        bool AddUser(UserInput userInput,out string message);
        bool MutipleImport(IEnumerable<UserInput> userInfos, out string msg);
    }
}
