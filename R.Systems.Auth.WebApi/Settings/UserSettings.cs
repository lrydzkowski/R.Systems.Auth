namespace R.Systems.Auth.WebApi.Settings;

public class UserSettings
{
    public const string PropertyName = "User";

    public int MaxNumOfIncorrectSignInBeforeBlock { get; set; }

    public int BlockDurationInMinutes { get; set; }
}
