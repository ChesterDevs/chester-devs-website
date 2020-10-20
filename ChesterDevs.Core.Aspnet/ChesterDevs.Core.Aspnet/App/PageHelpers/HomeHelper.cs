using ChesterDevs.Core.Aspnet.App.RemoteData.EventData;
using ChesterDevs.Core.Aspnet.App.RemoteData.YouTube;
using ChesterDevs.Core.Aspnet.Models.ViewModels;

namespace ChesterDevs.Core.Aspnet.App.PageHelpers
{
    public interface IHomeHelper
    {
        HomeViewModel LoadViewModel();
    }

    public class HomeHelper : IHomeHelper
    {
        private readonly IEventListingData _eventListingData;
        private readonly IYouTubeChannelVideos _youTubeChannelVideos;

        public HomeHelper(IEventListingData eventListingData, IYouTubeChannelVideos youTubeChannelVideos)
        {
            _eventListingData = eventListingData;
            _youTubeChannelVideos = youTubeChannelVideos;
        }

        public HomeViewModel LoadViewModel()
        {
            return new HomeViewModel()
            {
                EventItems = _eventListingData.GetData(),
                YouTubeVideos = _youTubeChannelVideos.GetData()
            };
        }
    }
}