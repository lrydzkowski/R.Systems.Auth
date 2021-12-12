using R.Systems.Auth.Core.Interfaces;
using R.Systems.Auth.Core.Models;
using R.Systems.Auth.SharedKernel.Interfaces;
using R.Systems.Auth.SharedKernel.Validation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace R.Systems.Auth.Core.Validators
{
    public class UserWriteValidator : IDependencyInjectionScoped
    {
        public UserWriteValidator(
            IUserReadRepository userReadRepository,
            IRoleReadRepository roleReadRepository,
            ValidationResult validationResult)
        {
            UserReadRepository = userReadRepository;
            RoleReadRepository = roleReadRepository;
            ValidationResult = validationResult;
        }

        public IUserReadRepository UserReadRepository { get; }
        public IRoleReadRepository RoleReadRepository { get; }
        public ValidationResult ValidationResult { get; }

        public async Task<bool> ValidateWriteAsync(EditUserDto editUserDto, long? userId = null)
        {
            bool result = true;
            bool isUpdate = userId != null;
            if (isUpdate)
            {
                result &= await ValidateUserIdAsync((long)userId!);
            }
            result &= await ValidateEmailAsync(editUserDto.Email, userId, isUpdate);
            result &= ValidateFirstName(editUserDto.FirstName, isUpdate);
            result &= ValidateLastName(editUserDto.LastName, isUpdate);
            result &= ValidatePassword(editUserDto.Password, isUpdate);
            result &= await ValidateRolesAsync(editUserDto.RoleIds, isUpdate);
            return result;
        }

        private async Task<bool> ValidateUserIdAsync(long userId)
        {
            if (!(await UserReadRepository.UserExistsAsync(userId)))
            {
                ValidationResult.Errors.Add(new ErrorInfo(errorKey: "NotExist", "UserId"));
                return false;
            }
            return true;
        }

        private async Task<bool> ValidateEmailAsync(string? email, long? userId, bool isUpdate)
        {
            string elementKey = "Email";
            if (email == null && isUpdate)
            {
                return true;
            }
            email = email?.Trim();
            if (email == null || email.Length == 0)
            {
                ValidationResult.Errors.Add(new ErrorInfo(errorKey: "IsRequired", elementKey));
                return false;
            }
            if (!email.Contains('@'))
            {
                ValidationResult.Errors.Add(new ErrorInfo(errorKey: "WrongStructure", elementKey));
                return false;
            }
            if (email.Length > 200)
            {
                ValidationResult.Errors.Add(new ErrorInfo(errorKey: "TooLong", elementKey));
                return false;
            }
            if (await UserReadRepository.UserExistsAsync(email, userId))
            {
                ValidationResult.Errors.Add(new ErrorInfo(errorKey: "Exists", elementKey));
                return false;
            }
            return true;
        }

        private bool ValidateFirstName(string? firstName, bool isUpdate)
        {
            string elementKey = "FirstName";
            if (firstName == null && isUpdate)
            {
                return true;
            }
            firstName = firstName?.Trim();
            if (firstName == null || firstName.Length == 0)
            {
                ValidationResult.Errors.Add(new ErrorInfo(errorKey: "IsRequired", elementKey));
                return false;
            }
            if (firstName.Length > 200)
            {
                ValidationResult.Errors.Add(new ErrorInfo(errorKey: "TooLong", elementKey));
                return false;
            }
            return true;
        }

        private bool ValidateLastName(string? lastName, bool isUpdate)
        {
            string elementKey = "LastName";
            if (lastName == null && isUpdate)
            {
                return true;
            }
            lastName = lastName?.Trim();
            if (lastName == null || lastName.Length == 0)
            {
                ValidationResult.Errors.Add(new ErrorInfo(errorKey: "IsRequired", elementKey));
                return false;
            }
            if (lastName.Length > 200)
            {
                ValidationResult.Errors.Add(new ErrorInfo(errorKey: "TooLong", elementKey));
                return false;
            }
            return true;
        }

        private bool ValidatePassword(string? password, bool isUpdate)
        {
            string elementKey = "Password";
            if (password == null && isUpdate)
            {
                return true;
            }
            password = password?.Trim();
            if (password == null || password.Length == 0)
            {
                ValidationResult.Errors.Add(new ErrorInfo(errorKey: "IsRequired", elementKey));
                return false;
            }
            if (password.Length < 6)
            {
                ValidationResult.Errors.Add(new ErrorInfo(errorKey: "TooShort", elementKey));
                return false;
            }
            if (password.Length > 30)
            {
                ValidationResult.Errors.Add(new ErrorInfo(errorKey: "TooLong", elementKey));
                return false;
            }
            return true;
        }

        private async Task<bool> ValidateRolesAsync(List<long>? roleIds, bool isUpdate)
        {
            string elementKey = "RoleId";
            if (roleIds == null && isUpdate)
            {
                return true;
            }
            if (roleIds == null || roleIds.Count == 0)
            {
                ValidationResult.Errors.Add(new ErrorInfo(errorKey: "IsRequired", elementKey));
                return false;
            }
            bool result = true;
            foreach (long roleId in roleIds)
            {
                if (await RoleReadRepository.RoleExistsAsync(roleId))
                {
                    continue;
                }
                ValidationResult.Errors.Add(
                    new ErrorInfo(
                        errorKey: "NotExist",
                        elementKey,
                        data: new Dictionary<string, string>() { { elementKey, roleId.ToString() } }
                    )
                );
                result = false;
            }
            return result;
        }
    }
}
