using System;
using System.IO;
using System.Net.Http;
using Remotion.Linq.Clauses;

namespace ChesterDevs.Core.Aspnet.App.RemoteData
{
    public class HttpWrapper: IHttpWrapper
    {
        private readonly HttpClient _httpClient;

        public HttpWrapper()
        {
            _httpClient = new HttpClient();
        }

        public T GetData<T>(string url, Func<Stream, T> responseBuilder)
        {
            using (var stream = _httpClient.GetStreamAsync(new Uri(url))
                .Result)
            {
                return responseBuilder(stream);
            } 
        }
    }
}