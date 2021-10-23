namespace R.Systems.Auth.Common.Services
{
    public class PasswordService
    {
        public string CreatePasswordHash(string password)
        {
            return BCrypt.Net.BCrypt.EnhancedHashPassword(password, hashType: BCrypt.Net.HashType.SHA384);
        }

        public bool VerifyPasswordHash(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.EnhancedVerify(password, passwordHash, hashType: BCrypt.Net.HashType.SHA384);
        }
    }
}
