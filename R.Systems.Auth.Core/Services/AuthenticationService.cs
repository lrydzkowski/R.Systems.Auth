using Microsoft.IdentityModel.Tokens;
using R.Systems.Auth.Core.Interfaces;
using R.Systems.Auth.Core.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace R.Systems.Auth.Core.Services
{
    public class AuthenticationService
    {
        public AuthenticationService(
            IUserReadRepository userReadRepository,
            IPasswordHasher passwordHasher,
            IUserWriteRepository userWriteRepository)
        {
            UserReadRepository = userReadRepository;
            PasswordHasher = passwordHasher;
            UserWriteRepository = userWriteRepository;
        }

        public IUserReadRepository UserReadRepository { get; }
        public IPasswordHasher PasswordHasher { get; }
        public IUserWriteRepository UserWriteRepository { get; }

        public async Task<Token?> AuthenticateAsync(string email, string password, TokenSettings tokenSettings)
        {
            User? user = await AuthenticateAsync(email, password);
            if (user == null)
            {
                return null;
            }
            return await GenerateTokensAsync(user, tokenSettings);
        }

        public async Task<Token?> GenerateNewTokensAsync(string refreshToken, TokenSettings tokenSettings)
        {
            User? user = await UserReadRepository.GetUserWithRefreshTokenAsync(refreshToken);
            if (user == null)
            {
                return null;
            }
            return await GenerateTokensAsync(user, tokenSettings);
        }

        private async Task<Token> GenerateTokensAsync(User user, TokenSettings tokenSettings)
        {
            string accessToken = GenerateAccessToken(
                user,
                tokenSettings.AccessTokenLifeTimeInMinutes,
                tokenSettings.PrivateKeyPem
            );
            string refreshToken = GenerateRefreshToken();
            await SaveRefreshTokenAsync(
                user.UserId,
                refreshToken,
                tokenSettings.RefreshTokenLifeTimeInMinutes
            );
            return new Token
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        private async Task<User?> AuthenticateAsync(string email, string password)
        {
            User? user = await UserReadRepository.GetUserForAuthenticationAsync(email);
            if (user == null)
            {
                return null;
            }
            if (user.PasswordHash == null)
            {
                return user;
            }
            if (!PasswordHasher.VerifyPasswordHash(password, user.PasswordHash))
            {
                return null;
            }
            return user;
        }

        private string GenerateAccessToken(User user, int lifetimeInMinutes, string privateKeyPem)
        {
            IDictionary<string, object> claims = GenerateUsersClaims(user);
            DateTime? expires = DateTime.Now.AddMinutes(lifetimeInMinutes);

            using RSA rsa = RSA.Create();
            rsa.ImportFromPem(privateKeyPem.ToCharArray());
            SigningCredentials signingCredentials = new(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha384)
            {
                CryptoProviderFactory = new CryptoProviderFactory { CacheSignatureProviders = false }
            };

            JwtSecurityTokenHandler tokenHandler = new();
            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Claims = claims,
                Expires = expires,
                SigningCredentials = signingCredentials
            };
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            byte[] randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            string refreshToken = Convert.ToBase64String(randomNumber);
            return refreshToken;
        }

        private async Task SaveRefreshTokenAsync(long userId, string refreshToken, int lifetimeInMinutes)
        {
            await UserWriteRepository.SaveRefreshTokenAsync(userId, refreshToken, lifetimeInMinutes);
        }

        private Dictionary<string, object> GenerateUsersClaims(User user)
        {
            List<string> rolesKeys = new();
            foreach (Role role in user.Roles)
            {
                rolesKeys.Add(role.RoleKey);
            }
            Dictionary<string, object> claims = new()
            {
                { ClaimTypes.NameIdentifier, user.UserId },
                { ClaimTypes.Email, user.Email },
                { ClaimTypes.Role, rolesKeys }
            };
            return claims;
        }
    }
}
