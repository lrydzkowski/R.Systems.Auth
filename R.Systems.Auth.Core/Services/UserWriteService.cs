using R.Systems.Auth.Core.Interfaces;
using R.Systems.Auth.Core.Models;
using R.Systems.Auth.Core.Validators;
using R.Systems.Auth.SharedKernel.Interfaces;
using R.Systems.Auth.SharedKernel.Validation;
using System.Threading.Tasks;

namespace R.Systems.Auth.Core.Services
{
    public class UserWriteService : IDependencyInjectionScoped
    {
        public UserWriteService(IUserWriteRepository userWriteRepository, UserWriteValidator userWriteValidator)
        {
            UserWriteRepository = userWriteRepository;
            UserWriteValidator = userWriteValidator;
        }

        public IUserWriteRepository UserWriteRepository { get; }
        public UserWriteValidator UserWriteValidator { get; }

        public async Task<OperationResult<long>> EditUserAsync(EditUserDto editUserDto, long? userId = null)
        {
            bool validationResult = await UserWriteValidator.ValidateWriteAsync(editUserDto, userId);
            if (!validationResult)
            {
                return new OperationResult<long>() { Result = false };
            }
            long editedUserId = await UserWriteRepository.EditUserAsync(editUserDto, userId);
            return new OperationResult<long>() { Result = true, Data = editedUserId };
        }
    }
}
