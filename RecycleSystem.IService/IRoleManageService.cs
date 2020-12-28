using RecycleSystem.Data.Data.RoleManageDTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace RecycleSystem.IService
{
    public interface IRoleManageService
    {
        IEnumerable<RoleOutput> GetRoles(int page, int limit, out int count, string queryInfo);
        RoleOutput GetRoleById(int id);
        bool AddRole(RoleInput roleInput,out string message);
        bool Update(RoleInput roleInput,out string message);
        bool BanRole(string roleId,out string message);
    }
}
