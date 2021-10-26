using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using R.Systems.Auth.Core.DependencyInjection;
using R.Systems.Auth.Services;
using R.Systems.Auth.Settings;

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
        }

        private static void AddSettingsServices(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.PropertyName));
        }
    }
}
