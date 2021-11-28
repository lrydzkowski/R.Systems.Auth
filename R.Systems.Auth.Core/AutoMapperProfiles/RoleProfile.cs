using AutoMapper;
using R.Systems.Auth.Core.Models;

namespace R.Systems.Auth.Core.AutoMapperProfiles
{
    internal class RoleProfile : Profile
    {
        public RoleProfile()
        {
            CreateMap<Role, RoleDto>();
        }
    }
}
