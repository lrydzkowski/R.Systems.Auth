using System;
using R.Systems.Auth.Core.Models.Users;
using R.Systems.Shared.Core.Interfaces;
using R.Systems.Shared.Core.Validation;

namespace R.Systems.Auth.Core.Validators;

public class AuthenticationValidator : IDependencyInjectionScoped
{
    public AuthenticationValidator(ValidationResult validationResult)
    {
        ValidationResult = validationResult;
    }

    public ValidationResult ValidationResult { get; }

    public bool IsBlocked(UserAuthentication user, UserSettings userSettings)
    {
        if (!ExceedMaxNumOfIncorrectSignIn(user, userSettings) || BlockDurationTimePassed(user, userSettings))
        {
            return false;
        }
        ValidationResult.Errors.Add(new ErrorInfo(
            errorKey: "Blocked",
            elementKey: "User"
        ));
        return true;
    }

    private bool ExceedMaxNumOfIncorrectSignIn(UserAuthentication user, UserSettings userSettings)
    {
        return user.NumOfIncorrectSignIn >= userSettings.MaxNumOfIncorrectLoginsBeforeBlock;
    }

    private bool BlockDurationTimePassed(UserAuthentication user, UserSettings userSettings)
    {
        if (user.LastIncorrectSignInDateTimeUtc == null)
        {
            return true;
        }
        TimeSpan timeFromLastIncorrectSignIn = DateTime.UtcNow.Subtract((DateTime)user.LastIncorrectSignInDateTimeUtc);
        TimeSpan blockDurationTime = TimeSpan.FromMinutes(userSettings.BlockDurationInMinutes);
        return timeFromLastIncorrectSignIn > blockDurationTime;
    }
}
