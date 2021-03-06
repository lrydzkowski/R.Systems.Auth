using Microsoft.Extensions.Options;
using R.Systems.Auth.Core.Models.Tokens;
using R.Systems.Auth.WebApi.Settings;
using R.Systems.Shared.Core.Interfaces;

namespace R.Systems.Auth.WebApi.Services;

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
