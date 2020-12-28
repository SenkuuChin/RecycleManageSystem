using Microsoft.EntityFrameworkCore;
using RecycleSystem.Data.Data.LogDTO;
using RecycleSystem.DataEntity.Entities;
using RecycleSystem.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RecycleSystem.Service
{
    public class LogManageService : ILogManageService
    {
        private readonly DbContext _dbContext;
        public LogManageService(DbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IEnumerable<LoginLogOutput> GetLoginLogOutputs(int page, int limit, out int count)
        {
            IQueryable<UserInfo> userInfos = _dbContext.Set<UserInfo>();
            IQueryable<LoginLogInfo> loginLogInfos = _dbContext.Set<LoginLogInfo>();
            count = loginLogInfos.Count();
            IEnumerable<LoginLogOutput> loginLogOutputs = (from l in loginLogInfos
                                                           join u in userInfos on l.UserId equals u.UserId
                                                           select new LoginLogOutput
                                                           {
                                                               Id = l.Id,
                                                               UserId = u.UserName,
                                                               Ip = l.Ip,
                                                               SystemInfo = l.SystemInfo,
                                                               BrowserInfo = l.BrowserInfo,
                                                               IsLoginSuccess = l.IsLoginSuccess,
                                                               HappenTime = l.HappenTime
                                                           }).OrderBy(o => o.HappenTime).Skip((page - 1) * limit).Take(limit).ToList();
            return loginLogOutputs;
        }

        public IEnumerable<OperateLogOutput> GetOperateLogOutputs(int page, int limit, out int count)
        {
            IQueryable<UserInfo> userInfos = _dbContext.Set<UserInfo>();
            IQueryable<OperateLog> operateLogs = _dbContext.Set<OperateLog>();
            count = operateLogs.Count();
            IEnumerable<OperateLogOutput> operateLogOutputs = (from o in operateLogs
                                                               join u in userInfos on o.OperatorId equals u.UserId
                                                               select new OperateLogOutput
                                                               {
                                                                   Id = o.Id,
                                                                   Operator = u.UserName,
                                                                   Info = o.Info,
                                                                   AddTime = o.AddTime
                                                               }).OrderBy(o => o.AddTime).Skip((page - 1) * limit).Take(limit).ToList();
            return operateLogOutputs;
        }
    }
}
