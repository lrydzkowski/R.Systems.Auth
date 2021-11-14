using Microsoft.IdentityModel.Tokens;
using R.Systems.Auth.Core.Interfaces;
using R.Systems.Auth.Core.Models;
using R.Systems.Auth.SharedKernel.Interfaces;
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
        public AuthenticationService(IRepository<User> repository, IPasswordHasher passwordHasher)
        {
            Repository = repository;
            PasswordHasher = passwordHasher;
        }

        public IRepository<User> Repository { get; }
        public IPasswordHasher PasswordHasher { get; }

        public async Task<User?> AuthenticateAsync(string email, string password)
        {
            User? user = await Repository.GetAsync(user => user.Email == email);
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

        public string GenerateJwtToken(User user, int lifetimeInMinutes, string privateKeyPem)
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
