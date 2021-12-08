using R.Systems.Auth.Core.Interfaces;
using R.Systems.Auth.Core.Models;
using R.Systems.Auth.Core.Validators;
using R.Systems.Auth.SharedKernel.Interfaces;
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

        public async Task<bool> EditUserAsync(EditUserDto editUserDto, long? userId = null)
        {
            bool validationResult = await UserWriteValidator.ValidateWriteAsync(editUserDto, userId);
            if (!validationResult)
            {
                return validationResult;
            }
            await UserWriteRepository.EditUserAsync(editUserDto, userId);
            return validationResult;
        }
    }
}
