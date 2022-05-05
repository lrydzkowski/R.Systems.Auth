using System;

namespace R.Systems.Auth.Core.Models.Users;

public class UserAuthentication : User
{
    public string? PasswordHash { get; set; } = null;

    public int? NumOfIncorrectSignIn { get; set; }

    public DateTime? LastIncorrectSignInDateTimeUtc { get; set; }
}
