using System.Collections.Generic;

namespace R.Systems.Auth.Common.Models
{
    public class Role
    {
        public long RoleId { get; set; }

        public string RoleKey { get; set; } = "";

        public string Name { get; set; } = "";

        public string Description { get; set; } = "";

        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
