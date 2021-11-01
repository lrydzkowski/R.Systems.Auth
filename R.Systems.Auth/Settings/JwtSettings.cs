namespace R.Systems.Auth.Settings
{
    public class JwtSettings
    {
        public const string PropertyName = "Jwt";

        public int AccessTokenLifeTimeInMinutes { get; init; }

        public string PrivateKeyPemFilePath { get; init; } = "";
    }
}
