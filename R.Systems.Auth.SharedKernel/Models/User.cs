using System.Collections.Generic;

namespace R.Systems.Auth.SharedKernel.Models
{
    public class User
    {
        public long UserId { get; set; }

        public string Email { get; set; } = "";

        public string FirstName { get; set; } = "";

        public string LastName { get; set; } = "";

        public string? PasswordHash { get; set; } = null;

        public ICollection<Role> Roles { get; set; } = new List<Role>();
    }
}
