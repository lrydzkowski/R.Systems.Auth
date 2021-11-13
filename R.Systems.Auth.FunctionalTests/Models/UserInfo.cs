using R.Systems.Auth.Common.Models;

namespace R.Systems.Auth.FunctionalTests.Models
{
    public class UserInfo : User
    {
        public string Password { get; init; } = "";
    }
}
