using R.Systems.Auth.SharedKernel.Interfaces;
using System.Collections.Generic;

namespace R.Systems.Auth.Core.Models
{
    public class Role : IEntity
    {
        public long RecId { get; set; }

        public long RoleId
        {
            get
            {
                return RecId;
            }
        }

        public string RoleKey { get; set; } = "";

        public string Name { get; set; } = "";

        public string Description { get; set; } = "";

        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
