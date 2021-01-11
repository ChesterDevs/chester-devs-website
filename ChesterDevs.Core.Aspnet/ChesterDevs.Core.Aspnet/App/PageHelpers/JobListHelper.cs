using System.Threading;
using System.Threading.Tasks;
using ChesterDevs.Core.Aspnet.App.Jobs;
using ChesterDevs.Core.Aspnet.Models.ViewModels;
using Microsoft.Extensions.Logging;

namespace ChesterDevs.Core.Aspnet.App.PageHelpers
{
    public interface IJobListHelper
    {
        Task<JobListViewModel> LoadViewModelAsync(int pageNumber, CancellationToken cancellationToken);
    }

    public class JobListHelper : IJobListHelper
    {
        private const int PageSize = 10;

        private readonly ILogger<JobListHelper> _logger;
        private readonly IJobService _jobService;

        public JobListHelper(ILogger<JobListHelper> logger, IJobService jobService)
        {
            _logger = logger;
            _jobService = jobService;
        }

        public async Task<JobListViewModel> LoadViewModelAsync(int pageNumber, CancellationToken cancellationToken)
        {
            var jobPage = await _jobService.GetJobsAsync(pageNumber, PageSize, cancellationToken);
            if (!(jobPage?.Status?.Success ?? true))
            {
                _logger.LogWarning($"Job load failed for page {pageNumber}");
            }

            return new JobListViewModel()
            {
                JobListPage = jobPage,
                PageNumber = pageNumber
            };
        }
    }
}