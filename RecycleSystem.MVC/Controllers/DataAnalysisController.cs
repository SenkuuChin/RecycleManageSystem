﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace RecycleSystem.MVC.Controllers
{
    public class DataAnalysisController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}