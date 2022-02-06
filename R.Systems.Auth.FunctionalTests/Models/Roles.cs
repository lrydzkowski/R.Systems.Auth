using R.Systems.Auth.Core.Models.Roles;
using System.Collections.Generic;

namespace R.Systems.Auth.FunctionalTests.Models;

public class Roles
{
    public Dictionary<string, RoleEntity> Data { get; } = new()
    {
        {
            "admin",
            new RoleEntity
            {
                Id = 1,
                RoleKey = "admin",
                Name = "Administrator",
                Description = "System administrator."
            }
        },
        {
            "user",
            new RoleEntity
            {
                Id = 2,
                RoleKey = "user",
                Name = "User",
                Description = "Standard user."
            }
        }
    };

    public Roles() { }
}
