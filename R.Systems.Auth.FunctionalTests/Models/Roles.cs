﻿using R.Systems.Auth.Core.Models;
using System.Collections.Generic;

namespace R.Systems.Auth.FunctionalTests.Models;

public class Roles
{
    public Dictionary<string, Role> Data { get; } = new()
    {
        {
            "admin",
            new Role
            {
                Id = 1,
                RoleKey = "admin",
                Name = "Administrator",
                Description = "System administrator."
            }
        },
        {
            "user",
            new Role
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