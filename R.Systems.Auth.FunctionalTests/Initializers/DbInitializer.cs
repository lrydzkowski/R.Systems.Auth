using R.Systems.Auth.SharedKernel.Interfaces;
using R.Systems.Auth.Db;
using R.Systems.Auth.FunctionalTests.Models;

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
            Users users = new(passwordHasher);
            dbContext.Users.AddRange(users.Data);
        }
    }
}
