using ChesterDevs.Core.Aspnet.App.Utils;

namespace ChesterDevs.Core.Aspnet.App.Jobs
{
    public class JobSummary
    {
        public int Id { get; set; }
        public string Advertiser { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Location { get; set; }
        public string JobType { get; set; }
        public string Salary { get; set; }
        public string LogoUrl { get; set; }

        public string HtmlSafeDetail
            => HtmlUtils.ConvertToHtml(Summary);
    }
}