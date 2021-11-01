using R.Systems.Auth.Common.Models;

namespace R.Systems.Auth.IntegrationTests.Models
{
    public class UserInfo : User
    {
        public string Password { get; init; } = "";
    }
}
