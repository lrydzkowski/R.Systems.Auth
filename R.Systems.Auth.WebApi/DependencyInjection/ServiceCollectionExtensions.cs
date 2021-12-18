using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using R.Systems.Auth.Core.DependencyInjection;
using R.Systems.Auth.Infrastructure.Db.DependencyInjection;
using R.Systems.Auth.SharedKernel.DependencyInjection;
using R.Systems.Auth.SharedKernel.Interfaces;
using R.Systems.Auth.SharedKernel.Models;
using R.Systems.Auth.WebApi.Settings;

namespace R.Systems.Auth.WebApi.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutomaticServices();
            services.AddCoreServices();
            services.AddInfrastructureDbServices(configuration["DbConnectionString"]);
            services.AddSettingsServices(configuration);
            services.AddJwtServices();
            services.AddSwaggerServices();
        }

        private static void AddSettingsServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.PropertyName));
            services.AddSingleton<IRsaKeys, RsaKeys>(ctx =>
            {
                JwtSettings? jwtSettings = ctx.GetRequiredService<IOptions<JwtSettings>>()?.Value;
                return new RsaKeys(jwtSettings?.PublicKeyPemFilePath, jwtSettings?.PrivateKeyPemFilePath);
            });
        }
    }
}
