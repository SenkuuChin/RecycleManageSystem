using Microsoft.EntityFrameworkCore;
using RecycleSystem.Data.Data.FinanceDTO;
using RecycleSystem.DataEntity.Entities;
using RecycleSystem.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RecycleSystem.Service
{
    public class FinancialManageService : IFinancialManageService
    {
        private readonly DbContext _dbContext;
        public FinancialManageService(DbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IEnumerable<RevenueOutput> GetBills(int page, int limit, out int count, string queryInfo)
        {
            IQueryable<RevenueBill> revenueBills = _dbContext.Set<RevenueBill>();
            count = revenueBills.Count();
            IEnumerable<RevenueOutput> revenueOutputs = (from r in revenueBills
                                                         where r.Zid.Contains(queryInfo) || queryInfo == null
                                                         select new RevenueOutput
                                                         {
                                                             Id = r.Id,
                                                             Zid = r.Zid,
                                                             Name = r.Name,
                                                             Oid = r.Oid,
                                                             Num = r.Num,
                                                             Unit = r.Unit,
                                                             Money = r.Money,
                                                             AddTime = r.AddTime
                                                         }).OrderBy(o => o.AddTime).Skip((page - 1) * limit).Take(limit).ToList();
            return revenueOutputs;
        }
        public IEnumerable<RevenueOutput> GetAllBills()
        {
            IQueryable<RevenueBill> revenueBills = _dbContext.Set<RevenueBill>();
            IEnumerable<RevenueOutput> revenueOutputs = (from r in revenueBills
                                                         select new RevenueOutput
                                                         {
                                                             Id = r.Id,
                                                             Zid = r.Zid,
                                                             Name = r.Name,
                                                             Oid = r.Oid,
                                                             Num = r.Num,
                                                             Unit = r.Unit,
                                                             Money = r.Money,
                                                             AddTime = r.AddTime
                                                         }).OrderBy(o => o.AddTime).ToList();
            return revenueOutputs;
        }
    }
}
