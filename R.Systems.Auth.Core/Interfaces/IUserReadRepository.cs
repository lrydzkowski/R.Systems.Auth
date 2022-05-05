using System.Threading.Tasks;
using R.Systems.Auth.Core.Models.Users;
using R.Systems.Shared.Core.Interfaces;

namespace R.Systems.Auth.Core.Interfaces;

public interface IUserReadRepository : IGenericReadRepository<UserEntity>
{
    Task<UserAuthentication?> GetUserForAuthenticationAsync(string email);

    Task<UserAuthentication?> GetUserForAuthenticationAsync(long userId);

    Task<UserRefreshToken?> GetUserWithRefreshTokenAsync(string refreshToken);

    Task<bool> UserExistsAsync(string email, long? userId = null);

    Task<bool> UserExistsAsync(long userId);
}
