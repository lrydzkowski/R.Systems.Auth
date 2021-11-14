namespace R.Systems.Auth.SharedKernel.Interfaces
{
    public interface IPasswordHasher
    {
        string CreatePasswordHash(string password);

        bool VerifyPasswordHash(string password, string passwordHash);
    }
}
