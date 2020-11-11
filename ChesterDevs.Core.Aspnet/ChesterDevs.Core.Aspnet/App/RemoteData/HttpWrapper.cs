using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Remotion.Linq.Clauses;

namespace ChesterDevs.Core.Aspnet.App.RemoteData
{
    public class HttpWrapper: IHttpWrapper
    {
        private readonly ILogger<HttpWrapper> _logger;
        private readonly HttpClient _httpClient;

        public HttpWrapper(ILogger<HttpWrapper> logger)
        {
            _logger = logger;
            _httpClient = new HttpClient();
        }

        public T GetData<T>(string url, Func<Stream, T> responseBuilder)
        {
            return GetDataAsync(url, responseBuilder).Result;
        }

        public async Task<T> GetDataAsync<T>(string url, Func<Stream, T> responseBuilder)
        {
            var response = await _httpClient.GetAsync(new Uri(url));

            using (var stream = await response.Content.ReadAsStreamAsync())
            {
                if (response.IsSuccessStatusCode)
                {
                    return responseBuilder(stream);
                }
                else
                {
                    using (var reader = new StreamReader(stream))
                        _logger.LogError(reader.ReadToEnd());

                    throw new HttpRequestException(
                        $"Response status code does not indicate success: {(int) response.StatusCode} ({response.StatusCode.ToString()}).");
                }

            }

        }
    }
}