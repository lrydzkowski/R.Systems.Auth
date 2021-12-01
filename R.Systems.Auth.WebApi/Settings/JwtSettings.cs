namespace R.Systems.Auth.WebApi.Settings
{
    public class JwtSettings
    {
        public const string PropertyName = "Jwt";

        public double AccessTokenLifeTimeInMinutes { get; init; }

        public double RefreshTokenLifeTimeInMinutes { get; set; }

        public string PrivateKeyPemFilePath { get; init; } = "";

        public string PublicKeyPemFilePath { get; init; } = "";
    }
}
