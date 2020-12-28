using Microsoft.AspNetCore.Mvc;
using RecycleSystem.Data.Data.LogDTO;
using RecycleSystem.IService;
using Senkuu.MaterialSystem.Model;
using Senkuu.MaterialSystem.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecycleSystem.MVC.Controllers
{
    public class LogManageController : Controller
    {
        private readonly ILogManageService _logManageService;
        public LogManageController(ILogManageService logManageService)
        {
            _logManageService = logManageService;
        }
        public IActionResult Index()
        {
            return View();
        }
        public string GetLoginLogData(int page, int limit)
        {
            int count;
            IEnumerable<LoginLogOutput> loginLog = _logManageService.GetLoginLogOutputs(page, limit, out count);
            DataResult<IEnumerable<LoginLogOutput>> data = new DataResult<IEnumerable<LoginLogOutput>>
            {
                msg = "获取成功！",
                code = 0,
                count = count,
                data = loginLog
            };
            return JsonNetHelper.SerialzeoJsonForCamelCase(data);
        }
        public IActionResult OperateLog()
        {
            return View();
        }
    }
}
