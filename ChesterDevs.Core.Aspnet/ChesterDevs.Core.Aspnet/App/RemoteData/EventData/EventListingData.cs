using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using ChesterDevs.Core.Aspnet.App.Secrets;
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

    public class EventbriteEventListingData : IEventListingData
    {
        private const string CACHE_KEY = nameof(EventbriteEventListingData);

        private const string API_URL =
            "https://www.eventbriteapi.com/v3/organizations/2569267043351/events/?token={0}&status=live";

        private const string FILE_CACHE_PATH = "/Data/EventbriteEventListingData.json";

        private readonly IHttpWrapper _httpWrapper;
        private readonly IAppCache _cache;
        private readonly ILogger<EventbriteEventListingData> _logger;
        private readonly IFileSystemWrapper _fileSystemWrapper;
        private readonly ISecretManager _secretManager;

        public EventbriteEventListingData(IHttpWrapper httpWrapper, IAppCache cache, ILogger<EventbriteEventListingData> logger, IFileSystemWrapper fileSystemWrapper, ISecretManager secretManager)
        {
            _httpWrapper = httpWrapper;
            _cache = cache;
            _logger = logger;
            _fileSystemWrapper = fileSystemWrapper;
            _secretManager = secretManager;
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
                var key = _secretManager.Secrets.EventBriteApiKey;
                var url = string.Format(API_URL, key);
                var data = _httpWrapper.GetData(url, ResponseBuilder);
                _cache.Add(CACHE_KEY, data,
                    new MemoryCacheEntryOptions() { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1) });
                _fileSystemWrapper.SaveFile(FILE_CACHE_PATH, JsonConvert.SerializeObject(data));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error thrown getting data from Eventbrite");
            }
        }

        //Todo: Replace with eventbrite
        private List<EventItem> ResponseBuilder(Stream response)
        {
            var rawJson = (new StreamReader(response)).ReadToEnd();
            var settings = new JsonSerializerSettings { DateParseHandling = DateParseHandling.None };
            var json = JsonConvert.DeserializeObject<JObject>(rawJson, settings);

            return json["events"].Select(evt => new EventItem()
            {
                Name = (string)evt["name"]["text"],
                Link = (string)evt["url"],
                Description = Truncate((string)evt["summary"], 250),
                EventDate = DateTime.ParseExact(((string)evt["start"]["local"]).Split('T')[0], "yyyy-MM-dd", CultureInfo.InvariantCulture).Date,
                Time = ((string)evt["start"]["local"]).Split('T')[1].Substring(0,5),
                PhotoLink = (string)evt["logo"]?["url"]
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