using Microsoft.EntityFrameworkCore;
using R.Systems.Auth.Common.Models;
using R.Systems.Auth.Common.Repositories;
using R.Systems.Auth.Common.Services;
using System.Linq;
using System.Threading.Tasks;

namespace R.Systems.Auth.Db.Repositories
{
    public class UserVerifier : IUserVerifier
    {
        public UserVerifier(AuthDbContext dbContext, PasswordService passwordService)
        {
            DbContext = dbContext;
            PasswordService = passwordService;
        }

        public AuthDbContext DbContext { get; }
        public PasswordService PasswordService { get; }

        public async Task<bool> AuthenticateUserAsync(string email, string password)
        {
            User? user = await DbContext.Users
                .AsNoTracking()
                .Select(user => new User()
                {
                    UserId = user.UserId,
                    Email = user.Email,
                    PasswordHash = user.PasswordHash
                })
                .Where(user => user.Email == email)
                .FirstOrDefaultAsync();
            if (user == null)
            {
                return false;
            }
            if (user.PasswordHash == null)
            {
                return true;
            }
            bool isAuthenticated = PasswordService.VerifyPasswordHash(password, user.PasswordHash);
            return isAuthenticated;
        }
    }
}
