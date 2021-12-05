using R.Systems.Auth.Core.Models;
using System;
using System.Linq.Expressions;

namespace R.Systems.Auth.Infrastructure.Db.Repositories
{
    public class RoleReadRepository : GenericReadRepository<Role>
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
    }
}
