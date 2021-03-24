using System.Linq;
using ChesterDevs.Core.Aspnet.App.Jobs;

namespace ChesterDevs.Core.Aspnet.Models.ViewModels
{
    public class JobListViewModel
    {
        public JobListPage JobListPage { get; set; }

        public int PageNumber { get; set; }

        public bool HasJobs
            => (JobListPage?.Status.Success ?? false) && (JobListPage.Jobs?.Any() ?? false);

        public bool HasNextPage
            => (JobListPage?.PageCount ?? 0) > PageNumber;

        public int NextPage
            => PageNumber + 1;

    }
}