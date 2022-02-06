using Microsoft.EntityFrameworkCore;
using R.Systems.Auth.Core.Interfaces;
using R.Systems.Auth.Core.Models.Roles;
using R.Systems.Auth.Core.Models.Users;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace R.Systems.Auth.Infrastructure.Db.Repositories;

public class UserReadRepository : GenericReadRepository<UserEntity>, IUserReadRepository
{
    public UserReadRepository(AuthDbContext dbContext) : base(dbContext)
    {
    }

    protected override Expression<Func<UserEntity, UserEntity>> Entities { get; } = user => new UserEntity()
    {
        Id = user.Id,
        Email = user.Email,
        FirstName = user.FirstName,
        LastName = user.LastName,
        PasswordHash = user.PasswordHash,
        Roles = user.Roles
            .Select(role => new RoleEntity()
            {
                Id = role.Id,
                RoleKey = role.RoleKey,
                Name = role.Name,
                Description = role.Description,
            })
            .ToList()
    };

    protected Expression<Func<UserEntity, UserAuthentication>> UserForAuthentication { get; }
        = user => new UserAuthentication()
        {
            Id = user.Id,
            Email = user.Email,
            PasswordHash = user.PasswordHash,
            NumOfIncorrectSignIn = user.NumOfIncorrectSignIn,
            LastIncorrectSignInDateTimeUtc = user.LastIncorrectSignInDateTimeUtc,
            RoleKeys = user.Roles
                .Select(role => new RoleKey()
                {
                    Id = role.Id,
                    Key = role.RoleKey
                })
                .ToList()
        };

    protected Expression<Func<UserEntity, UserRefreshToken>> UserForRefreshingToken { get; }
        = user => new UserRefreshToken()
        {
            Id = user.Id,
            Email = user.Email,
            RefreshToken = user.RefreshToken,
            RefreshTokenExpireDateTimeUtc = user.RefreshTokenExpireDateTimeUtc,
            RoleKeys = user.Roles
                .Select(role => new RoleKey()
                {
                    Id = role.Id,
                    Key = role.RoleKey
                })
                .ToList()
        };

    public async Task<UserAuthentication?> GetUserForAuthenticationAsync(string email)
    {
        UserAuthentication? user = await DbContext.Users
            .AsNoTracking()
            .Where(user => user.Email == email)
            .Select(UserForAuthentication)
            .FirstOrDefaultAsync();
        return user;
    }

    public async Task<UserAuthentication?> GetUserForAuthenticationAsync(long userId)
    {
        UserAuthentication? user = await DbContext.Users
            .AsNoTracking()
            .Where(user => user.Id == userId)
            .Select(UserForAuthentication)
            .FirstOrDefaultAsync();
        return user;
    }

    public async Task<UserRefreshToken?> GetUserWithRefreshTokenAsync(string refreshToken)
    {
        UserRefreshToken? user = await DbContext.Users
            .AsNoTracking()
            .Where(user => user.RefreshToken == refreshToken)
            .Select(UserForRefreshingToken)
            .FirstOrDefaultAsync();
        return user;
    }

    public async Task<bool> UserExistsAsync(string email, long? userId = null)
    {
        var query = DbContext.Users.AsNoTracking().Where(user => user.Email == email);
        if (userId != null)
        {
            query = query.Where(user => user.Id != userId);
        }
        bool exists = await query.Select(OnlyId).AnyAsync();
        return exists;
    }

    public async Task<bool> UserExistsAsync(long userId)
    {
        bool exists = await DbContext.Users
            .AsNoTracking()
            .Where(user => user.Id == userId)
            .Select(OnlyId)
            .AnyAsync();
        return exists;
    }
}
