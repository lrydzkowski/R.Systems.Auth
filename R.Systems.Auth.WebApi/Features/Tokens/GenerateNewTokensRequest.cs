namespace R.Systems.Auth.WebApi.Features.Tokens;

public class GenerateNewTokensRequest
{
    public string RefreshToken { get; init; } = "";
}
