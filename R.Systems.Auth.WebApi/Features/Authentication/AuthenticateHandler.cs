using R.Systems.Auth.Core.Models;
using R.Systems.Auth.Core.Services;
using R.Systems.Auth.WebApi.Services;
using R.Systems.Shared.Core.Interfaces;

namespace R.Systems.Auth.WebApi.Features.Authentication
{
    public class AuthenticateHandler : IDependencyInjectionScoped
    {
        public AuthenticateHandler(
            AuthenticationService authenticationService,
            TokenService tokenService)
        {
            AuthenticationService = authenticationService;
            TokenService = tokenService;
        }

        public AuthenticationService AuthenticationService { get; }
        public TokenService TokenService { get; }

        public async Task<AuthenticateResponse?> HandleAsync(AuthenticateRequest request)
        {
            TokenSettings tokenSettings = TokenService.GetTokenSettings();
            Token? token = await AuthenticationService.AuthenticateAsync(
                request.Email, request.Password, tokenSettings
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
}
