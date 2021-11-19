using R.Systems.Auth.SharedKernel.Interfaces;
using System.Reflection;

namespace R.Systems.Auth.FunctionalTests.Services
{
    internal class PrivateKeyEmbeddedLoader : ITxtFileLoader
    {
        public string Load(string id)
        {
            ResourceLoader resourceLoader = new();
            string privateKey = resourceLoader.GetEmbeddedResourceString(GetType().Assembly, "private-key.pem");
            return privateKey;
        }
    }
}
