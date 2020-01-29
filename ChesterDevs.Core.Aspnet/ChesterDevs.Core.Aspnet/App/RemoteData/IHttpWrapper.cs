using System;
using System.IO;

namespace ChesterDevs.Core.Aspnet.App.RemoteData
{
    public interface IHttpWrapper
    {
        T GetData<T>(string url, Func<Stream, T> responseBuilder);
    }
}