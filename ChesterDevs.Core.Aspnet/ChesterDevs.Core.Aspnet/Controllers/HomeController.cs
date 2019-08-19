using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ChesterDevs.Core.Aspnet.Models;

namespace ChesterDevs.Core.Aspnet.Controllers
{
    public class HomeController : Controller
    {
        private static string[] ValidViews;


        public IActionResult Pages(string view)
        {
            if(string.IsNullOrEmpty(view))
                return View("home");

            return View(view);
        }
        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
