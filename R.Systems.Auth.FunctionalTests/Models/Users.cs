using R.Systems.Auth.Core.Interfaces;
using System.Collections;
using System.Collections.Generic;

namespace R.Systems.Auth.FunctionalTests.Models
{
    public class Users : IEnumerable<UserInfo>
    {
        private readonly List<UserInfo> _users = new()
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
            _users.ForEach(userInfo => userInfo.PasswordHash = passwordHasher.CreatePasswordHash(userInfo.Password));
        }

        public UserInfo this[int index]
        {
            get { return _users[index]; }
            set { _users[index] = value; }
        }

        public IEnumerator<UserInfo> GetEnumerator()
        {
            return _users.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _users.GetEnumerator();
        }
    }
}
