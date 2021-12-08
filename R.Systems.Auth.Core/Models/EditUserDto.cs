using System.Collections.Generic;

namespace R.Systems.Auth.Core.Models
{
    public class EditUserDto
    {
        public string? Email { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Password { get; set; }

        public List<long>? RoleIds { get; set; }
    }
}
