using R.Systems.Auth.Core.Models;
using R.Systems.Shared.Core.Validation;
using System.Threading.Tasks;

namespace R.Systems.Auth.Core.Interfaces;

public interface IUserWriteRepository
{
    Task SaveRefreshTokenAsync(long userId, string refreshToken, double lifetimeInMinutes);

    Task SaveIncorrectSignInAsync(long userId);

    Task ClearIncorrectSignInAsync(long userId);

    Task<OperationResult<long>> EditUserAsync(EditUserDto editUserDto, long? userId = null);

    Task DeleteUserAsync(long userId);
}
