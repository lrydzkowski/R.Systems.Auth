namespace R.Systems.Auth.SharedKernel.Jwt
{
    public class JwtVerificationSettings
    {
        public const string PropertyName = "Jwt";

        public string PublicKeyPemFilePath { get; init; } = "";
    }
}
