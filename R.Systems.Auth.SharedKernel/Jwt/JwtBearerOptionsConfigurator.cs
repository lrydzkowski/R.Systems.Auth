using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using R.Systems.Auth.SharedKernel.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace R.Systems.Auth.SharedKernel.Jwt
{
    public class JwtBearerOptionsConfigurator : IConfigureNamedOptions<JwtBearerOptions>
    {
        public JwtBearerOptionsConfigurator(IRsaKeys rsaKeys)
        {
            RsaKeys = rsaKeys;
        }

        public IRsaKeys RsaKeys { get; }

        public void Configure(string name, JwtBearerOptions options)
        {
            if (name != JwtBearerDefaults.AuthenticationScheme)
            {
                return;
            }

            RSA rsa = RSA.Create();
            rsa.ImportFromPem(RsaKeys.PublicKey);
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = false,
                IssuerSigningKey = new RsaSecurityKey(rsa)
            };
            options.Events = new JwtBearerEvents
            {
                OnTokenValidated = context =>
                {
                    if (context.SecurityToken is not JwtSecurityToken accessToken)
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
