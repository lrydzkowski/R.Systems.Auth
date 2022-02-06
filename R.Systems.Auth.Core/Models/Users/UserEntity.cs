using R.Systems.Auth.Core.Models.Roles;
using R.Systems.Shared.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace R.Systems.Auth.Core.Models.Users;

public class UserEntity : IEntity
{
    public long Id { get; init; }

    public string Email { get; set; } = "";

    public string FirstName { get; set; } = "";

    public string LastName { get; set; } = "";

    public string? PasswordHash { get; set; } = null;

    public string? RefreshToken { get; set; } = null;

    public DateTime? RefreshTokenExpireDateTimeUtc { get; set; } = null;

    public int? NumOfIncorrectSignIn { get; set; }

    public DateTime? LastIncorrectSignInDateTimeUtc { get; set; }

    public ICollection<RoleEntity> Roles { get; set; } = new List<RoleEntity>();
}
