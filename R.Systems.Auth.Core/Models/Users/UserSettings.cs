namespace R.Systems.Auth.Core.Models.Users;

public class UserSettings
{
    public int MaxNumOfIncorrectLoginsBeforeBlock { get; set; }

    public double BlockDurationInMinutes { get; set; }
}
