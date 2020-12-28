using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecycleSystem.Data.Data.DepartmentManageDTO;
using RecycleSystem.IService;
using Senkuu.MaterialSystem.Model;
using Senkuu.MaterialSystem.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecycleSystem.MVC.Controllers
{
    public class DepartmentManageController : Controller
    {
        private readonly IDepartmentManageService _departmentManageService;
        public DepartmentManageController(IDepartmentManageService departmentManageService)
        {
            _departmentManageService = departmentManageService;
        }
        public IActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 获取部门信息（用于部门信息页面展示）
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="queryInfo"></param>
        /// <returns></returns>
        public string GetDepartmentInfo(int page,int limit, string queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo))
            {
                queryInfo = queryInfo.Trim();
            }
            int count;
            IEnumerable<DepartmentOutput> departments = _departmentManageService.GetDepartments(page, limit, out count, queryInfo);
            DataResult<IEnumerable<DepartmentOutput>> data = new DataResult<IEnumerable<DepartmentOutput>>
            {
                msg = "获取成功！",
                code = 0,
                count = count,
                data = departments
            };
            return JsonNetHelper.SerialzeoJsonForCamelCase(data);
        }
        /// <summary>
        /// 获取部门信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetDepartmentInfo()
        {
            return Json(_departmentManageService.GetDepartments());
        }
        public IActionResult Update(string id)
        {
            HttpContext.Session.SetString("UpdateId", id);
            return View();
        }
        public JsonResult GetDepartmentById()
        {
            string dId = HttpContext.Session.GetString("UpdateId");
            DepartmentOutput departmentOutput = _departmentManageService.GetDepartmentById(dId);
            HttpContext.Session.Remove("UpdateId");
            return Json(departmentOutput);
        }
        [HttpPost]
        public JsonResult Update(DepartmentInput departmentInput)
        {
            string message;
            //进行输入认证，是否输入的格式是对的。要求格式 D开头后接数字
            string str = departmentInput.DepartmentId.Substring(0, 1); //获取第一个字符，验证是否为D开头
            string str1 = departmentInput.DepartmentId.Substring(1);
            if (str!="D")
            {
                message = "部门ID格式不正确！请以D开头，后续接数字！如：D1001 ！！";
                return Json(message);
            }
            bool result = _departmentManageService.UpdateDepartmentInfoById(departmentInput, out message);
            return Json(message);
        }
        public IActionResult AddDepartment()
        {
            return View();
        }
        [HttpPost]
        public JsonResult AddDepartment(DepartmentInput departmentInput)
        {
            string message;
            //进行输入认证，是否输入的格式是对的。要求格式 D开头后接数字
            string str = departmentInput.DepartmentId.Substring(0, 1); //获取第一个字符，验证是否为D开头
            string str1 = departmentInput.DepartmentId.Substring(1);
            if (str != "D")
            {
                message = "部门ID格式不正确！请以 D 开头，后续接数字！如：D1001 ！！";
                return Json(message);
            }
            _departmentManageService.AddDepartment(departmentInput, out message);
            return Json(message);
        }
    }
}
