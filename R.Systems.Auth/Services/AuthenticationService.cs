using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using R.Systems.Auth.Common.Models;
using R.Systems.Auth.Common.Repositories;
using R.Systems.Auth.Settings;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace R.Systems.Auth.Services
{
    public class AuthenticationService
    {
        public AuthenticationService(
            IRepository<User> repository,
            IUserVerifier userVerifier,
            IOptionsSnapshot<JwtSettings> optionsSnapshot)
        {
            Repository = repository;
            UserVerifier = userVerifier;
            JwtSettings = optionsSnapshot.Value;
        }

        public IRepository<User> Repository { get; }
        public IUserVerifier UserVerifier { get; }
        public JwtSettings JwtSettings { get; }

        public async Task<bool> AuthenticateAsync(string email, string password)
        {
            bool isAuthenticated = await UserVerifier.AuthenticateUserAsync(email, password);
            return isAuthenticated;
        }

        public async Task<string?> GenerateJwtTokenAsync(string email)
        {
            User? user = await Repository.GetAsync(user => user.Email == email);
            if (user == null)
            {
                return null;
            }
            IDictionary<string, object> claims = GenerateUsersClaims(user);
            DateTime? expires = DateTime.Now.AddMinutes(JwtSettings.AccessTokenLifeTimeInMinutes);

            string privateKey = File.ReadAllText(JwtSettings.PrivateKeyPemFilePath);
            using RSA rsa = RSA.Create();
            rsa.ImportFromPem(privateKey.ToCharArray());
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
