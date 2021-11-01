using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace R.Systems.Auth.Db.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddDbServices(this IServiceCollection services, string dbConnectionString)
        {
            services.AddDbContext<AuthDbContext>(
                opts => opts.UseNpgsql(dbConnectionString)
            );
        }
    }
}
