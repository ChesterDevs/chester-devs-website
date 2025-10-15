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
            return RedirectPermanent("https://forms.office.com/Pages/ResponsePage.aspx?id=DQSIkWdsW0yxEjajBLZtrQAAAAAAAAAAAAMAAOQSD_9UNDVBMU5NUDBUS0tOUVFOSTBMMlRYNlFaQS4u");
        }

        [Route("/Slack")]
        public IActionResult Slack()
        {
            return RedirectPermanent("https://join.slack.com/t/chesterdevs/shared_invite/zt-fk2t22wn-TNZ_tqp4JKE~lnKYthqThQ");
        }

        [Route("/YouTube")]
        public IActionResult YouTube()
        {
            return RedirectPermanent("https://www.youtube.com/channel/UCWS-6okcWfCrviVGzy1-KEg");
        }

        [Route("/CodeOfConduct")]
        public IActionResult CodeOfConduct()
        {
            return RedirectToActionPermanent("Pages", "Home", new { view= "CodeOfConduct" });
        }

    }
}