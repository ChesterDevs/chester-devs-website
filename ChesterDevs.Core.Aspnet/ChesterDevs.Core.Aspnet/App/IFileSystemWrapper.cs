using System;
using System.IO;

namespace ChesterDevs.Core.Aspnet.App
{
    public interface IFileSystemWrapper
    {
        T ReadJson<T>(string path);
        void SaveFile(string path, string data);
    }
}