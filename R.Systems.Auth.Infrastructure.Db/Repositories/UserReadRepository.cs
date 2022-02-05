﻿using Microsoft.EntityFrameworkCore;
using R.Systems.Auth.Core.Interfaces;
using R.Systems.Auth.Core.Models;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace R.Systems.Auth.Infrastructure.Db.Repositories;

public class UserReadRepository : GenericReadRepository<User>, IUserReadRepository
{
    public UserReadRepository(AuthDbContext dbContext) : base(dbContext)
    {
    }

    protected override Expression<Func<User, User>> Entities { get; } = user => new User()
    {
        Id = user.Id,
        Email = user.Email,
        FirstName = user.FirstName,
        LastName = user.LastName,
        PasswordHash = user.PasswordHash,
        Roles = user.Roles
            .Select(role => new Role()
            {
                Id = role.Id,
                RoleKey = role.RoleKey,
                Name = role.Name,
                Description = role.Description,
            })
            .ToList()
    };

    protected Expression<Func<User, User>> UserForAuthentication { get; } = user => new User()
    {
        Id = user.Id,
        Email = user.Email,
        PasswordHash = user.PasswordHash,
        NumOfIncorrectSignIn = user.NumOfIncorrectSignIn,
        LastIncorrectSignInDateTimeUtc = user.LastIncorrectSignInDateTimeUtc,
        Roles = user.Roles
            .Select(role => new Role()
            {
                Id = role.Id,
                RoleKey = role.RoleKey
            })
            .ToList()
    };

    public async Task<User?> GetUserForAuthenticationAsync(string email)
    {
        User? user = await DbContext.Users
            .AsNoTracking()
            .Where(user => user.Email == email)
            .Select(UserForAuthentication)
            .FirstOrDefaultAsync();
        return user;
    }

    public async Task<User?> GetUserForAuthenticationAsync(long userId)
    {
        User? user = await DbContext.Users
            .AsNoTracking()
            .Where(user => user.Id == userId)
            .Select(UserForAuthentication)
            .FirstOrDefaultAsync();
        return user;
    }

    public async Task<User?> GetUserWithRefreshTokenAsync(string refreshToken)
    {
        User? user = await DbContext.Users
            .AsNoTracking()
            .Where(user => user.RefreshToken == refreshToken)
            .Select(user => new User()
            {
                Id = user.Id,
                Email = user.Email,
                RefreshToken = refreshToken,
                RefreshTokenExpireDateTimeUtc = user.RefreshTokenExpireDateTimeUtc,
                Roles = user.Roles
                    .Select(role => new Role()
                    {
                        Id = role.Id,
                        RoleKey = role.RoleKey
                    })
                    .ToList()
            })
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
