using R.Systems.Auth.Interfaces;
using System.IO;

namespace R.Systems.Auth.Services
{
    public class PrivateKeyFileLoader : IPrivateKeyLoader
    {
        public string Load(string privateKeyFilePath)
        {
            return File.ReadAllText(privateKeyFilePath);
        }
    }
}
