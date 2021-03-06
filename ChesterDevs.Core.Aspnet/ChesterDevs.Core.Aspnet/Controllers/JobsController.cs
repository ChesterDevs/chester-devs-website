﻿using System.Threading;
using System.Threading.Tasks;
using ChesterDevs.Core.Aspnet.App.Jobs;
using ChesterDevs.Core.Aspnet.App.PageHelpers;
using ChesterDevs.Core.Aspnet.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ChesterDevs.Core.Aspnet.Controllers
{
    public class JobsController: Controller
    {
        private readonly ILogger<JobsController> _logger;
        private readonly IJobListHelper _jobListHelper;
        private readonly IJobDetailHelper _jobDetailHelper;
        private readonly IJobSubscriptionHelper _jobSubscriptionHelper;
        private readonly IAskARecruiterHelper _askARecruiterHelper;

        public JobsController(ILogger<JobsController> logger, IJobListHelper jobListHelper, IJobDetailHelper jobDetailHelper, IJobSubscriptionHelper jobSubscriptionHelper, IAskARecruiterHelper askARecruiterHelper)
        {
            _logger = logger;
            _jobListHelper = jobListHelper;
            _jobDetailHelper = jobDetailHelper;
            _jobSubscriptionHelper = jobSubscriptionHelper;
            _askARecruiterHelper = askARecruiterHelper;
        }

        public async Task<IActionResult> Index(int? pageNumber, CancellationToken cancellationToken)
        {
            return View(await _jobListHelper.LoadViewModelAsync(pageNumber ?? 1, cancellationToken));
        }

        public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
        {
            return View(await _jobDetailHelper.LoadViewModelAsync(id, cancellationToken));
        }

        public IActionResult Subscribe()
        {
            return View(new SubscribeViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Subscribe(SubscribeViewModel model, CancellationToken cancellationToken)
        {
            return View(await _jobSubscriptionHelper.Subscribe(model, cancellationToken));
        }

        public IActionResult Advertise()
        {
            return View();
        }

        [HttpGet]
        public IActionResult RecruiterPanel()
        {
            return View(new AskARecruiterViewModel(){
                AskARecruiter = new AskARecruiter()
            });
        }

        [HttpPost]
        public async Task<IActionResult> RecruiterPanel(AskARecruiterViewModel model, CancellationToken cancellationToken)
        {
            return View(await  _askARecruiterHelper.SendAskARecruiter(model, cancellationToken));
        }

    }
}