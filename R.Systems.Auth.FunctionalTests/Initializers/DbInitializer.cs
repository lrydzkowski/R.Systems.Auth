using R.Systems.Auth.Core.Interfaces;
using R.Systems.Auth.Core.Models;
using R.Systems.Auth.FunctionalTests.Models;
using R.Systems.Auth.Infrastructure.Db;
using System.Linq;

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
            dbContext.Roles.AddRange(roles.Data.Select(x => x.Value));
            return roles;
        }

        private static void AddUsers(AuthDbContext dbContext, IPasswordHasher passwordHasher, Roles roles)
        {
            Users users = new(passwordHasher);
            Role adminRole = roles.Data["admin"];
            users.Data["test@lukaszrydzkowski.pl"].Roles.Add(adminRole);
            users.Data["test2@lukaszrydzkowski.pl"].Roles.Add(adminRole);
            users.Data["test3@lukaszrydzkowski.pl"].Roles.Add(adminRole);
            
            dbContext.Users.AddRange(users.Data.Select(x => x.Value));
        }
    }
}
