using Microsoft.Extensions.Options;
using R.Systems.Auth.Core.DependencyInjection;
using R.Systems.Auth.Infrastructure.Db.DependencyInjection;
using R.Systems.Auth.WebApi.Settings;
using R.Systems.Shared.Core.DependencyInjection;
using R.Systems.Shared.Core.Interfaces;
using R.Systems.Shared.Core.Models;
using R.Systems.Shared.WebApi.DependencyInjection;

namespace R.Systems.Auth.WebApi.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static void AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAutomaticServices();
        services.AddCoreServices();
        services.AddInfrastructureDbServices(configuration["DbConnectionString"]);
        services.AddSettingsServices(configuration);
        services.AddJwtServices();
        services.AddSwaggerServices(swaggerPageTitle: "R.Systems.Auth.WebApi");
    }

    private static void AddSettingsServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.PropertyName));
        services.Configure<UserSettings>(configuration.GetSection(UserSettings.PropertyName));
        services.AddSingleton<IRsaKeys, RsaKeys>(ctx =>
        {
            JwtSettings jwtSettings = ctx.GetRequiredService<IOptions<JwtSettings>>().Value;
            return new RsaKeys(jwtSettings.PublicKeyPemFilePath, jwtSettings.PrivateKeyPemFilePath);
        });
    }
}
