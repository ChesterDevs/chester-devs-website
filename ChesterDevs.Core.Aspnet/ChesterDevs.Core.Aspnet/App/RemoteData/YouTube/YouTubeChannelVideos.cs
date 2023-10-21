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

namespace ChesterDevs.Core.Aspnet.App.RemoteData.YouTube
{
    public interface IYouTubeChannelVideos : IRemoteDataSource
    {
        List<YouTubeVideo> GetData();
    }

    public class YouTubeChannelVideos : IYouTubeChannelVideos
    {
        private const string CACHE_KEY = nameof(YouTubeChannelVideos);
        private const string YOU_TUBE_CHANNEL_ID = "UCWS-6okcWfCrviVGzy1-KEg";
        private const string API_URL =
            "https://www.googleapis.com/youtube/v3/search?key={0}&part=snippet&type=video&channelId=" + YOU_TUBE_CHANNEL_ID + "&order=date&maxResults=4";

        private const string FILE_CACHE_PATH = "/Data/YouTubeChannelVideos.json";

        private readonly IHttpWrapper _httpWrapper;
        private readonly IAppCache _cache;
        private readonly ILogger<YouTubeChannelVideos> _logger;
        private readonly IFileSystemWrapper _fileSystemWrapper;
        private readonly ISecretManager _secretManager;
        
        private DateTime _lastAccessed = DateTime.MinValue;

        public YouTubeChannelVideos(IHttpWrapper httpWrapper, IAppCache cache, ILogger<YouTubeChannelVideos> logger, IFileSystemWrapper fileSystemWrapper, ISecretManager secretManager)
        {
            _httpWrapper = httpWrapper;
            _cache = cache;
            _logger = logger;
            _fileSystemWrapper = fileSystemWrapper;
            _secretManager = secretManager;
        }

        public List<YouTubeVideo> GetData()
        {
            return _cache.GetOrAdd(
                CACHE_KEY,
                (c) => _fileSystemWrapper.ReadJson<List<YouTubeVideo>>(FILE_CACHE_PATH));
        }

        public void RefreshData()
        {
            try
            {
                if (string.IsNullOrEmpty(_secretManager.Secrets.GoogleApiKey))
                    return;

                if (DateTime.Now.Subtract(_lastAccessed).Minutes < 30)
                    return;

                _lastAccessed = DateTime.Now;

                var data = _httpWrapper.GetData(
                    string.Format(API_URL, _secretManager.Secrets.GoogleApiKey), 
                    ResponseBuilder);
                _cache.Add(CACHE_KEY, data,
                    new MemoryCacheEntryOptions() {AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)});
                _fileSystemWrapper.SaveFile(FILE_CACHE_PATH, JsonConvert.SerializeObject(data));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error thrown getting data from YouTube");
            }
        }

        private List<YouTubeVideo> ResponseBuilder(Stream response)
        {
            var json = JObject.ReadFrom(new JsonTextReader(new StreamReader(response)));

            return json["items"].Select(video => new YouTubeVideo()
                {
                    VideoId = (string)video["id"]["videoId"],
                    Title = (string) video["snippet"]["title"],
                    ThumbnailUrl = (string) video["snippet"]["thumbnails"]["high"]["url"],
                    PublishedDate = DateTime.Parse((string)video["snippet"]["publishedAt"], 
                        CultureInfo.InvariantCulture, 
                        DateTimeStyles.None)
            })
                .ToList();
        }
        
    }
}