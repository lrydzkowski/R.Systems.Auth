namespace R.Systems.Auth.WebApi.Settings;

public class UserSettings
{
    public const string PropertyName = "User";

    public int MaxNumOfIncorrectLoginsBeforeBlock { get; set; }

    public double BlockDurationInMinutes { get; set; }
}
