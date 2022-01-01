using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;

namespace R.Systems.Auth.FunctionalTests.Services;

internal class JwtTokenService
{
    public string TamperAccessTokenExpireDate(string accessTokenSerialized, int minutesToAdd)
    {
        RSA rsa = RSA.Create();
        rsa.ImportFromPem(new EmbeddedRsaKeys().PrivateKey);
        SigningCredentials signingCredentials = new(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha384)
        {
            CryptoProviderFactory = new CryptoProviderFactory { CacheSignatureProviders = false }
        };

        JwtSecurityTokenHandler tokenHandler = new();
        JwtSecurityToken accessToken = tokenHandler.ReadJwtToken(accessTokenSerialized);
        JwtSecurityToken newAccessToken = new(
            accessToken.Issuer,
            null,
            accessToken.Claims,
            accessToken.ValidFrom.AddMinutes(minutesToAdd),
            accessToken.ValidTo.AddMinutes(minutesToAdd),
            signingCredentials
        );
        string newAccessTokenSerialized = tokenHandler.WriteToken(newAccessToken);

        return newAccessTokenSerialized;
    }
}
