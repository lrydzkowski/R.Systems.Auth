using System;

namespace R.Systems.Auth.Core.Models.Users;

public class UserRefreshToken : User
{
    public string? RefreshToken { get; set; } = null;

    public DateTime? RefreshTokenExpireDateTimeUtc { get; set; } = null;
}
