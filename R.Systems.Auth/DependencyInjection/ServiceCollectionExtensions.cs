using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using R.Systems.Auth.Common.Models;
using R.Systems.Auth.Common.Repositories;
using R.Systems.Auth.Common.Services;
using R.Systems.Auth.Db;
using R.Systems.Auth.Db.Repositories;
using R.Systems.Auth.Services;
using R.Systems.Auth.Settings;

namespace R.Systems.Auth.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            AddDbContext(services, configuration);
            AddSettingsServices(services, configuration);
            services.AddScoped<AuthenticationService>();
            services.AddScoped<PasswordService>();
            services.AddScoped<IRepository<User>, UserRepository>();
            services.AddScoped<IUserVerifier, UserVerifier>();
        }

        private static void AddDbContext(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AuthDbContext>(
                opts => opts.UseNpgsql(configuration["DbConnectionString"])
            );
        }

        private static void AddSettingsServices(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.PropertyName));
        }
    }
}
