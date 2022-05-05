namespace R.Systems.Auth.Core.Models.Tokens;

public class TokenSettings
{
    public string PrivateKeyPem { get; init; } = "";

    public double AccessTokenLifeTimeInMinutes { get; init; }

    public double RefreshTokenLifeTimeInMinutes { get; init; }
}
