using AutoMapper;
using R.Systems.Auth.Core.Models;
using R.Systems.Shared.Core.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace R.Systems.Auth.Core.Services;

public class RoleService : IDependencyInjectionScoped
{
    public RoleService(IGenericReadRepository<Role> repository, IMapper mapper)
    {
        Repository = repository;
        Mapper = mapper;
    }

    public IGenericReadRepository<Role> Repository { get; }
    public IMapper Mapper { get; }

    public async Task<List<RoleDto>> GetAsync()
    {
        List<Role> roles = await Repository.GetAsync();
        List<RoleDto> rolesDto = Mapper.Map<List<RoleDto>>(roles);
        return rolesDto;
    }
}
