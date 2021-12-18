using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using R.Systems.Auth.SharedKernel.Interfaces;
using R.Systems.Auth.SharedKernel.Jwt;
using R.Systems.Auth.SharedKernel.Models;
using R.Systems.Auth.SharedKernel.Swagger;

namespace R.Systems.Auth.SharedKernel.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddAutomaticServices(this IServiceCollection services)
        {
            AddScopedServices(services);
            AddTransientServices(services);
            AddSingletonServices(services);
        }

        public static void AddJwtSettingsServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtVerificationSettings>(configuration.GetSection(JwtVerificationSettings.PropertyName));
            services.AddSingleton<IRsaKeys, RsaKeys>(ctx =>
            {
                JwtVerificationSettings? jwtSettings = ctx.GetRequiredService<IOptions<JwtVerificationSettings>>()?.Value;
                return new RsaKeys(jwtSettings?.PublicKeyPemFilePath, privateKeyPemFilePath: null);
            });
        }

        public static void AddJwtServices(this IServiceCollection services)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
            services.ConfigureOptions<JwtBearerOptionsConfigurator>();
        }

        public static void AddSwaggerServices(this IServiceCollection services)
        {
            services.AddSwaggerGen();
            services.ConfigureOptions<SwaggerGenOptionsConfigurator>();
        }

        private static void AddScopedServices(IServiceCollection services)
        {
            services.Scan(scan => scan.FromApplicationDependencies()
                    .AddClasses(classes => classes.AssignableTo<IDependencyInjectionScoped>())
                    .AsSelf()
                    .WithScopedLifetime()
                );
        }

        private static void AddTransientServices(IServiceCollection services)
        {
            services.Scan(scan => scan.FromApplicationDependencies()
                    .AddClasses(classes => classes.AssignableTo<IDependencyInjectionTransient>())
                    .AsSelf()
                    .WithTransientLifetime()
                );
        }

        private static void AddSingletonServices(IServiceCollection services)
        {
            services.Scan(scan => scan.FromApplicationDependencies()
                    .AddClasses(classes => classes.AssignableTo<IDependencyInjectionSingleton>())
                    .AsSelf()
                    .WithSingletonLifetime()
                );
        }
    }
}
