using R.Systems.Auth.SharedKernel.Interfaces;
using System.IO;

namespace R.Systems.Auth.SharedKernel.Services
{
    internal class TxtFileLoader : ITxtFileLoader
    {
        public string? Load(string path)
        {
            if (!File.Exists(path))
            {
                return null;
            }
            return File.ReadAllText(path);
        }
    }
}
