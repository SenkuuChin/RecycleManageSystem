using Microsoft.EntityFrameworkCore;
using RecycleSystem.Data.Data.RoleManageDTO;
using RecycleSystem.DataEntity.Entities;
using RecycleSystem.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RecycleSystem.Service
{
    public class RoleManageService : IRoleManageService
    {
        private readonly DbContext _dbContext;
        public RoleManageService(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public bool AddRole(RoleInput roleInput, out string message)
        {
            IQueryable<RoleInfo> roleInfos = _dbContext.Set<RoleInfo>();
            RoleInfo role1 = roleInfos.Where(r => r.RoleId == roleInput.RoleId).FirstOrDefault();
            if (role1 == null)
            {
                RoleInfo role2 = roleInfos.Where(r => r.RoleName == roleInput.RoleName).FirstOrDefault();
                if (role2 == null)
                {
                    try
                    {
                        RoleInfo newRole = new RoleInfo
                        {
                            RoleId = roleInput.RoleId,
                            RoleName = roleInput.RoleName,
                            DelFlag = false,
                            Description = roleInput.Description,
                            AddTime = DateTime.Now
                        };
                        _dbContext.Set<RoleInfo>().Add(newRole);
                        if (_dbContext.SaveChanges() > 0)
                        {
                            message = "添加成功！";
                            return true;
                        }
                        message = "失败！内部出现异常！";
                        return false;
                    }
                    catch (Exception ex)
                    {
                        message = ex.Message;
                        return false;

                    }

                }
                message = "该角色名已被使用！";
                return false;
            }
            message = "角色ID已存在！";
            return false;
        }
        /// <summary>
        /// 禁用角色
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="message">返回的信息</param>
        /// <returns></returns>
        public bool BanRole(string roleId, out string message)
        {
            IQueryable<RoleInfo> roleInfos = _dbContext.Set<RoleInfo>();
            RoleInfo role = roleInfos.Where(r => r.RoleId == roleId).FirstOrDefault();
            if (role != null)
            {
                try
                {
                    if (role.DelFlag == false)
                    {
                        role.DelFlag = true;
                        role.DelTime = DateTime.Now;
                        _dbContext.Set<RoleInfo>().Update(role);
                        if (_dbContext.SaveChanges() > 0)
                        {
                            message = "已禁用该角色";
                            return true;
                        }
                        else
                        {
                            message = "失败！内部出现异常！";
                            return false;
                        }
                    }
                    else
                    {
                        role.DelFlag = false;
                        _dbContext.Set<RoleInfo>().Update(role);
                        if (_dbContext.SaveChanges() > 0)
                        {
                            message = "已开启该角色";
                            return true;
                        }
                        else
                        {
                            message = "失败！内部出现异常！";
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                    return false;

                }
            }
            message = "角色不存在！数据可能被篡改！";
            return false;
        }

        public RoleOutput GetRoleById(int id)
        {
            RoleInfo role = _dbContext.Set<RoleInfo>().Find(id);
            if (role != null)
            {
                RoleOutput output = new RoleOutput
                {
                    RoleId = role.RoleId,
                    RoleName = role.RoleName,
                    Description = role.Description
                };
                return output;
            }
            return null;
        }

        public IEnumerable<RoleOutput> GetRoles(int page, int limit, out int count, string queryInfo)
        {
            IQueryable<RoleInfo> roleInfos = _dbContext.Set<RoleInfo>();

            count = roleInfos.Count();
            IEnumerable<RoleOutput> roleOutputs = (from r in roleInfos
                                                   where r.RoleName.Contains(queryInfo) || queryInfo == null
                                                   select new RoleOutput
                                                   {
                                                       Id = r.Id,
                                                       RoleId = r.RoleId,
                                                       RoleName = r.RoleName,
                                                       Description = r.Description,
                                                       DelFlag = r.DelFlag,
                                                       AddTime = r.AddTime
                                                   }).OrderBy(o => o.Id).Skip((page - 1) * limit).Take(limit).ToList();
            return roleOutputs;
        }

        public bool Update(RoleInput roleInput, out string message)
        {
            IQueryable<RoleInfo> roleInfos = _dbContext.Set<RoleInfo>();
            RoleInfo role1 = roleInfos.Where(r => r.RoleId == roleInput.RoleId).FirstOrDefault();//判断角色是否存在
            if (role1 != null)
            {
                RoleInfo role2 = roleInfos.Where(r => r.RoleName == roleInput.RoleName).FirstOrDefault();//名字是否被使用
                if (role2 == null || role1.RoleName == roleInput.RoleName)
                {
                    try
                    {
                        role1.RoleName = roleInput.RoleName;
                        role1.Description = roleInput.Description;
                        _dbContext.Set<RoleInfo>().Update(role1);
                        if (_dbContext.SaveChanges()>0)
                        {
                            message = "修改成功！";
                            return true;
                        }
                        message = "失败！内部出现异常！";
                        return false;
                    }
                    catch (Exception ex)
                    {
                        message = ex.Message;
                        return false;
                        
                    }
                }
                message = "该角色名已被使用！";
                return false;
            }
            message = "找不到该角色！数据可能被篡改！";
            return false;
        }
    }
}
