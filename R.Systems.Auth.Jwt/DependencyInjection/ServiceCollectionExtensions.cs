using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Security.Cryptography;

namespace R.Systems.Auth.Jwt.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddJwtServices(this IServiceCollection services, string publicKeyPem)
        {
            services.AddAuthentication(AddAuthenticationAction).AddJwtBearer(AddJwtBearerAction(publicKeyPem));
        }

        private static void AddAuthenticationAction(AuthenticationOptions options)
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }

        private static Action<JwtBearerOptions> AddJwtBearerAction(string publicKeyPem)
        {
            return new Action<JwtBearerOptions>(options =>
            {
                RSA rsa = RSA.Create();
                rsa.ImportFromPem(publicKeyPem.ToCharArray());
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = false,
                    IssuerSigningKey = new RsaSecurityKey(rsa)
                };
            });
        }
    }
}
