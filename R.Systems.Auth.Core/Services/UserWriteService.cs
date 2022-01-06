using R.Systems.Auth.Core.Interfaces;
using R.Systems.Auth.Core.Models;
using R.Systems.Auth.Core.Validators;
using R.Systems.Shared.Core.Interfaces;
using R.Systems.Shared.Core.Validation;
using System.Threading.Tasks;

namespace R.Systems.Auth.Core.Services;

public class UserWriteService : IDependencyInjectionScoped
{
    public UserWriteService(IUserWriteRepository userWriteRepository, UserWriteValidator userWriteValidator)
    {
        UserWriteRepository = userWriteRepository;
        UserWriteValidator = userWriteValidator;
    }

    public IUserWriteRepository UserWriteRepository { get; }
    public UserWriteValidator UserWriteValidator { get; }

    public async Task<bool> ChangeUserPasswordAsync(
        long userId, string? currentPassword, string newPassword, string repeatedNewPassword)
    {
        bool validationResult = await UserWriteValidator.ValidateChangePasswordAsync(
            userId, currentPassword, newPassword, repeatedNewPassword
        );
        if (!validationResult)
        {
            return false;
        }
        OperationResult<long> editResult = await EditUserAsync(new EditUserDto { Password = newPassword }, userId);
        return editResult.Result;
    }

    public async Task<OperationResult<long>> EditUserAsync(EditUserDto editUserDto, long? userId = null)
    {
        bool validationResult = await UserWriteValidator.ValidateWriteAsync(editUserDto, userId);
        if (!validationResult)
        {
            return new OperationResult<long>() { Result = false };
        }
        OperationResult<long> operationResult = await UserWriteRepository.EditUserAsync(editUserDto, userId);
        return operationResult;
    }

    public async Task<bool> DeleteUserAsync(long userId, long authorizedUserId)
    {
        bool result = await UserWriteValidator.ValidateDeleteAsync(userId, authorizedUserId);
        if (!result)
        {
            return false;
        }
        await UserWriteRepository.DeleteUserAsync(userId);
        return true;
    }
}
