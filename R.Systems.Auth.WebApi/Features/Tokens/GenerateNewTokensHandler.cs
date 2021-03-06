using R.Systems.Auth.Core.Models.Tokens;
using R.Systems.Auth.Core.Services;
using R.Systems.Auth.WebApi.Services;
using R.Systems.Shared.Core.Interfaces;

namespace R.Systems.Auth.WebApi.Features.Tokens;

public class GenerateNewTokensHandler : IDependencyInjectionScoped
{
    public GenerateNewTokensHandler(
        AuthenticationService authenticationService,
        TokenService tokenService)
    {
        AuthenticationService = authenticationService;
        TokenService = tokenService;
    }

    public AuthenticationService AuthenticationService { get; }
    public TokenService TokenService { get; }

    public async Task<GenerateNewTokensResponse?> HandleAsync(GenerateNewTokensRequest request)
    {
        TokenSettings tokenSettings = TokenService.GetTokenSettings();
        Token? token = await AuthenticationService.GenerateNewTokensAsync(request.RefreshToken, tokenSettings);
        if (token == null)
        {
            return null;
        }
        return new GenerateNewTokensResponse
        {
            AccessToken = token.AccessToken,
            RefreshToken = token.RefreshToken
        };
    }
}
