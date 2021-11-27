namespace R.Systems.Auth.WebApi.Features.Tokens
{
    public class GenerateNewTokensResponse
    {
        public string AccessToken { get; init; } = "";

        public string RefreshToken { get; set; } = "";
    }
}
