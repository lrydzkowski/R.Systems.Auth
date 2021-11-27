using Microsoft.Extensions.Options;
using R.Systems.Auth.Core.Models;
using R.Systems.Auth.SharedKernel.Interfaces;
using R.Systems.Auth.WebApi.Settings;
using System.IO;

namespace R.Systems.Auth.WebApi.Services
{
    public class TokenService : IDependencyInjectionScoped
    {
        public TokenService(
            ITxtFileLoader fileLoader,
            IOptionsSnapshot<JwtSettings> optionsSnapshot)
        {
            FileLoader = fileLoader;
            JwtSettings = optionsSnapshot.Value;
        }

        public ITxtFileLoader FileLoader { get; }
        public JwtSettings JwtSettings { get; }

        public TokenSettings GetTokenSettings()
        {
            string? privateKeyPem = FileLoader.Load(JwtSettings.PrivateKeyPemFilePath);
            if (privateKeyPem == null)
            {
                throw new FileNotFoundException("Private key doesn't exist.");
            }
            TokenSettings tokenSettings = new()
            {
                PrivateKeyPem = privateKeyPem,
                AccessTokenLifeTimeInMinutes = JwtSettings.AccessTokenLifeTimeInMinutes,
                RefreshTokenLifeTimeInMinutes = JwtSettings.RefreshTokenLifeTimeInMinutes
            };
            return tokenSettings;
        }
    }
}
