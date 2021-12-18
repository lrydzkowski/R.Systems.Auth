using R.Systems.Auth.SharedKernel.Interfaces;
using System.IO;

namespace R.Systems.Auth.SharedKernel.Models
{
    public class RsaKeys : IRsaKeys
    {
        public RsaKeys(string? publicKeyPemFilePath, string? privateKeyPemFilePath)
        {
            PublicKey = publicKeyPemFilePath;
            PrivateKey = privateKeyPemFilePath;
        }

        private string? _publicKey = null;
        public string? PublicKey
        {
            get
            {
                return _publicKey;
            }
            init
            {
                if (value == null)
                {
                    return;
                }
                if (!File.Exists(value))
                {
                    throw new FileNotFoundException($"RSA public key pem file ({value}) doesn't exist.");
                }
                _publicKey = File.ReadAllText(value);
            }
        }

        private string? _privateKey = null;
        public string? PrivateKey
        {
            get
            {
                return _privateKey;
            }
            init
            {
                if (value == null)
                {
                    return;
                }
                if (!File.Exists(value))
                {
                    throw new FileNotFoundException($"RSA private key pem file ({value}) doesn't exist.");
                }
                _privateKey = File.ReadAllText(value);
            }
        }
    }
}
