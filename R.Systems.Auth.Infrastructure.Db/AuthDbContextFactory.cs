using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

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
        if (!secretProvider.TryGet("DbConnectionString", out string connectionString)
            || connectionString == null
            || connectionString.Length == 0)
        {
            throw new Exception("There is no DbConnectionString in user secrets.");
        }
        return connectionString;
    }
}
