using System.IO;
using System.Reflection;
using Newtonsoft.Json;

namespace ChesterDevs.Core.Aspnet.App
{
    public class FileSystemWrapper : IFileSystemWrapper
    {
        private static string rootPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public T ReadJson<T>(string path)
        {
            var fullPath = BuildAndEnsurePath(path);

            if (!File.Exists(fullPath))
                return default(T);
            
            using (var streamReader = File.OpenText(fullPath))
            using (var jsonTextReader = new JsonTextReader(streamReader))
            {
                var serializer = new JsonSerializer();
                return serializer.Deserialize<T>(jsonTextReader);
            }
        }

        public void SaveFile(string path, string data)
        {
            var fullPath = BuildAndEnsurePath(path);

            File.WriteAllText(fullPath, data);
        }

        private string BuildAndEnsurePath(string path)
        {
            var cleanPath = path.Replace("/", "\\").TrimStart('\\');
            var fullPath = $"{rootPath}\\{cleanPath}";
            var directory = Path.GetDirectoryName(fullPath);

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            return fullPath;
        }
    }
}