using System.Reflection;
using Microsoft.EntityFrameworkCore;
using R.Systems.Auth.Core.Models.Roles;
using R.Systems.Auth.Core.Models.Users;

namespace R.Systems.Auth.Infrastructure.Db;

public class AuthDbContext : DbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
    {
    }

    public DbSet<UserEntity> Users => Set<UserEntity>();

    public DbSet<RoleEntity> Roles => Set<RoleEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
