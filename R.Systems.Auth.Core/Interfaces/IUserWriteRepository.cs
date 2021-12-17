using R.Systems.Auth.Core.Models;
using R.Systems.Auth.SharedKernel.Validation;
using System.Threading.Tasks;

namespace R.Systems.Auth.Core.Interfaces
{
    public interface IUserWriteRepository
    {
        Task SaveRefreshTokenAsync(long userId, string refreshToken, double lifetimeInMinutes);

        Task<OperationResult<long>> EditUserAsync(EditUserDto editUserDto, long? userId = null);

        Task DeleteUserAsync(long userId);
    }
}
