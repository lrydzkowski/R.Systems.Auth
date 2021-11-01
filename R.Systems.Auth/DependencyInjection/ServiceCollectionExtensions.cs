using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using R.Systems.Auth.Core.DependencyInjection;
using R.Systems.Auth.Interfaces;
using R.Systems.Auth.Jwt.DependencyInjection;
using R.Systems.Auth.Services;
using R.Systems.Auth.Settings;
using System.IO;

namespace R.Systems.Auth.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            AddSettingsServices(services, configuration);
            services.AddScoped<IPrivateKeyLoader, PrivateKeyFileLoader>();
            services.AddScoped<UserService>();
            services.AddCoreServices(configuration["DbConnectionString"]);
            services.AddJwtServices(File.ReadAllText(configuration["Jwt:PublicKeyPemFilePath"]));
        }

        private static void AddSettingsServices(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.PropertyName));
        }
    }
}
