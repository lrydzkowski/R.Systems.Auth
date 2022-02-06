namespace R.Systems.Auth.Core.Models.Roles;

public class RoleDto
{
    public long RoleId { get; init; }

    public string RoleKey { get; init; } = "";

    public string Name { get; init; } = "";

    public string Description { get; init; } = "";
}
