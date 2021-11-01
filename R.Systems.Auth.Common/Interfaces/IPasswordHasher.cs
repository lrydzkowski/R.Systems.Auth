namespace R.Systems.Auth.Common.Interfaces
{
    public interface IPasswordHasher
    {
        string CreatePasswordHash(string password);

        bool VerifyPasswordHash(string password, string passwordHash);
    }
}
