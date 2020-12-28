using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecycleSystem.Data.Data.UserManageDTO;
using RecycleSystem.IService;
using RecycleSystem.Ulitity;
using Senkuu.MaterialSystem.Model;
using Senkuu.MaterialSystem.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RecycleSystem.MVC.Controllers
{
    public class UserManageController : Controller
    {
        private readonly IUserManageService _userManageService;
        public UserManageController(IUserManageService userManageService)
        {
            _userManageService = userManageService;
        }
        public IActionResult Index()
        {
            return View();
        }
        public string GetUserListJson(int page,int limit,string queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo))
            {
                queryInfo = queryInfo.Trim();
            }
            int count;
            IEnumerable<UserOutput> users = _userManageService.GetUsers(page, limit, out count, queryInfo);
            DataResult<IEnumerable<UserOutput>> data = new DataResult<IEnumerable<UserOutput>>
            {
                msg = "获取成功！",
                code = 0,
                count = count,
                data = users
            };
            return JsonNetHelper.SerialzeoJsonForCamelCase(data);
        }
        public IActionResult Update(int Id)
        {
            //储存从页面传过来的Id,供获取用户信息使用
            HttpContext.Session.SetInt32("UpdateId", Id);
            return View();
        }
        [HttpPost]
        public JsonResult GetUserInfoById()
        {
            int Id = (int)HttpContext.Session.GetInt32("UpdateId");
            UserOutput userOutput = _userManageService.GetUserById(Id);
            HttpContext.Session.Remove("UpdateId");
            return Json(userOutput);
        }
        [HttpPost]
        public JsonResult GetUserType()
        {
            return Json(_userManageService.GetUserType());
        }
        [HttpPost]
        public JsonResult Update(UserInput userInput)
        {
            string message;
            if (userInput!=null)
            {
                _userManageService.ModifyUserInfo(userInput, out message);
                return Json(message);
            }
            else
            {
                message = "没有填写任何数据！";
                return Json(message);
            }
        }
        [HttpPost]
        public JsonResult Repwd(IEnumerable<int> Id)
        {
            string message = "";
            _userManageService.RecoverPassword(Id, out message);
            return Json(message);
        }
        public JsonResult ChangeState(int id)
        {
            string message = "";
            _userManageService.BanUser(id, out message);
            return Json(message);
        }
        public IActionResult AddUserPage()
        {
            return View();
        }
        [HttpPost]
        public JsonResult AddUser(UserInput userInput)
        {
            string message;
            //进行输入认证，是否输入的格式是对的。要求格式 D开头后接数字
            string str = userInput.UserId.Substring(0, 1); //获取第一个字符，验证是否为D开头
            string str1 = userInput.UserId.Substring(1);
            if (str != "U")
            {
                message = "用户ID格式不正确！请以 U 开头，后续接数字！如：U1001 ！！";
                return Json(message);
            }
            _userManageService.AddUser(userInput, out message);
            return Json(message);
        }
        [HttpPost]
        public JsonResult GetUsers()
        {
            return Json(_userManageService.GetUsers());
        }
        public JsonResult MultipleImport(IFormFile file)
        {
            string msg="";
            string msg1;
            //IEnumerable<UserInput> inputs = new ExcelHelper<UserInput>().ImportFromExcel(Path.GetExtension(file.FileName));
            DataTable dataTable = OriginExcelHelper.ExcelToDataTable(file.OpenReadStream(), Path.GetExtension(file.FileName), out msg);
            IEnumerable<UserInput> users = OriginExcelHelper.ConvertToList(dataTable,out msg1);
            _userManageService.MutipleImport(users, out msg);
            return Json(new
            {
                Result = true,
                Data = msg
            });
        }
    }
}
