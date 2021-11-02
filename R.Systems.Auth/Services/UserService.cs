using R.Systems.Auth.Common.Models;
using R.Systems.Auth.Common.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace R.Systems.Auth.Services
{
    public class UserService
    {
        public UserService(IRepository<User> repository)
        {
            Repository = repository;
        }

        public IRepository<User> Repository { get; }

        public async Task<User?> GetUserAsync(long userId)
        {
            User? user = await Repository.GetAsync(userId);
            return user;
        }

        public async Task<List<User>> GetUsersAsync()
        {
            List<User> users = await Repository.GetAsync();
            return users;
        }
    }
}
