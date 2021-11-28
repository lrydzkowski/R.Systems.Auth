﻿using AutoMapper;
using R.Systems.Auth.Core.Models;
using R.Systems.Auth.SharedKernel.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace R.Systems.Auth.Core.Services
{
    public class UserService : IDependencyInjectionScoped
    {
        public UserService(IGenericReadRepository<User> repository, IMapper mapper)
        {
            Repository = repository;
            Mapper = mapper;
        }

        public IGenericReadRepository<User> Repository { get; }
        public IMapper Mapper { get; }

        public async Task<UserDto?> GetAsync(long userId)
        {
            User? user = await Repository.GetAsync(userId);
            UserDto userDto = Mapper.Map<UserDto>(user);
            return userDto;
        }

        public async Task<List<UserDto>> GetAsync()
        {
            List<User> users = await Repository.GetAsync();
            List<UserDto> usersDto = Mapper.Map<List<UserDto>>(users);
            return usersDto;
        }
    }
}
