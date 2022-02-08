namespace R.Systems.Auth.Core.Models.Users;

public class ChangeUserPasswordDto
{
    public string? CurrentPassword { get; set; }

    public string NewPassword { get; set; } = "";

    public string RepeatedNewPassword { get; set; } = "";
}
