using Microsoft.Extensions.Options;
using R.Systems.Auth.Core.Models;
using R.Systems.Auth.Core.Services;
using R.Systems.Auth.SharedKernel.Interfaces;
using R.Systems.Auth.WebApi.Settings;
using System.IO;
using System.Threading.Tasks;

namespace R.Systems.Auth.WebApi.Features.Authentication
{
    public class AuthenticateHandler : IDependencyInjectionScoped
    {
        public AuthenticateHandler(
            AuthenticationService authenticationService,
            ITxtFileLoader fileLoader,
            IOptionsSnapshot<JwtSettings> optionsSnapshot)
        {
            AuthenticationService = authenticationService;
            FileLoader = fileLoader;
            JwtSettings = optionsSnapshot.Value;
        }

        public AuthenticationService AuthenticationService { get; }
        public ITxtFileLoader FileLoader { get; }
        public JwtSettings JwtSettings { get; }

        public async Task<AuthenticateResponse?> HandleAsync(AuthenticateRequest request)
        {
            User? user = await AuthenticationService.AuthenticateAsync(request.Email, request.Password);
            if (user == null)
            {
                return null;
            }
            string? privateKeyPem = FileLoader.Load(JwtSettings.PrivateKeyPemFilePath);
            if (privateKeyPem == null)
            {
                throw new FileNotFoundException("Private key doesn't exist.");
            }
            string accessToken = AuthenticationService.GenerateAccessToken(
                user,
                JwtSettings.AccessTokenLifeTimeInMinutes,
                privateKeyPem
            );
            string refreshToken = AuthenticationService.GenerateRefreshToken();
            await AuthenticationService.SaveRefreshTokenAsync(
                user.UserId,
                refreshToken,
                JwtSettings.RefreshTokenLifeTimeInMinutes
            );
            return new AuthenticateResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }
    }
}
