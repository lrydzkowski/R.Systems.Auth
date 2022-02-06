using AutoMapper;
using R.Systems.Auth.Core.Models.Users;
using R.Systems.Shared.Core.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace R.Systems.Auth.Core.Services;

public class UserReadService : IDependencyInjectionScoped
{
    public UserReadService(IGenericReadRepository<UserEntity> repository, IMapper mapper)
    {
        Repository = repository;
        Mapper = mapper;
    }

    public IGenericReadRepository<UserEntity> Repository { get; }
    public IMapper Mapper { get; }

    public async Task<UserDto?> GetAsync(long userId)
    {
        UserEntity? user = await Repository.GetAsync(userId);
        UserDto userDto = Mapper.Map<UserDto>(user);
        return userDto;
    }

    public async Task<List<UserDto>> GetAsync()
    {
        List<UserEntity> users = await Repository.GetAsync();
        List<UserDto> usersDto = Mapper.Map<List<UserDto>>(users);
        return usersDto;
    }
}
