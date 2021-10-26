using Microsoft.Extensions.Options;
using R.Systems.Auth.Common.Models;
using R.Systems.Auth.Common.Repositories;
using R.Systems.Auth.Core.Services;
using R.Systems.Auth.Models;
using R.Systems.Auth.Settings;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace R.Systems.Auth.Services
{
    public class UserService
    {
        public UserService(
            IRepository<User> repository,
            AuthenticationService authenticationService,
            IPrivateKeyLoader privateKeyLoader,
            IOptionsSnapshot<JwtSettings> optionsSnapshot)
        {
            Repository = repository;
            AuthenticationService = authenticationService;
            PrivateKeyLoader = privateKeyLoader;
            JwtSettings = optionsSnapshot.Value;
        }

        public IRepository<User> Repository { get; }
        public AuthenticationService AuthenticationService { get; }
        public IPrivateKeyLoader PrivateKeyLoader { get; }
        public JwtSettings JwtSettings { get; }

        public async Task<string?> AuthenticateAsync(AuthenticateRequest request)
        {
            User? user = await AuthenticationService.AuthenticateAsync(request.Email, request.Password);
            if (user == null)
            {
                return null;
            }
            string privateKeyPem = PrivateKeyLoader.Load(JwtSettings.PrivateKeyPemFilePath);
            string? jwtToken = AuthenticationService.GenerateJwtToken(
                user,
                JwtSettings.AccessTokenLifeTimeInMinutes,
                privateKeyPem
            );
            return jwtToken;
        }

        public async Task<User?> GetUserAsync(long userId)
        {
            User? user = await Repository.GetAsync(userId);
            return user;
        }

        public async Task<List<User>> GetUsersAsync()
        {
            List<User> users = await Repository.GetAsync();
            return users;
        }
    }
}
