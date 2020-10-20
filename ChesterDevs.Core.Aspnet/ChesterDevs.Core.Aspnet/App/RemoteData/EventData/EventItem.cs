using System;

namespace ChesterDevs.Core.Aspnet.App.RemoteData.EventData
{
    public class EventItem
    {
        public string Name { get; set; }
        public string Link { get; set; }
        public string Description { get; set; }
        public DateTime EventDate { get; set; }
        public string Time { get; set; }
        public string PhotoLink { get; set; }
        public string VenueName { get; set; }
        public string VenueCity { get; set; }
    }
}