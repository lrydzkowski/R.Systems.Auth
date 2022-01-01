namespace R.Systems.Auth.Core.Models;

public class RoleDto
{
    public long RoleId { get; set; }

    public string RoleKey { get; set; } = "";

    public string Name { get; set; } = "";

    public string Description { get; set; } = "";
}
