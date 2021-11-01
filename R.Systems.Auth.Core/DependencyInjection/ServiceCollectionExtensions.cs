using Microsoft.Extensions.DependencyInjection;
using R.Systems.Auth.Common.Models;
using R.Systems.Auth.Common.Interfaces;
using R.Systems.Auth.Core.Services;
using R.Systems.Auth.Db.Repositories;
using R.Systems.Auth.Db.DependencyInjection;

namespace R.Systems.Auth.Core.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddCoreServices(this IServiceCollection services, string dbConnectionString)
        {
            services.AddScoped<AuthenticationService>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IRepository<User>, UserRepository>();
            services.AddDbServices(dbConnectionString);
        }
    }
}
