using R.Systems.Auth.SharedKernel.Interfaces;
using System.Collections.Generic;

namespace R.Systems.Auth.Core.Models
{
    public class User : IEntity
    {
        public long RecId { get; set; }

        public long UserId
        {
            get
            {
                return RecId;
            }
        }

        public string Email { get; set; } = "";

        public string FirstName { get; set; } = "";

        public string LastName { get; set; } = "";

        public string? PasswordHash { get; set; } = null;

        public ICollection<Role> Roles { get; set; } = new List<Role>();
    }
}
