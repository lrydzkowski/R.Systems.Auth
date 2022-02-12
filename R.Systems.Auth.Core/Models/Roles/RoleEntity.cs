using System.Collections.Generic;
using R.Systems.Auth.Core.Models.Users;
using R.Systems.Shared.Core.Interfaces;

namespace R.Systems.Auth.Core.Models.Roles;

public class RoleEntity : IEntity
{
    public long Id { get; init; }

    public string RoleKey { get; set; } = "";

    public string Name { get; set; } = "";

    public string Description { get; set; } = "";

    public ICollection<UserEntity> Users { get; set; } = new List<UserEntity>();
}
