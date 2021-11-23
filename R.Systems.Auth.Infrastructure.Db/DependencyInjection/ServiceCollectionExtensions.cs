using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using R.Systems.Auth.Core.Interfaces;
using R.Systems.Auth.Core.Models;
using R.Systems.Auth.Infrastructure.Db.Repositories;
using R.Systems.Auth.SharedKernel.Interfaces;

namespace R.Systems.Auth.Infrastructure.Db.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddInfrastructureDbServices(this IServiceCollection services, string dbConnectionString)
        {
            services.AddRepositories();
            services.AddDbContext<AuthDbContext>(
                opts => opts.UseNpgsql(dbConnectionString)
            );
        }

        private static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUserReadRepository, UserReadRepository>();
            services.AddScoped<IGenericReadRepository<User>, UserReadRepository>();
            services.AddScoped<IUserWriteRepository, UserWriteRepository>();
            services.AddScoped<IGenericWriteRepository<User>, UserWriteRepository>();
        }
    }
}
