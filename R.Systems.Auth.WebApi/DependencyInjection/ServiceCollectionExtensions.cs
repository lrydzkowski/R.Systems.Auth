using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using R.Systems.Auth.Core.DependencyInjection;
using R.Systems.Auth.WebApi.Interfaces;
using R.Systems.Auth.WebApi.Services;
using R.Systems.Auth.WebApi.Settings;
using System.IO;
using R.Systems.Auth.Db.DependencyInjection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace R.Systems.Auth.WebApi.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            AddSettingsServices(services, configuration);
            services.AddScoped<IPrivateKeyLoader, PrivateKeyFileLoader>();
            services.AddScoped<UserService>();
            services.AddScoped<Services.AuthenticationService>();
            services.AddDbServices(configuration["DbConnectionString"]);
            services.AddCoreServices();
            services.AddJwtServices(File.ReadAllText(configuration["Jwt:PublicKeyPemFilePath"]));
        }

        private static void AddSettingsServices(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.PropertyName));
        }
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
