using R.Systems.Auth.Core.Interfaces;
using R.Systems.Auth.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace R.Systems.Auth.Infrastructure.Db.Repositories
{
    public class UserWriteRepository : IUserWriteRepository
    {
        public UserWriteRepository(AuthDbContext dbContext, IPasswordHasher passwordHasher)
        {
            DbContext = dbContext;
            PasswordHasher = passwordHasher;
        }

        public AuthDbContext DbContext { get; }
        public IPasswordHasher PasswordHasher { get; }

        public async Task SaveRefreshTokenAsync(long userId, string refreshToken, double lifetimeInMinutes)
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

        public async Task<long> EditUserAsync(EditUserDto editUserDto, long? userId = null)
        {
            User user = new();
            bool isUpdate = userId != null;
            if (isUpdate)
            {
                user = await DbContext.Users.FindAsync(userId);
            }
            if (editUserDto.Email != null)
            {
                user.Email = editUserDto.Email;
            }
            if (editUserDto.FirstName != null)
            {
                user.FirstName = editUserDto.FirstName;
            }
            if (editUserDto.LastName != null)
            {
                user.LastName = editUserDto.LastName;
            }
            if (editUserDto.Password != null)
            {
                user.PasswordHash = PasswordHasher.CreatePasswordHash(editUserDto.Password);
            }
            if (editUserDto.RoleIds != null)
            {
                foreach (long roleId in editUserDto.RoleIds)
                {
                    Role role = await DbContext.Roles.FindAsync(roleId);
                    user.Roles.Add(role);
                }
            }
            if (!isUpdate)
            {
                DbContext.Users.Add(user);
            }
            await DbContext.SaveChangesAsync();
            return user.Id;
        }
    }
}
