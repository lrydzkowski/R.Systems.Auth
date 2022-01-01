using R.Systems.Shared.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace R.Systems.Auth.Core.Models;

public class User : IEntity
{
    public long Id { get; set; }

    public string Email { get; set; } = "";

    public string FirstName { get; set; } = "";

    public string LastName { get; set; } = "";

    public string? PasswordHash { get; set; } = null;

    public string? RefreshToken { get; set; } = null;

    public DateTime? RefreshTokenExpireDateTimeUtc { get; set; } = null;

    public ICollection<Role> Roles { get; set; } = new List<Role>();
}
