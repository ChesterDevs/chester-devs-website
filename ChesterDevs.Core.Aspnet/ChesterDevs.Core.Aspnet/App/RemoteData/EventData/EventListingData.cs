using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LazyCache;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ChesterDevs.Core.Aspnet.App.RemoteData.EventData
{
    public interface IEventListingData : IRemoteDataSource
    {
        List<EventItem> GetData();
    }

    public class EventListingData : IEventListingData
    {
        private const string CACHE_KEY = nameof(EventListingData);

        private const string API_URL =
            "https://api.meetup.com/Chester-Devs/events?&sign=true&photo-host=secure&page=20&fields=featured_photo,plain_text_description";

        private const string FILE_CACHE_PATH = "/Data/EventListingData.json";

        private readonly IHttpWrapper _httpWrapper;
        private readonly IAppCache _cache;
        private readonly ILogger<EventListingData> _logger;
        private readonly IFileSystemWrapper _fileSystemWrapper;

        public EventListingData(IHttpWrapper httpWrapper, IAppCache cache, ILogger<EventListingData> logger, IFileSystemWrapper fileSystemWrapper)
        {
            _httpWrapper = httpWrapper;
            _cache = cache;
            _logger = logger;
            _fileSystemWrapper = fileSystemWrapper;
        }

        public List<EventItem> GetData()
        {
            return _cache.GetOrAdd(
                CACHE_KEY,
                (c) => _fileSystemWrapper.ReadJson<List<EventItem>>(FILE_CACHE_PATH));
        }

        public void RefreshData()
        {
            try
            {
                var data = _httpWrapper.GetData(API_URL, ResponseBuilder);
                _cache.Add(CACHE_KEY, data,
                    new MemoryCacheEntryOptions() {AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)});
                _fileSystemWrapper.SaveFile(FILE_CACHE_PATH, JsonConvert.SerializeObject(data));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error thrown getting data from Meetup");
            }
        }

        private List<EventItem> ResponseBuilder(Stream response)
        {
            var json = JObject.ReadFrom(new JsonTextReader(new StreamReader(response)));

            return json.Select(evt => new EventItem()
                {
                    Name = (string)evt["name"],
                    Link = (string) evt["link"],
                    Description = Truncate((string) evt["plain_text_description"], 250),
                    EventDate = DateTime.Parse((string)evt["local_date"]),
                    Time = (string) evt["local_time"],
                    PhotoLink = (string) evt["featured_photo"]?["photo_link"],
                    VenueName = (string) evt["venue"]["name"],
                    VenueCity = (string)evt["venue"]["city"]
            })
                .ToList();
        }

        private string Truncate(string value, int maxLength)
        {
            if (value.Length <= maxLength)
                return value;

            var lastSpaceIndex = value.Substring(0, maxLength).LastIndexOf(" ");
            return $"{value.Substring(0, lastSpaceIndex)} ...";
        }
        
    }
}