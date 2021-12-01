using R.Systems.Auth.Core.Models;
using System.Collections;
using System.Collections.Generic;

namespace R.Systems.Auth.FunctionalTests.Models
{
    public class Roles : IEnumerable<Role>
    {
        private readonly List<Role> _roles = new()
        {
            new Role
            {
                RoleKey = "admin",
                Name = "Administrator",
                Description = "System administrator"
            }
        };

        public Roles() { }

        public Role this[int index]
        {
            get { return _roles[index]; }
            set { _roles[index] = value; }
        }

        public IEnumerator<Role> GetEnumerator()
        {
            return _roles.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _roles.GetEnumerator();
        }
    }
}
