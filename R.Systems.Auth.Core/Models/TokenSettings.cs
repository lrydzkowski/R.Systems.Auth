namespace R.Systems.Auth.Core.Models
{
    public class TokenSettings
    {
        public string PrivateKeyPem { get; init; } = "";

        public int AccessTokenLifeTimeInMinutes { get; init; }

        public int RefreshTokenLifeTimeInMinutes { get; set; }
    }
}
