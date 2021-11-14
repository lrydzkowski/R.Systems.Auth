using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using R.Systems.Auth.Core.Models;
using R.Systems.Auth.Infrastructure.Db.Repositories;
using R.Systems.Auth.SharedKernel.Interfaces;

namespace R.Systems.Auth.Infrastructure.Db.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddDbServices(this IServiceCollection services, string dbConnectionString)
        {
            services.AddScoped<IRepository<User>, UserRepository>();
            services.AddDbContext<AuthDbContext>(
                opts => opts.UseNpgsql(dbConnectionString)
            );
        }
    }
}
