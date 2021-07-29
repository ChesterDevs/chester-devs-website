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
            return RedirectPermanent("https://forms.office.com/Pages/ResponsePage.aspx?id=kVkFyuLjwUOSgJospHkmWD_i3dEovIdOp_HvoF_AuApURURGRDc3OUcxTTI0Q1IzV1VaNjJBV1M4Ti4u");
        }

        [Route("/Slack")]
        public IActionResult Slack()
        {
            return RedirectPermanent("https://join.slack.com/t/chesterdevs/shared_invite/zt-fk2t22wn-TNZ_tqp4JKE~lnKYthqThQ");
        }
    }
}