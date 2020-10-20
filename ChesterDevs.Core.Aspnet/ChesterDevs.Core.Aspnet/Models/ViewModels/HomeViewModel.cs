using System.Collections.Generic;
using ChesterDevs.Core.Aspnet.App.RemoteData.EventData;
using ChesterDevs.Core.Aspnet.App.RemoteData.YouTube;

namespace ChesterDevs.Core.Aspnet.Models.ViewModels
{
    public class HomeViewModel
    {
        public List<EventItem> EventItems { get; set; }
        public List<YouTubeVideo> YouTubeVideos { get; set; }
    }
}