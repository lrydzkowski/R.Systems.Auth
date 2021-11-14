using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using R.Systems.Auth.Core.DependencyInjection;
using R.Systems.Auth.WebApi.Interfaces;
using R.Systems.Auth.Jwt.DependencyInjection;
using R.Systems.Auth.WebApi.Services;
using R.Systems.Auth.WebApi.Settings;
using System.IO;
using R.Systems.Auth.Db.DependencyInjection;

namespace R.Systems.Auth.WebApi.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            AddSettingsServices(services, configuration);
            services.AddScoped<IPrivateKeyLoader, PrivateKeyFileLoader>();
            services.AddScoped<UserService>();
            services.AddScoped<AuthenticationService>();
            services.AddDbServices(configuration["DbConnectionString"]);
            services.AddCoreServices();
            services.AddJwtServices(File.ReadAllText(configuration["Jwt:PublicKeyPemFilePath"]));
        }

        private static void AddSettingsServices(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.PropertyName));
        }
    }
}
