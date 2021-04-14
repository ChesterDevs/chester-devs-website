using System.Threading;
using System.Threading.Tasks;
using ChesterDevs.Core.Aspnet.App.Jobs;
using ChesterDevs.Core.Aspnet.Models.ViewModels;

namespace ChesterDevs.Core.Aspnet.App.PageHelpers
{
    public interface IAskARecruiterHelper
    {
        Task<AskARecruiterViewModel> SendAskARecruiter(AskARecruiterViewModel model, CancellationToken cancellationToken);
    }

    public class AskARecruiterHelper : IAskARecruiterHelper
    {
        private readonly IJobService _jobService;

        public AskARecruiterHelper(IJobService jobService)
        {
            _jobService = jobService;
        }

        public async Task<AskARecruiterViewModel> SendAskARecruiter(AskARecruiterViewModel model, CancellationToken cancellationToken)
        {
            await _jobService.AskARecruiterAsync(model.AskARecruiter, cancellationToken);

            model.Confirmation = true;

            return model;
        }
    }
}