using System.Collections.Generic;

namespace R.Systems.Auth.Core.Models;

public class UserDto
{
    public long UserId { get; set; }

    public string Email { get; set; } = "";

    public string FirstName { get; set; } = "";

    public string LastName { get; set; } = "";

    public List<RoleDto> Roles { get; set; } = new();
}
