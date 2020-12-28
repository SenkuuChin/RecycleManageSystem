using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecycleSystem.Data.Data.CategoryDTO;
using RecycleSystem.IService;
using Senkuu.MaterialSystem.Model;
using Senkuu.MaterialSystem.Utility;

namespace RecycleSystem.MVC.Controllers
{
    public class CategoryManageController : Controller
    {
        private ICategoryManageService _categoryManageService;
        public CategoryManageController(ICategoryManageService categoryManageService)
        {
            _categoryManageService = categoryManageService;
        }
        public IActionResult Index()
        {
            return View();
        }
        public string GetCategoriesList(int page,int limit,string queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo))
            {
                queryInfo = queryInfo.Trim();
            }
            int count;
            IEnumerable<CategoryOutput> categories = _categoryManageService.GetCategories(page, limit, out count, queryInfo);
            DataResult<IEnumerable<CategoryOutput>> data = new DataResult<IEnumerable<CategoryOutput>>
            {
                msg = "获取成功！",
                code = 0,
                count = count,
                data = categories
            };
            return JsonNetHelper.SerialzeoJsonForCamelCase(data);
        }
        public JsonResult GetCategories()
        {
            return Json(_categoryManageService.GetCategories());
        }
        public IActionResult Update(int id)
        {
            HttpContext.Session.SetInt32("UpdateId", id);
            return View();
        }
        [HttpPost]
        public JsonResult GetCategoryById()
        {
            int id = (int)HttpContext.Session.GetInt32("UpdateId");
            CategoryOutput output = _categoryManageService.GetCategory(id);
            HttpContext.Session.Remove("UpdateId");
            return Json(output);
        }
        [HttpPost]
        public JsonResult Update(CategoryInput categoryInput)
        {
            string msg;
            _categoryManageService.UpdateCategoryById(categoryInput, out msg);
            return Json(msg);
        }
        public JsonResult ChangeState(int id)
        {
            string msg;
            _categoryManageService.BanCategory(id,out msg);
            return Json(msg);
        }
        public IActionResult AddCategoryPage()
        {
            return View();
        }
        [HttpPost]
        public JsonResult AddCategory(CategoryInput categoryInput)
        {
            string msg;
            if (categoryInput.CurrentPrice==null)
            {
                msg = "你输入的不是数字噢！检查一下吧！";
                return Json(msg);
            }
            _categoryManageService.AddCategory(categoryInput, out msg);
            return Json(msg);
        }
    }
}
