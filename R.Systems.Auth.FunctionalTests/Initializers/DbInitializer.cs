using R.Systems.Auth.Core.Interfaces;
using R.Systems.Auth.FunctionalTests.Models;
using R.Systems.Auth.Infrastructure.Db;

namespace R.Systems.Auth.FunctionalTests.Initializers
{
    internal static class DbInitializer
    {
        public static void InitData(AuthDbContext dbContext, IPasswordHasher passwordHasher)
        {
            Roles roles = AddRoles(dbContext);
            AddUsers(dbContext, passwordHasher, roles);
            dbContext.SaveChanges();
        }

        private static Roles AddRoles(AuthDbContext dbContext)
        {
            Roles roles = new();
            dbContext.Roles.AddRange(roles);
            return roles;
        }

        private static void AddUsers(AuthDbContext dbContext, IPasswordHasher passwordHasher, Roles roles)
        {
            Users users = new(passwordHasher);
            users[0].Roles.Add(roles[0]);
            dbContext.Users.AddRange(users);
        }
    }
}
