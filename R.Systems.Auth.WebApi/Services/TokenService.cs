using Microsoft.Extensions.Options;
using R.Systems.Auth.Core.Models;
using R.Systems.Auth.SharedKernel.Interfaces;
using R.Systems.Auth.WebApi.Settings;

namespace R.Systems.Auth.WebApi.Services
{
    public class TokenService : IDependencyInjectionScoped
    {
        public TokenService(
            IRsaKeys rsaKeys,
            IOptionsSnapshot<JwtSettings> optionsSnapshot)
        {
            RsaKeys = rsaKeys;
            JwtSettings = optionsSnapshot.Value;
        }

        public IRsaKeys RsaKeys { get; }
        public JwtSettings JwtSettings { get; }

        public TokenSettings GetTokenSettings()
        {
            TokenSettings tokenSettings = new()
            {
                PrivateKeyPem = RsaKeys.PrivateKey ?? "",
                AccessTokenLifeTimeInMinutes = JwtSettings.AccessTokenLifeTimeInMinutes,
                RefreshTokenLifeTimeInMinutes = JwtSettings.RefreshTokenLifeTimeInMinutes
            };
            return tokenSettings;
        }
    }
}
