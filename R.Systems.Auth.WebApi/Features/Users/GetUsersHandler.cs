using R.Systems.Auth.Core.Models;
using R.Systems.Auth.SharedKernel.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace R.Systems.Auth.WebApi.Features.Users
{
    public class GetUsersHandler : IDependencyInjectionScoped
    {
        public GetUsersHandler(IGenericReadRepository<User> repository)
        {
            Repository = repository;
        }

        public IGenericReadRepository<User> Repository { get; }

        public async Task<User?> HandleAsync(long userId)
        {
            User? user = await Repository.GetAsync(userId);
            return user;
        }

        public async Task<List<User>> HandleAsync()
        {
            List<User> users = await Repository.GetAsync();
            return users;
        }
    }
}
