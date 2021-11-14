using R.Systems.Auth.Core.Models;
using R.Systems.Auth.SharedKernel.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace R.Systems.Auth.WebApi.Services
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
