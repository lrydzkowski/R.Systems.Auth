using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using R.Systems.Auth.Core.Interfaces;
using R.Systems.Auth.Core.Models.Roles;

namespace R.Systems.Auth.Infrastructure.Db.Repositories;

public class RoleReadRepository : GenericReadRepository<RoleEntity>, IRoleReadRepository
{
    public RoleReadRepository(AuthDbContext dbContext) : base(dbContext)
    {
    }

    protected override Expression<Func<RoleEntity, RoleEntity>> Entities { get; } = role => new RoleEntity
    {
        Id = role.Id,
        RoleKey = role.RoleKey,
        Name = role.Name,
        Description = role.Description
    };

    public async Task<bool> RoleExistsAsync(long roleId)
    {
        RoleEntity? role = await DbContext.Roles.AsNoTracking().Where(role => role.Id == roleId).FirstOrDefaultAsync();
        if (role == null)
        {
            return false;
        }
        return true;
    }
}
