using R.Systems.Auth.Core.Interfaces;
using R.Systems.Auth.Core.Models;
using R.Systems.Shared.Core.Interfaces;
using R.Systems.Shared.Core.Validation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace R.Systems.Auth.Core.Validators;

public class UserWriteValidator : IDependencyInjectionScoped
{
    public UserWriteValidator(
        IUserReadRepository userReadRepository,
        IRoleReadRepository roleReadRepository,
        ValidationResult validationResult,
        IPasswordHasher passwordHasher)
    {
        UserReadRepository = userReadRepository;
        RoleReadRepository = roleReadRepository;
        ValidationResult = validationResult;
        PasswordHasher = passwordHasher;
    }

    public IUserReadRepository UserReadRepository { get; }
    public IRoleReadRepository RoleReadRepository { get; }
    public ValidationResult ValidationResult { get; }
    public IPasswordHasher PasswordHasher { get; }

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

    public async Task<bool> ValidateDeleteAsync(long userId, long authorizedUserId)
    {
        bool result = true;
        result &= await ValidateUserIdAsync(userId);
        result &= ValidateAuthorizedUserId(userId, authorizedUserId);
        return result;
    }

    public async Task<bool> ValidateChangePasswordAsync(
        long userId, string currentPassword, string newPassword, string repeatedNewPassword)
    {
        bool result = true;
        result &= await ValidateUserIdAsync(userId);
        result &= await ValidateUserPasswordAsync(userId, currentPassword);
        result &= ValidateNewPasswords(newPassword, repeatedNewPassword);
        result &= ValidatePassword(newPassword, isUpdate: false);
        return result;
    }

    private async Task<bool> ValidateUserIdAsync(long userId)
    {
        if (!(await UserReadRepository.UserExistsAsync(userId)))
        {
            ValidationResult.Errors.Add(new ErrorInfo(errorKey: "NotExist", elementKey: "UserId"));
            return false;
        }
        return true;
    }

    private bool ValidateAuthorizedUserId(long userId, long authorizedUserId)
    {
        if (userId == authorizedUserId)
        {
            ValidationResult.Errors.Add(new ErrorInfo(errorKey: "CannotDeleteYourself", elementKey: "UserId"));
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

    private async Task<bool> ValidateUserPasswordAsync(long userId, string password)
    {
        User? user = await UserReadRepository.GetUserForAuthenticationAsync(userId);
        if (user == null)
        {
            ValidationResult.Errors.Add(new ErrorInfo(errorKey: "NotExist", elementKey: "User"));
            return false;
        }
        if (user.PasswordHash == null)
        {
            return true;
        }
        if (!PasswordHasher.VerifyPasswordHash(password, user.PasswordHash))
        {
            ValidationResult.Errors.Add(new ErrorInfo(errorKey: "WrongPassword", elementKey: "User"));
            return false;
        }
        return true;
    }

    private bool ValidateNewPasswords(string newPassword, string repeatedNewPassword)
    {
        if (newPassword != repeatedNewPassword)
        {
            ValidationResult.Errors.Add(new ErrorInfo(errorKey: "DifferentValues", elementKey: "Passwords"));
            return false;
        }
        return true;
    }
}
