using R.Systems.Auth.SharedKernel.Interfaces;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace R.Systems.Auth.SharedKernel.Jwt
{
    public class UserClaimsService : IDependencyInjectionScoped
    {
        protected IEnumerable<Claim> Claims { get; set; } = new List<Claim>();

        public void SetClaims(IEnumerable<Claim> claims)
        {
            Claims = claims;
        }

        public List<string> GetClaim(string type)
        {
            if (JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.ContainsKey(type))
            {
                type = JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap[type];
            }
            List<string> values = new();
            if (Claims != null)
            {
                foreach (Claim claim in Claims)
                {
                    if (claim.Type == type)
                    {
                        values.Add(claim.Value);
                    }
                }
            }
            return values;
        }

        public long GetUserId()
        {
            string? claim = GetClaim(ClaimTypes.NameIdentifier).FirstOrDefault();
            bool parsingResult = long.TryParse(claim, out long userId);
            if (!parsingResult)
            {
                return 0;
            }
            return userId;
        }
    }
}
