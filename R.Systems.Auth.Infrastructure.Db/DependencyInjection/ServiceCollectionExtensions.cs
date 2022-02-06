using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using R.Systems.Auth.Core.Interfaces;
using R.Systems.Auth.Core.Models.Roles;
using R.Systems.Auth.Core.Models.Users;
using R.Systems.Auth.Infrastructure.Db.Repositories;
using R.Systems.Shared.Core.Interfaces;

namespace R.Systems.Auth.Infrastructure.Db.DependencyInjection;

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
        services.AddScoped<IGenericReadRepository<UserEntity>, UserReadRepository>();
        services.AddScoped<IUserWriteRepository, UserWriteRepository>();

        services.AddScoped<IRoleReadRepository, RoleReadRepository>();
        services.AddScoped<IGenericReadRepository<RoleEntity>, RoleReadRepository>();
    }
}
