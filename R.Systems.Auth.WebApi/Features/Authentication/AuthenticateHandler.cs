using Microsoft.Extensions.Options;
using R.Systems.Auth.Core.Models;
using R.Systems.Auth.Core.Models.Tokens;
using R.Systems.Auth.Core.Models.Users;
using R.Systems.Auth.Core.Services;
using R.Systems.Auth.WebApi.Services;
using R.Systems.Shared.Core.Interfaces;

namespace R.Systems.Auth.WebApi.Features.Authentication;

public class AuthenticateHandler : IDependencyInjectionScoped
{
    public AuthenticateHandler(
        AuthenticationService authenticationService,
        TokenService tokenService,
        IOptionsSnapshot<Settings.UserSettings> options)
    {
        AuthenticationService = authenticationService;
        TokenService = tokenService;
        UserSettings = options.Value;
    }

    public AuthenticationService AuthenticationService { get; }
    public TokenService TokenService { get; }
    public Settings.UserSettings UserSettings { get; }

    public async Task<AuthenticateResponse?> HandleAsync(AuthenticateRequest request)
    {
        TokenSettings tokenSettings = TokenService.GetTokenSettings();
        Token? token = await AuthenticationService.AuthenticateAsync(
            request.Email,
            request.Password,
            tokenSettings,
            new UserSettings
            {
                MaxNumOfIncorrectLoginsBeforeBlock = UserSettings.MaxNumOfIncorrectLoginsBeforeBlock,
                BlockDurationInMinutes = UserSettings.BlockDurationInMinutes
            }
        );
        if (token == null)
        {
            return null;
        }
        return new AuthenticateResponse
        {
            AccessToken = token.AccessToken,
            RefreshToken = token.RefreshToken
        };
    }
}
