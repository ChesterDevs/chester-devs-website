using System;
using System.Collections.Generic;
using System.IO;
using ChesterDevs.Core.Aspnet.App.RemoteData;

namespace Tests
{
    public class HttpWrapperFake : IHttpWrapper
    {
        private Stream _getDataSource;
        public List<string> UrlRequests = new List<string>();

        public void SetupGetData(Stream data)
        {
            _getDataSource = data;
        }

        public T GetData<T>(string url, Func<Stream, T> responseBuilder)
        {
            UrlRequests.Add(url);

            return responseBuilder(_getDataSource);
        }
    }
}