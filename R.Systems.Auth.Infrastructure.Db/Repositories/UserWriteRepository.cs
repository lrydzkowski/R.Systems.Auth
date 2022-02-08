﻿using Microsoft.EntityFrameworkCore;
using Npgsql;
using R.Systems.Auth.Core.Interfaces;
using R.Systems.Auth.Core.Models.Roles;
using R.Systems.Auth.Core.Models.Users;
using R.Systems.Shared.Core.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace R.Systems.Auth.Infrastructure.Db.Repositories;

public class UserWriteRepository : IUserWriteRepository
{
    public UserWriteRepository(
        AuthDbContext dbContext,
        IPasswordHasher passwordHasher,
        ValidationResult validationResult)
    {
        DbContext = dbContext;
        PasswordHasher = passwordHasher;
        ValidationResult = validationResult;
    }

    public AuthDbContext DbContext { get; }
    public IPasswordHasher PasswordHasher { get; }
    public ValidationResult ValidationResult { get; }

    public async Task SaveRefreshTokenAsync(long userId, string refreshToken, double lifetimeInMinutes)
    {
        UserEntity user = await FindUserAsync(userId);
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpireDateTimeUtc = DateTime.UtcNow.AddMinutes(lifetimeInMinutes);
        await DbContext.SaveChangesAsync();
    }

    public async Task SaveIncorrectSignInAsync(long userId)
    {
        UserEntity user = await FindUserAsync(userId);
        user.NumOfIncorrectSignIn = user.NumOfIncorrectSignIn == null ? 1 : (user.NumOfIncorrectSignIn + 1);
        user.LastIncorrectSignInDateTimeUtc = DateTime.UtcNow;
        await DbContext.SaveChangesAsync();
    }

    public async Task ClearIncorrectSignInAsync(long userId)
    {
        UserEntity user = await FindUserAsync(userId);
        user.NumOfIncorrectSignIn = 0;
        user.LastIncorrectSignInDateTimeUtc = null;
        await DbContext.SaveChangesAsync();
    }

    public async Task ChangeUserPasswordAsync(long userId, string newPassword)
    {
        UserEntity? user = await DbContext.Users
            .Where(user => user.Id == userId)
            .Select(user => new UserEntity { Id = user.Id })
            .FirstOrDefaultAsync();
        if (user == null)
        {
            throw new ArgumentException($"User with Userid = {userId} doesn't exist");
        }
        DbContext.Attach(user);
        user.PasswordHash = PasswordHasher.CreatePasswordHash(newPassword);
        await DbContext.SaveChangesAsync();
    }

    public async Task<OperationResult<long>> EditUserAsync(EditUserDto editUserDto, long? userId = null)
    {
        UserEntity? user = new();
        bool isUpdate = userId != null;
        if (isUpdate)
        {
            user = await DbContext.Users
                .Where(user => user.Id == userId)
                .Select(user => new UserEntity
                {
                    Id = user.Id,
                    Roles = user.Roles.Select(role => new RoleEntity { Id = role.Id }).ToList()
                })
                .FirstOrDefaultAsync();
            if (user == null)
            {
                throw new ArgumentException($"User with Userid = {userId} doesn't exist");
            }
            DbContext.Attach(user);
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
            List<RoleEntity> rolesToAdd = new();
            foreach (long roleId in editUserDto.RoleIds)
            {
                RoleEntity? role = user.Roles.FirstOrDefault(role => role.Id == roleId);
                if (role == null)
                {
                    role = new RoleEntity { Id = roleId };
                    DbContext.Attach(role);
                }
                rolesToAdd.Add(role);
            }
            if (isUpdate)
            {
                user.Roles.Clear();
            }
            foreach (RoleEntity roleToAdd in rolesToAdd)
            {
                user.Roles.Add(roleToAdd);
            }
        }
        if (!isUpdate)
        {
            DbContext.Users.Add(user);
        }
        try
        {
            await DbContext.SaveChangesAsync();
        }
        catch (Exception dbUpdateException)
        {
            if (!HandleDbUpdateException(dbUpdateException))
            {
                throw;
            }
            return new OperationResult<long> { Result = false };
        }
        return new OperationResult<long> { Result = true, Data = user.Id };
    }

    public async Task DeleteUserAsync(long userId)
    {
        UserEntity user = new() { Id = userId };
        DbContext.Users.Remove(user);
        await DbContext.SaveChangesAsync();
    }

    private async Task<UserEntity> FindUserAsync(long userId)
    {
        UserEntity? user = await DbContext.Users.FindAsync(userId);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with userId = {userId} doesn't exist");
        }
        return user;
    }

    private bool HandleDbUpdateException(Exception dbUpdateException)
    {
        if (dbUpdateException.InnerException == null)
        {
            return false;
        }
        if (dbUpdateException.InnerException is not PostgresException postgresException)
        {
            return false;
        }
        if (postgresException.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            ValidationResult.Errors.Add(new ErrorInfo(errorKey: "Exists", elementKey: "Email"));
            return true;
        }
        return false;
    }
}
