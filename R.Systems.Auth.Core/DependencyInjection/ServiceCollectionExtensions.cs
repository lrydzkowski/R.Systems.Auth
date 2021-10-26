using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using R.Systems.Auth.Common.Models;
using R.Systems.Auth.Common.Repositories;
using R.Systems.Auth.Core.Services;
using R.Systems.Auth.Db;
using R.Systems.Auth.Db.Repositories;

namespace R.Systems.Auth.Core.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddCoreServices(this IServiceCollection services, string dbConnectionString)
        {
            AddDbContext(services, dbConnectionString);
            services.AddScoped<AuthenticationService>();
            services.AddScoped<PasswordService>();
            services.AddScoped<IRepository<User>, UserRepository>();
        }

        private static void AddDbContext(IServiceCollection services, string dbConnectionString)
        {
            services.AddDbContext<AuthDbContext>(
                opts => opts.UseNpgsql(dbConnectionString)
            );
        }
    }
}
