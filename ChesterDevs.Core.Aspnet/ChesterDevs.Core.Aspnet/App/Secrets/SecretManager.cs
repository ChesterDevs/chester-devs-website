using System.IO;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;

namespace ChesterDevs.Core.Aspnet.App.Secrets
{
    public interface ISecretManager
    {
        SecretData Secrets { get; }
    }

    public class SecretManager : ISecretManager
    {
        public SecretData Secrets { get; }

        public SecretManager(IWebHostEnvironment hostingEnvironment)
        {
            var secretPath = $"{hostingEnvironment.ContentRootPath}\\info.json";

            if (File.Exists(secretPath))
            {
                using (var fileStream = File.OpenText(secretPath))
                {
                    var serializer = new JsonSerializer();
                    Secrets = serializer.Deserialize<SecretData>(new JsonTextReader(fileStream));
                }
            }
            else
            {
                Secrets= new SecretData();
            }
        }
    }
}