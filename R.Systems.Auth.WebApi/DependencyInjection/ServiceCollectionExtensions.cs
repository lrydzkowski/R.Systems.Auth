using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using R.Systems.Auth.Core.DependencyInjection;
using R.Systems.Auth.Infrastructure.Db.DependencyInjection;
using R.Systems.Auth.SharedKernel.DependencyInjection;
using R.Systems.Auth.WebApi.Settings;
using System;
using System.IO;
using System.Security.Cryptography;

namespace R.Systems.Auth.WebApi.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutomaticServices();
            services.AddSharedKernelServices();
            services.AddCoreServices();
            services.AddInfrastructureDbServices(configuration["DbConnectionString"]);
            services.AddSettingsServices(configuration);
            services.AddJwtServices(File.ReadAllText(configuration["Jwt:PublicKeyPemFilePath"]));
        }

        private static void AddSettingsServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.PropertyName));
        }

        private static void AddJwtServices(this IServiceCollection services, string publicKeyPem)
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
