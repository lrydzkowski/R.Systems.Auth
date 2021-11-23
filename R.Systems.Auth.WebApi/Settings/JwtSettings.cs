namespace R.Systems.Auth.WebApi.Settings
{
    public class JwtSettings
    {
        public const string PropertyName = "Jwt";

        public int AccessTokenLifeTimeInMinutes { get; init; }

        public int RefreshTokenLifeTimeInMinutes { get; set; }

        public string PrivateKeyPemFilePath { get; init; } = "";

        public string PublicKeyPemFilePath { get; init; } = "";
    }
}
