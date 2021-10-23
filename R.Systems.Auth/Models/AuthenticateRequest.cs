namespace R.Systems.Auth.Models
{
    public class AuthenticateRequest
    {
        public string Email { get; init; } = "";

        public string Password { get; init; } = "";
    }
}
