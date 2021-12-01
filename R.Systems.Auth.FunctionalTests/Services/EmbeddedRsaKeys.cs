using R.Systems.Auth.SharedKernel.Interfaces;
using System.IO;
using System.Reflection;

namespace R.Systems.Auth.FunctionalTests.Services
{
    internal class EmbeddedRsaKeys : IRsaKeys
    {
        public EmbeddedRsaKeys()
        {
            ResourceLoader resourceLoader = new();
            Assembly assembly = GetType().Assembly;
            string publicKeyFileName = "public.pem";
            PublicKey = resourceLoader.GetEmbeddedResourceString(assembly, publicKeyFileName);
            if (PublicKey == null)
            {
                throw new FileNotFoundException($"RSA public key file ({publicKeyFileName}) doesn't exist.");
            }
            string privateKeyFileName = "private.pem";
            PrivateKey = resourceLoader.GetEmbeddedResourceString(assembly, privateKeyFileName);
            if (PrivateKey == null)
            {
                throw new FileNotFoundException($"RSA private key file ({privateKeyFileName}) doesn't exist.");
            }
        }

        public string? PublicKey { get; init; }

        public string? PrivateKey { get; init; }
    }
}
