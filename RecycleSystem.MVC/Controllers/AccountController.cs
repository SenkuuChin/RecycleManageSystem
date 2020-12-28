using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecycleSystem.Data.Data.LoginDTO;
using Microsoft.Extensions.DependencyInjection;
using RecycleSystem.IService;
using RecycleSystem.Ulitity;
using Wangkanai.Detection.Services;
using RecycleSystem.Ulitity.Extension;

namespace RecycleSystem.MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        //使用 Detection包来获取设备相关信息
        private readonly IDetectionService _detection;
        public AccountController(IAccountService accountService, IHttpContextAccessor httpContextAccessor,IDetectionService detectionService)
        {
            _accountService = accountService;
            _httpContextAccessor = httpContextAccessor;
            _detection = detectionService;
        }
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult GetCaptchaImage()
        {
            string sessionId = GlobalContext.ServiceProvider?.GetService<IHttpContextAccessor>().HttpContext.Session.Id;

            Tuple<string, int> captchaCode = CaptchaHelper.GetCaptchaCode();
            byte[] bytes = CaptchaHelper.CreateCaptchaImage(captchaCode.Item1);
            new SessionHelper().WriteSession("CaptchaCode", captchaCode.Item2);
            return File(bytes, @"image/jpeg");
        }
        [HttpPost]
        public IActionResult Login(LoginInput loginInput)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(loginInput.VerifyCode))
                {
                    //obj.Message = "验证码不能为空";
                    //return Json(obj);
                    ViewBag.LoginError = "验证码不能为空";
                    return View();
                }
                if (loginInput.VerifyCode != new SessionHelper().GetSession("CaptchaCode").ParseToString())
                {
                    //obj.Message = "验证码错误，请重新输入";
                    //return Json(obj);
                    ViewBag.LoginError = "验证码错误，请重新输入";
                    return View();
                }
                loginInput.BrowerInfo = _detection.Browser.Name.ToString() + _detection.Browser.Version;
                loginInput.IP = NetHelper.GetWanIp();
                loginInput.OSVersion = NetHelper.GetOSVersion();
                LoginOutput login = _accountService.Login(loginInput);
                if (login != null)
                {
                    //string ipaddress = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
                    //string ip = GetUserDeviceInfo.GetUserIp(HttpContext);
                    //string userAgent = GetUserDeviceInfo.UserAgent(HttpContext);
                    //var userAgent = Request.Headers["User-Agent"];
                   
                    HttpContext.Session.SetString("UserName", login.UserName);
                    HttpContext.Session.SetString("UserId", login.UserId);
                    return RedirectToAction("Index", "Main");
                }
                return RedirectToAction("Login", "Account");
            }
            return View();
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
