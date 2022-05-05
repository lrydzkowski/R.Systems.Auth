using System.Collections.Generic;
using R.Systems.Auth.Core.Models.Users;

namespace R.Systems.Auth.FunctionalTests.Models;

public class UserInfo : UserEntity
{
    public string Password { get; init; } = "";

    public List<string> RoleKeys { get; set; } = new();
}
