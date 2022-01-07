namespace R.Systems.Auth.WebApi.Features.Authentication;

public class AuthenticateResponse
{
    public string AccessToken { get; init; } = "";

    public string RefreshToken { get; set; } = "";
}
