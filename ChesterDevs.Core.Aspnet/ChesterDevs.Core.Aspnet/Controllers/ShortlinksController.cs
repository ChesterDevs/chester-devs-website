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

        [Route("/Slack")]
        public IActionResult Slack()
        {
            return RedirectPermanent("https://join.slack.com/t/chesterdevs/shared_invite/zt-fk2t22wn-TNZ_tqp4JKE~lnKYthqThQ");
        }
    }
}