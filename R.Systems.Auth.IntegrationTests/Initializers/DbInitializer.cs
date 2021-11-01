using R.Systems.Auth.Common.Interfaces;
using R.Systems.Auth.Db;
using R.Systems.Auth.IntegrationTests.Models;

namespace R.Systems.Auth.IntegrationTests.Initializers
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
            Users users = new(passwordHasher);
            dbContext.Users.AddRange(users.Data);
        }
    }
}
