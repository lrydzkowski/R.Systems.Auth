using Microsoft.EntityFrameworkCore;
using R.Systems.Auth.Core.Models;
using R.Systems.Auth.SharedKernel.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace R.Systems.Auth.Infrastructure.Db.Repositories
{
    public class UserRepository : IRepository<User>
    {
        private readonly Expression<Func<User, User>> _users = user => new User()
        {
            UserId = user.UserId,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PasswordHash = user.PasswordHash,
            Roles = user.Roles
                .Select(role => new Role()
                {
                    RoleId = role.RoleId,
                    RoleKey = role.RoleKey,
                    Name = role.Name,
                    Description = role.Description,
                })
                .ToList()
        };

        public UserRepository(AuthDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public AuthDbContext DbContext { get; }

        public async Task<bool> EditAsync(User user)
        {
            await DbContext.AddAsync(user);
            await DbContext.SaveChangesAsync();
            return true;
        }

        public async Task<User?> GetAsync(long userId)
        {
            User? user = await GetQuery().Where(user => user.UserId == userId).FirstOrDefaultAsync();
            return user;
        }

        public async Task<User?> GetAsync(Expression<Func<User, bool>> whereExpression)
        {
            User? user = await GetQuery().Where(whereExpression).FirstOrDefaultAsync();
            return user;
        }

        public async Task<List<User>> GetAsync()
        {
            List<User> users = await GetQuery().ToListAsync();
            return users;
        }

        private IQueryable<User> GetQuery()
        {
            return DbContext.Users.AsNoTracking().Select(_users);
        }
    }
}
