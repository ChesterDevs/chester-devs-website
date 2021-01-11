using System.Collections.Generic;

namespace ChesterDevs.Core.Aspnet.App.Jobs
{
    public class JobListPage
    {
        public List<JobSummary> Jobs { get; set; }
        public int PageCount { get; set; }
        public Status Status { get; set; } 

    }
}