using System.Threading;
using System.Threading.Tasks;
using ChesterDevs.Core.Aspnet.App.Jobs;
using ChesterDevs.Core.Aspnet.Models.ViewModels;
using Microsoft.Extensions.Logging;

namespace ChesterDevs.Core.Aspnet.App.PageHelpers
{
    public interface IJobDetailHelper
    {
        Task<JobDetail> LoadViewModelAsync(int jobId, CancellationToken cancellationToken);
    }

    public class JobDetailHelper : IJobDetailHelper
    {
        private readonly ILogger<JobListHelper> _logger;
        private readonly IJobService _jobService;

        public JobDetailHelper(ILogger<JobListHelper> logger, IJobService jobService)
        {
            _logger = logger;
            _jobService = jobService;
        }

        public async Task<JobDetail> LoadViewModelAsync(int jobId, CancellationToken cancellationToken)
        {
            var jobPage = await _jobService.GetJobAsync(jobId, cancellationToken);
            if (!(jobPage?.Status?.Success ?? true))
            {
                _logger.LogWarning($"Job load failed for job id {jobId}");
            }

            return jobPage?.Job;
        }
    }
}