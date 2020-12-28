using Microsoft.EntityFrameworkCore;
using RecycleSystem.Data.Data.UserManageDTO;
using RecycleSystem.DataEntity.Entities;
using RecycleSystem.IService;
using RecycleSystem.Ulitity;
using Senkuu.MaterialSystem.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RecycleSystem.Service
{
    public class UserManageService : IUserManageService
    {
        private readonly DbContext _dbContext;
        public UserManageService(DbContext dbContext)
        {
            _dbContext = dbContext;
        }
        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="userInput">输入的数据</param>
        /// <param name="message">回抛的信息</param>
        /// <returns></returns>
        public bool AddUser(UserInput userInput, out string message)
        {
            IQueryable<UserInfo> users = _dbContext.Set<UserInfo>();
            UserInfo user = users.Where(u => u.UserId == userInput.UserId).FirstOrDefault();
            if (user == null)
            {
                try
                {
                    UserInfo newUser = new UserInfo
                    {
                        UserId = userInput.UserId,
                        UserName = userInput.UserName,
                        DepartmentId = userInput.DepartmentId,
                        Email = userInput.Email,
                        Gender = userInput.Gender,
                        DelFlag = false,
                        Tel = userInput.Tel,
                        Password = MD5Helper.EncryptString("123"),
                        UserTypeId = (int)TypeEnum.UserType.isGeneralUser,
                        AddTime = DateTime.Now
                    };
                    _dbContext.Set<UserInfo>().Add(newUser);
                    if (_dbContext.SaveChanges() > 0)
                    {
                        message = "添加成功！";
                        return true;
                    }
                    else
                    {
                        message = "添加失败！内部出现异常！";
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                }
            }
            message = "该用户已存在！请检查UserID！";
            return false;
        }

        /// <summary>
        /// 开启和禁用用户
        /// </summary>
        /// <param name="Id">用户数据的ID</param>
        /// <param name="message">返回的信息</param>
        /// <returns></returns>
        public bool BanUser(int Id, out string message)
        {

            UserInfo user = _dbContext.Set<UserInfo>().Find(Id);
            if (user != null)
            {
                message = "";
                if (user.DelFlag == false)
                {
                    user.DelFlag = true;
                    message = "禁用成功！";
                }
                if (user.DelFlag == true || user.DelFlag == null)
                {
                    user.DelFlag = false;
                    message = "已启用！";
                }
                _dbContext.Set<UserInfo>().Update(user);
                if (_dbContext.SaveChanges() > 0)
                {
                    return true;
                }
                else
                {
                    message = "修改失败！内部出现异常！";
                    return false;
                }
            }
            message = "找不到该用户，数据可能被篡改！";
            return false;
        }
        /// <summary>
        /// 根据Id获取用户信息
        /// </summary>
        /// <param name="Id">用户数据对应的Id</param>
        /// <returns></returns>
        public UserOutput GetUserById(int Id)
        {
            UserInfo user = _dbContext.Set<UserInfo>().Find(Id);
            UserOutput output = new UserOutput();
            if (user != null)
            {
                output.Id = user.Id;
                output.UserId = user.UserId;
                output.UserName = user.UserName;
                output.Tel = user.Tel;
                output.Gender = user.Gender;
                output.Email = user.Email;
                output.UserTypeId = user.UserTypeId;
                output.DepartmentId = user.DepartmentId;
                return output;
            }
            return output;
        }
        /// <summary>
        /// 用户管理页面信息展示
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="limit">每页显示条目数</param>
        /// <param name="count">返回的总条数</param>
        /// <param name="queryInfo">查询信息</param>
        /// <returns></returns>
        public IEnumerable<UserOutput> GetUsers(int page, int limit, out int count, string queryInfo)
        {
            IQueryable<UserInfo> userInfos = _dbContext.Set<UserInfo>();
            IQueryable<DepartmentInfo> departments = _dbContext.Set<DepartmentInfo>();
            IQueryable<UserType> userTypes = _dbContext.Set<UserType>();
            IQueryable<RoleInfo> roleInfos = _dbContext.Set<RoleInfo>();
            IQueryable<RUserRoleInfo> userRoleInfos = _dbContext.Set<RUserRoleInfo>();
            count = userInfos.Count();
            IEnumerable<UserOutput> users = (from t in userTypes
                                             join u in userInfos on t.Id equals u.UserTypeId into join_a
                                             from a in join_a.DefaultIfEmpty()
                                             join d in departments on a.DepartmentId equals d.DepartmentId into join_c
                                             from c in join_c.DefaultIfEmpty()
                                             where a.UserName.Contains(queryInfo) || queryInfo == null
                                             select new UserOutput
                                             {
                                                 Id = a.Id,
                                                 UserId = a.UserId,
                                                 UserName = a.UserName,
                                                 UserTypeName = t.TypeName,
                                                 DepartmentId = a.DepartmentId,
                                                 Tel = a.Tel,
                                                 Email = a.Email,
                                                 Gender = a.Gender,
                                                 EnterpriseName = a.EnterpriseName,
                                                 Token = a.Token,
                                                 DelFlag = a.DelFlag,
                                                 RoleName = (from f in userRoleInfos
                                                             join g in roleInfos on f.RoleId equals g.RoleId
                                                             where f.UserId == a.UserId
                                                             select new
                                                             {
                                                                 g.RoleName
                                                             }
                                                           ).Select(s => s.RoleName).ToList(),
                                                 DepartmentName = c.DepartmentName,
                                                 AddTime = a.AddTime
                                             }).OrderBy(o => o.Id).Skip((page - 1) * limit).Take(limit).ToList();
            return users;
        }
        /// <summary>
        /// 获取所有用户名，及用户ID
        /// </summary>
        /// <returns></returns>
        public IEnumerable<UserOutput> GetUsers()
        {
            IQueryable<UserInfo> users = _dbContext.Set<UserInfo>();
            IEnumerable<UserOutput> outputs = (from u in users
                                               select new UserOutput
                                               {
                                                   Id = u.Id,
                                                   UserId = u.UserId,
                                                   UserName = u.UserName
                                               }).OrderBy(o => o.Id).ToList();
            return outputs;
        }

        /// <summary>
        /// 获取用户类型信息
        /// </summary>
        /// <returns></returns>
        public IEnumerable<UserTypeOutput> GetUserType()
        {
            return (from a in _dbContext.Set<UserType>()
                    select new UserTypeOutput
                    {
                        Id = a.Id,
                        TypeName = a.TypeName
                    }).OrderBy(o => o.Id).ToList();
        }

        /// <summary>
        /// 修改用户信息
        /// </summary>
        /// <param name="input">输入进来的数据的参数模型</param>
        /// <param name="message">返回的信息</param>
        /// <returns></returns>
        public bool ModifyUserInfo(UserInput input, out string message)
        {
            IQueryable<UserInfo> userInfos = _dbContext.Set<UserInfo>();
            UserInfo user = userInfos.Where(u => u.UserId == input.UserId).FirstOrDefault();
            if (user != null)
            {
                user.UserName = input.UserName;
                user.Gender = input.Gender;
                user.Email = input.Email;
                user.DepartmentId = input.DepartmentId;
                user.Tel = input.Tel;
                user.UserTypeId = input.UserTypeId;
                _dbContext.Set<UserInfo>().Update(user);
                if (_dbContext.SaveChanges() > 0)
                {
                    message = "修改成功！";
                    return true;
                }
                else
                {
                    message = "修改失败！内部出现异常！";
                    return false;
                }
            }
            message = "无法找到该用户，数据可能被篡改！";
            return false;
        }
        /// <summary>
        /// 重置密码。可批量
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool RecoverPassword(IEnumerable<int> Id, out string message)
        {
            IQueryable<UserInfo> userInfos = _dbContext.Set<UserInfo>();
            UserInfo user;
            foreach (var item in Id)
            {
                user = userInfos.Where(u => u.Id == item).FirstOrDefault();
                user.Password = MD5Helper.EncryptString("123");
            }
            message = "密码重置成功！初始密码：123";
            return _dbContext.SaveChanges() > 0;
        }
        public bool MutipleImport(IEnumerable<UserInput> userInfos, out string msg)
        {
            IQueryable<UserInfo> users = _dbContext.Set<UserInfo>();
            //遍历
            foreach (var item in userInfos)
            {
                UserInfo userInfo = users.Where(u => u.UserId == item.UserId).FirstOrDefault();
                if (userInfo == null)
                {
                    UserInfo user = new UserInfo
                    {
                        UserId = item.UserId,
                        UserName = item.UserName,
                        UserTypeId = (int?)TypeEnum.UserType.isGeneralUser,
                        Password = MD5Helper.EncryptString("123"),
                        Gender = item.Gender,
                        Email = item.Email,
                        Tel = item.Tel,
                        AddTime = DateTime.Now,
                        DelFlag = false
                    };
                    _dbContext.Set<UserInfo>().Add(user);
                }
                else
                {
                    continue;
                }
            }
            if (_dbContext.SaveChanges() > 0)
            {
                msg = "导入成功！";
                return true;
            }
            msg = "失败！内部错误！";
            return false;
        }
    }
}
