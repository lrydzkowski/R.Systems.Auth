using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;

namespace R.Systems.Auth.Infrastructure.Db;

internal class AuthDbContextFactory : IDesignTimeDbContextFactory<AuthDbContext>
{
    public AuthDbContext CreateDbContext(string[] args)
    {
        var connectionString = GetConnectionStringFromUserSecrets();
        var builder = new DbContextOptionsBuilder<AuthDbContext>();
        builder.UseNpgsql(
            connectionString,
            x => x.MigrationsAssembly(typeof(AuthDbContext).Assembly.FullName)
        );
        return new AuthDbContext(builder.Options);
    }

    private string GetConnectionStringFromUserSecrets()
    {
        IConfigurationRoot config = new ConfigurationBuilder().AddUserSecrets<AuthDbContext>().Build();
        IConfigurationProvider secretProvider = config.Providers.First();
        if (!secretProvider.TryGet("ConnectionString", out string connectionString)
            || connectionString == null
            || connectionString.Length == 0)
        {
            throw new Exception("There is no ConnectionString in user secrets.");
        }
        return connectionString;
    }
}
