using BCrypt.Net;
using R.Systems.Auth.Core.Interfaces;

namespace R.Systems.Auth.Core.Services;

public class PasswordHasher : IPasswordHasher
{
    public string CreatePasswordHash(string password)
    {
        return BCrypt.Net.BCrypt.EnhancedHashPassword(password, hashType: HashType.SHA384);
    }

    public bool VerifyPasswordHash(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.EnhancedVerify(password, passwordHash, hashType: HashType.SHA384);
    }
}
