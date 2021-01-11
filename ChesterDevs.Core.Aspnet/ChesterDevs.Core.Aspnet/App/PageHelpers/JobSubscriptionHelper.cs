using System.Threading;
using System.Threading.Tasks;
using ChesterDevs.Core.Aspnet.App.Jobs;
using ChesterDevs.Core.Aspnet.Models.ViewModels;

namespace ChesterDevs.Core.Aspnet.App.PageHelpers
{
    public interface IJobSubscriptionHelper
    {
        Task<SubscribeViewModel> Subscribe(SubscribeViewModel model, CancellationToken cancellationToken);
    }

    public class JobSubscriptionHelper : IJobSubscriptionHelper
    {
        private readonly IJobService _jobService;

        public JobSubscriptionHelper(IJobService jobService)
        {
            _jobService = jobService;
        }

        public async Task<SubscribeViewModel> Subscribe(SubscribeViewModel model, CancellationToken cancellationToken)
        {
            await _jobService.SubscribeAsync(model.EmailAddress, cancellationToken);
            return new SubscribeViewModel()
            {
                Confirmation = true
            };
        }
    }
}