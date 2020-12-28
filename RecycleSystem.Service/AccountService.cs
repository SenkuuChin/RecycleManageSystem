using Microsoft.EntityFrameworkCore;
using RecycleSystem.Data.Data.LoginDTO;
using RecycleSystem.DataEntity.Entities;
using RecycleSystem.IService;
using Senkuu.MaterialSystem.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RecycleSystem.Service
{
    public class AccountService : IAccountService
    {
        private readonly DbContext _dbContext;
        public AccountService(DbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public LoginOutput Login(LoginInput loginInput)
        {
            IQueryable<UserInfo> infos = _dbContext.Set<UserInfo>();
            IQueryable<UserType> types = _dbContext.Set<UserType>();
            IQueryable<DepartmentInfo> departments = _dbContext.Set<DepartmentInfo>();
            IQueryable<RoleInfo> roleInfos = _dbContext.Set<RoleInfo>();
            IQueryable<RUserRoleInfo> userRoles = _dbContext.Set<RUserRoleInfo>();
            string password = MD5Helper.EncryptString(loginInput.Password);
            UserInfo user = infos.Where(u => u.UserId == loginInput.UserId && u.Password == password && u.DelFlag == false).FirstOrDefault();
            if (user != null)
            {
                IEnumerable<string> roleName = (from a in userRoles
                                                join b in roleInfos on a.RoleId equals b.RoleId into join_a
                                                from c in join_a.DefaultIfEmpty()
                                                where a.UserId == user.UserId
                                                select new
                                                {
                                                    c.RoleName
                                                }).Select(s => s.RoleName).ToList();
                IEnumerable<string> roleId = (from a in userRoles
                                              join b in roleInfos on a.RoleId equals b.RoleId into join_a
                                              from c in join_a.DefaultIfEmpty()
                                              where a.UserId == user.UserId
                                              select new
                                              {
                                                  c.RoleId
                                              }).Select(s => s.RoleId).ToList();
                string userType = types.Where(t => t.Id == user.UserTypeId).Select(s => s.TypeName).FirstOrDefault();
                string departmentName = departments.Where(d => d.DepartmentId == user.DepartmentId).Select(s => s.DepartmentName).FirstOrDefault();
                //if (string.IsNullOrEmpty(departmentName))
                //{
                //    departmentName = "";
                //}
                LoginOutput outputs = new LoginOutput
                {
                    Id = user.Id,
                    UserTypeName = userType,
                    DepartmentId = user.DepartmentId,
                    DepartmentName = departmentName,
                    Email = user.Email,
                    Tel = user.Tel,
                    AddTime = user.AddTime,
                    DelFlag = user.DelFlag,
                    EnterpriseName = user.EnterpriseName,
                    Gender = user.Gender,
                    Token = user.Token,
                    UserId = user.UserId,
                    UserName = user.UserName,
                    RoleName = roleName,
                    RoleId = roleId
                };
                LoginLogInfo loginSuccessLog = new LoginLogInfo
                {
                    UserId = loginInput.UserId,
                    Ip = loginInput.IP,
                    BrowserInfo = loginInput.BrowerInfo,
                    SystemInfo = loginInput.OSVersion,
                    IsLoginSuccess = true,
                    HappenTime = DateTime.Now
                };
                _dbContext.Set<LoginLogInfo>().Add(loginSuccessLog);
                if (_dbContext.SaveChanges() > 0)
                {
                    return outputs;
                }
                return null;
            }
            LoginLogInfo loginFaildLog = new LoginLogInfo
            {
                UserId = loginInput.UserId,
                Ip = loginInput.IP,
                BrowserInfo = loginInput.BrowerInfo,
                SystemInfo = loginInput.OSVersion,
                IsLoginSuccess = false,
                HappenTime = DateTime.Now
            };
            _dbContext.Set<LoginLogInfo>().Add(loginFaildLog);
            _dbContext.SaveChanges();
            return null;
        }
    }
}


