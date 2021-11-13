using R.Systems.Auth.Common.Interfaces;
using System.Collections.Generic;

namespace R.Systems.Auth.FunctionalTests.Models
{
    public class Users
    {
        private readonly List<UserInfo> _data = new()
        {
            new UserInfo
            {
                Email = "test@lukaszrydzkowski.pl",
                FirstName = "Testowy",
                LastName = "Tester",
                Password = "123123"
            },
            new UserInfo
            {
                Email = "test2@lukaszrydzkowski.pl",
                FirstName = "Testowy 2",
                LastName = "Tester 2"
            }
        };

        public Users() { }

        public Users(IPasswordHasher passwordHasher)
        {
            _data.ForEach(userInfo => userInfo.PasswordHash = passwordHasher.CreatePasswordHash(userInfo.Password));
        }

        public List<UserInfo> Data
        {
            get
            {
                return _data;
            }
        }
    }
}
