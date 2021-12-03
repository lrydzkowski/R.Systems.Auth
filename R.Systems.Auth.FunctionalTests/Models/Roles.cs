using R.Systems.Auth.Core.Models;
using System.Collections.Generic;

namespace R.Systems.Auth.FunctionalTests.Models
{
    public class Roles
    {
        public Dictionary<string, Role> Data { get; } = new()
        {
            {
                "admin",
                new Role
                {
                    RoleKey = "admin",
                    Name = "Administrator",
                    Description = "System administrator"
                }
            }
        };

        public Roles() { }
    }
}
