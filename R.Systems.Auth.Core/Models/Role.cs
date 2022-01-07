using R.Systems.Shared.Core.Interfaces;
using System.Collections.Generic;

namespace R.Systems.Auth.Core.Models;

public class Role : IEntity
{
    public long Id { get; set; }

    public string RoleKey { get; set; } = "";

    public string Name { get; set; } = "";

    public string Description { get; set; } = "";

    public ICollection<User> Users { get; set; } = new List<User>();
}
