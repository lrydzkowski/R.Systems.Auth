using AutoMapper;
using R.Systems.Auth.Core.Models;

namespace R.Systems.Auth.Core.AutoMapperProfiles
{
    internal class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserDto>().ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id));
        }
    }
}
