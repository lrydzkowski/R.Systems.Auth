using R.Systems.Auth.WebApi.Interfaces;
using System.IO;

namespace R.Systems.Auth.WebApi.Services
{
    public class PrivateKeyFileLoader : IPrivateKeyLoader
    {
        public string Load(string privateKeyFilePath)
        {
            return File.ReadAllText(privateKeyFilePath);
        }
    }
}
