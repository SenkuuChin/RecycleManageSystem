using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecycleSystem.Data.Data.RoleManageDTO;
using RecycleSystem.IService;
using Senkuu.MaterialSystem.Model;
using Senkuu.MaterialSystem.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecycleSystem.MVC.Controllers
{
    public class RoleManageController : Controller
    {
        private readonly IRoleManageService _roleManageService;
        public RoleManageController(IRoleManageService roleManageService)
        {
            _roleManageService = roleManageService;
        }
        public IActionResult Index()
        {
            return View();
        }
        public string GetRoleInfo(int page, int limit, string queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo))
            {
                queryInfo = queryInfo.Trim();
            }
            int count;
            IEnumerable<RoleOutput> roles = _roleManageService.GetRoles(page, limit, out count, queryInfo);
            DataResult<IEnumerable<RoleOutput>> data = new DataResult<IEnumerable<RoleOutput>>
            {
                msg = "获取成功！",
                code = 0,
                count = count,
                data = roles
            };
            return JsonNetHelper.SerialzeoJsonForCamelCase(data);
        }
        public IActionResult AddRole()
        {
            return View();
        }
        [HttpPost]
        public JsonResult AddRole(RoleInput roleInput)
        {
            string message;
            //进行输入认证，是否输入的格式是对的。要求格式 D开头后接数字
            string str = roleInput.RoleId.Substring(0, 1); //获取第一个字符，验证是否为D开头
            string str1 = roleInput.RoleId.Substring(1);  //获取从第一个字后的数字。
            if (str != "R")
            {
                message = "角色ID格式不正确！请以 R 开头，后续接数字！如：R1001 ！！";
                return Json(message);
            }
            _roleManageService.AddRole(roleInput, out message);
            return Json(message);
        }
        public IActionResult Update(int id)
        {
            HttpContext.Session.SetInt32("UpdateId", id);
            return View();
        }
        public JsonResult GetRoleById()
        {
            int id = (int)HttpContext.Session.GetInt32("UpdateId");
           RoleOutput roleOutput = _roleManageService.GetRoleById(id);
            HttpContext.Session.Remove("UpdateId");
            return Json(roleOutput);
        }
        [HttpPost]
        public JsonResult Update(RoleInput roleInput)
        {
            string message;
            //进行输入认证，是否输入的格式是对的。要求格式 D开头后接数字
            string str = roleInput.RoleId.Substring(0, 1); //获取第一个字符，验证是否为D开头
            string str1 = roleInput.RoleId.Substring(1);  //获取从第一个字后的数字。
            if (str != "R")
            {
                message = "角色ID格式不正确！请以 R 开头，后续接数字！如：R1001 ！！";
                return Json(message);
            }
            _roleManageService.Update(roleInput, out message);
            return Json(message);
        }
        public JsonResult ChangeState(string roleId)
        {
            string message;
            _roleManageService.BanRole(roleId, out message);
            return Json(message);
        }
    }
}
