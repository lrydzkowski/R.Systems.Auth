using System.Threading.Tasks;

namespace R.Systems.Auth.Common.Repositories
{
    public interface IUserVerifier
    {
        public Task<bool> AuthenticateUserAsync(string email, string password);
    }
}
