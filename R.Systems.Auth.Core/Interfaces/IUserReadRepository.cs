using R.Systems.Auth.Core.Models;
using R.Systems.Auth.SharedKernel.Interfaces;
using System.Threading.Tasks;

namespace R.Systems.Auth.Core.Interfaces
{
    public interface IUserReadRepository : IGenericReadRepository<User>
    {
        Task<User?> GetUserForAuthenticationAsync(string email);

        Task<User?> GetUserWithRefreshTokenAsync(string refreshToken);

        Task<bool> UserExistsAsync(string email, long? userId = null);

        Task<bool> UserExistsAsync(long userId);
    }
}
