using System;
using System.IO;
using ChesterDevs.Core.Aspnet.App.RemoteData;

namespace Tests
{
    public class HttpWrapperFake : IHttpWrapper
    {
        private Stream _getDataSource;

        public void SetupGetData(Stream data)
        {
            _getDataSource = data;
        }

        public T GetData<T>(string url, Func<Stream, T> responseBuilder)
        {
            return responseBuilder(_getDataSource);
        }
    }
}