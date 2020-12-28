using RecycleSystem.Data.Data.FinanceDTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace RecycleSystem.IService
{
    public interface IFinancialManageService
    {
        IEnumerable<RevenueOutput> GetBills(int page, int limit, out int count, string queryInfo);
        IEnumerable<RevenueOutput> GetAllBills();
    }
}
