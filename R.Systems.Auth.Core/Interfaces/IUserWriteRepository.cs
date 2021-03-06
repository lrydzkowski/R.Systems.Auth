using System.Threading.Tasks;
using R.Systems.Auth.Core.Models.Users;
using R.Systems.Shared.Core.Validation;

namespace R.Systems.Auth.Core.Interfaces;

public interface IUserWriteRepository
{
    Task SaveRefreshTokenAsync(long userId, string refreshToken, double lifetimeInMinutes);

    Task SaveIncorrectSignInAsync(long userId);

    Task ClearIncorrectSignInAsync(long userId);

    Task ChangeUserPasswordAsync(long userId, string newPassword);

    Task<OperationResult<long>> EditUserAsync(EditUserDto editUserDto, long? userId = null);

    Task DeleteUserAsync(long userId);
}
