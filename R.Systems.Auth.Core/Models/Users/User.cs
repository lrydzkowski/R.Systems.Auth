using System.Collections.Generic;
using R.Systems.Auth.Core.Models.Roles;

namespace R.Systems.Auth.Core.Models.Users;

public class User
{
    public long Id { get; init; }

    public string Email { get; init; } = "";

    public ICollection<RoleKey> RoleKeys { get; set; } = new List<RoleKey>();
}
