using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RecycleSystem.Data.Data.FinanceDTO;
using RecycleSystem.IService;
using Senkuu.MaterialSystem.Model;
using Senkuu.MaterialSystem.Utility;

namespace RecycleSystem.MVC.Controllers
{
    public class FinancialManageController : Controller
    {
        private readonly IFinancialManageService _financialManageService;
        public FinancialManageController(IFinancialManageService financialManageService)
        {
            _financialManageService = financialManageService;
        }
        public IActionResult Bills()
        {
            return View();
        }
        public string GetBills(int page, int limit, string queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo))
            {
                queryInfo = queryInfo.Trim();
            }
            int count;
            IEnumerable<RevenueOutput> revenues = _financialManageService.GetBills(page, limit, out count, queryInfo);
            DataResult<IEnumerable<RevenueOutput>> data = new DataResult<IEnumerable<RevenueOutput>>
            {
                msg = "获取成功！",
                code = 0,
                count = count,
                data = revenues
            };
            return JsonNetHelper.SerialzeoJsonForCamelCase(data);
        }
        public JsonResult ExcelExport()
        {

            return Json(_financialManageService.GetAllBills());
        }
    }
}
