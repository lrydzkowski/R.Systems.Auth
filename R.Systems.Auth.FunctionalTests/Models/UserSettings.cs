namespace R.Systems.Auth.FunctionalTests.Models;

internal static class UserSettings
{
    public const int MaxNumOfIncorrectLoginsBeforeBlock = 5;

    public const double BlockDurationInMinutes = 0.1d;
}
