namespace R.Systems.Auth.Core.Interfaces;

public interface IPasswordHasher
{
    string CreatePasswordHash(string password);

    bool VerifyPasswordHash(string password, string passwordHash);
}
