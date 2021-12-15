using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using R.Systems.Auth.SharedKernel.Interfaces;
using R.Systems.Auth.WebApi.Services;
using R.Systems.Auth.WebApi.Settings;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace R.Systems.Auth.WebApi.DependencyInjection
{
    public class ConfigureJwtBearerOptions : IConfigureNamedOptions<JwtBearerOptions>
    {
        public ConfigureJwtBearerOptions(IRsaKeys rsaKeys, IOptions<JwtSettings> options)
        {
            RsaKeys = rsaKeys;
            JwtSettings = options.Value;
        }

        public IRsaKeys RsaKeys { get; }
        public JwtSettings JwtSettings { get; }

        public void Configure(string name, JwtBearerOptions options)
        {
            if (name != JwtBearerDefaults.AuthenticationScheme)
            {
                return;
            }

            RSA rsa = RSA.Create();
            rsa.ImportFromPem(RsaKeys.PublicKey);
            TokenValidationParameters tokenValidationParameters = new()
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = false,
                IssuerSigningKey = new RsaSecurityKey(rsa)
            };
            options.TokenValidationParameters = tokenValidationParameters;
            options.Events = new JwtBearerEvents
            {
                OnTokenValidated = context =>
                {
                    var accessToken = context.SecurityToken as JwtSecurityToken;
                    if (accessToken == null)
                    {
                        return Task.CompletedTask;
                    }
                    var userClaimsService = (UserClaimsService?)context.HttpContext.RequestServices.GetService(
                        typeof(UserClaimsService)
                    );
                    userClaimsService?.SetClaims(accessToken.Claims);
                    return Task.CompletedTask;
                }
            };
        }

        public void Configure(JwtBearerOptions options)
        {
            Configure(Options.DefaultName, options);
        }
    }
}
