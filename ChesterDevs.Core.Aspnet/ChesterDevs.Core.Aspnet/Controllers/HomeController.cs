using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ChesterDevs.Core.Aspnet.App.RemoteData;
using ChesterDevs.Core.Aspnet.BackgroundServices;
using Microsoft.AspNetCore.Mvc;
using ChesterDevs.Core.Aspnet.Models;
using Microsoft.Extensions.Logging;

namespace ChesterDevs.Core.Aspnet.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IEventListingData _eventListingData;

        public HomeController(ILogger<HomeController> logger, IEventListingData eventListingData)
        {
            _logger = logger;
            _eventListingData = eventListingData;
        }

        public IActionResult Index()
        {
            return View(_eventListingData.GetData());
        }

        [Route("[Controller]/[Action]/{view=Home}")]
        public IActionResult Pages(string view)
        {
            if (string.IsNullOrEmpty(view) || view.Equals("home", StringComparison.InvariantCultureIgnoreCase))
                return RedirectToAction(nameof(Index));

            return View(view);
        }
        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
