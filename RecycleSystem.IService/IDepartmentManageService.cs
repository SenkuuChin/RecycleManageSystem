using RecycleSystem.Data.Data.DepartmentManageDTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace RecycleSystem.IService
{
    public interface IDepartmentManageService
    {
        IEnumerable<DepartmentOutput> GetDepartments(int page, int limit, out int count, string queryInfo);
        IEnumerable<DepartmentOutput> GetDepartments();
        DepartmentOutput GetDepartmentById(string id);
        bool UpdateDepartmentInfoById(DepartmentInput departmentInput, out string message);
        bool AddDepartment(DepartmentInput departmentInput, out string message);
    }
}
