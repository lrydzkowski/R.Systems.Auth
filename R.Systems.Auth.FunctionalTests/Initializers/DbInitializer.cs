using R.Systems.Auth.Core.Interfaces;
using R.Systems.Auth.Core.Models;
using R.Systems.Auth.FunctionalTests.Models;
using R.Systems.Auth.Infrastructure.Db;
using System.Collections.Generic;

namespace R.Systems.Auth.FunctionalTests.Initializers
{
    internal static class DbInitializer
    {
        public static void InitData(AuthDbContext dbContext, IPasswordHasher passwordHasher)
        {
            AddUsers(dbContext, passwordHasher);
            dbContext.SaveChanges();
        }

        private static void AddUsers(AuthDbContext dbContext, IPasswordHasher passwordHasher)
        {
            Roles roles = new();
            foreach (KeyValuePair<string, Role> role in roles.Data)
            {
                dbContext.Attach(role.Value);
            }
            List<UserInfo> users = new Users(passwordHasher).Get();
            foreach (UserInfo user in users)
            {
                foreach (string roleKey in user.RoleKeys)
                {
                    user.Roles.Add(roles.Data[roleKey]);
                }
            }
            dbContext.Users.AddRange(users);
        }
    }
}
