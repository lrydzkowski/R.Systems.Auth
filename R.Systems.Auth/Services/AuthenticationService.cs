using Microsoft.Extensions.Options;
using R.Systems.Auth.Common.Models;
using R.Systems.Auth.Interfaces;
using R.Systems.Auth.Models;
using R.Systems.Auth.Settings;
using System.Threading.Tasks;
using CoreAuthenticationService = R.Systems.Auth.Core.Services.AuthenticationService;

namespace R.Systems.Auth.Services
{
    public class AuthenticationService
    {
        public AuthenticationService(
            CoreAuthenticationService coreAuthenticationService,
            IPrivateKeyLoader privateKeyLoader,
            IOptionsSnapshot<JwtSettings> optionsSnapshot)
        {
            CoreAuthenticationService = coreAuthenticationService;
            PrivateKeyLoader = privateKeyLoader;
            JwtSettings = optionsSnapshot.Value;
        }

        public CoreAuthenticationService CoreAuthenticationService { get; }
        public IPrivateKeyLoader PrivateKeyLoader { get; }
        public JwtSettings JwtSettings { get; }

        public async Task<string?> AuthenticateAsync(AuthenticateRequest request)
        {
            User? user = await CoreAuthenticationService.AuthenticateAsync(request.Email, request.Password);
            if (user == null)
            {
                return null;
            }
            string privateKeyPem = PrivateKeyLoader.Load(JwtSettings.PrivateKeyPemFilePath);
            string? jwtToken = CoreAuthenticationService.GenerateJwtToken(
                user,
                JwtSettings.AccessTokenLifeTimeInMinutes,
                privateKeyPem
            );
            return jwtToken;
        }
    }
}
