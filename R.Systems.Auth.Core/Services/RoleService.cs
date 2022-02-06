using AutoMapper;
using R.Systems.Auth.Core.Models.Roles;
using R.Systems.Shared.Core.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace R.Systems.Auth.Core.Services;

public class RoleService : IDependencyInjectionScoped
{
    public RoleService(IGenericReadRepository<RoleEntity> repository, IMapper mapper)
    {
        Repository = repository;
        Mapper = mapper;
    }

    public IGenericReadRepository<RoleEntity> Repository { get; }
    public IMapper Mapper { get; }

    public async Task<List<RoleDto>> GetAsync()
    {
        List<RoleEntity> roles = await Repository.GetAsync();
        List<RoleDto> rolesDto = Mapper.Map<List<RoleDto>>(roles);
        return rolesDto;
    }
}
