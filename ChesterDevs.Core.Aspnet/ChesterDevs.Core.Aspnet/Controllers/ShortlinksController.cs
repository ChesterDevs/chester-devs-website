using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ChesterDevs.Core.Aspnet.Controllers
{
    public class ShortlinksController : Controller
    {
        [Route("/Feedback")]

        public IActionResult Feedback()
        {
            return RedirectPermanent("https://fran682371.typeform.com/to/sISmVB");
        }
    }
}