using R.Systems.Auth.Core.Interfaces;
using R.Systems.Auth.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace R.Systems.Auth.Infrastructure.Db.Repositories
{
    public class UserWriteRepository : IUserWriteRepository
    {
        public UserWriteRepository(AuthDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public AuthDbContext DbContext { get; }

        public async Task SaveRefreshTokenAsync(long userId, string refreshToken, int lifetimeInMinutes)
        {
            User? user = await DbContext.Users.FindAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with userId = {userId} doesn't exist");
            }
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpireDateTimeUtc = DateTime.UtcNow.AddMinutes(lifetimeInMinutes);
            await DbContext.SaveChangesAsync();
        }
    }
}
