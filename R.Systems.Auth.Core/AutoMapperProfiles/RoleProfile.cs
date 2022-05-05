using AutoMapper;
using R.Systems.Auth.Core.Models.Roles;

namespace R.Systems.Auth.Core.AutoMapperProfiles;

internal class RoleProfile : Profile
{
    public RoleProfile()
    {
        CreateMap<RoleEntity, RoleDto>().ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.Id));
    }
}
