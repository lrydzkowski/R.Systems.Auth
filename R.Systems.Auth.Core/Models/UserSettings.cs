namespace R.Systems.Auth.Core.Models;

public class UserSettings
{
    public int MaxNumOfIncorrectSignInBeforeBlock { get; set; }

    public int BlockDurationInMinutes { get; set; }
}
