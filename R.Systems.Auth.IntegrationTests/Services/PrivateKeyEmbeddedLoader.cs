using R.Systems.Auth.Interfaces;
using System.Reflection;

namespace R.Systems.Auth.IntegrationTests.Services
{
    internal class PrivateKeyEmbeddedLoader : IPrivateKeyLoader
    {
        public string Load(string id)
        {
            ResourceLoader resourceLoader = new();
            string privateKey = resourceLoader.GetEmbeddedResourceString(GetType().Assembly, "private-key.pem");
            return privateKey;
        }
    }
}
