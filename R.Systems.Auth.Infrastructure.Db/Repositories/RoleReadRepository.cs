using Microsoft.EntityFrameworkCore;
using R.Systems.Auth.Core.Interfaces;
using R.Systems.Auth.Core.Models;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace R.Systems.Auth.Infrastructure.Db.Repositories
{
    public class RoleReadRepository : GenericReadRepository<Role>, IRoleReadRepository
    {
        public RoleReadRepository(AuthDbContext dbContext) : base(dbContext)
        {
        }

        protected override Expression<Func<Role, Role>> Entities { get; } = role => new Role()
        {
            Id = role.Id,
            RoleKey = role.RoleKey,
            Name = role.Name,
            Description = role.Description
        };

        public async Task<bool> RoleExistsAsync(long roleId)
        {
            Role? role = await DbContext.Roles.AsNoTracking().Where(role => role.Id == roleId).FirstOrDefaultAsync();
            if (role == null)
            {
                return false;
            }
            return true;
        }
    }
}
