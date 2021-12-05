using R.Systems.Auth.Core.Models;
using System.Collections.Generic;

namespace R.Systems.Auth.FunctionalTests.Models
{
    public class UserInfo : User
    {
        public string Password { get; init; } = "";

        public List<string> RoleKeys { get; set; } = new();
    }
}
