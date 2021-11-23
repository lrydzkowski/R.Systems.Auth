using R.Systems.Auth.Core.Interfaces;
using R.Systems.Auth.Core.Models;
using System;
using System.Threading.Tasks;

namespace R.Systems.Auth.Infrastructure.Db.Repositories
{
    public class UserWriteRepository : IUserWriteRepository
    {
        public Task<bool> DeleteAsync(User entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> EditAsync(User entity)
        {
            throw new NotImplementedException();
        }
    }
}
