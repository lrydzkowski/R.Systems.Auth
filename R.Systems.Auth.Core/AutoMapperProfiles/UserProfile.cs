using AutoMapper;
using R.Systems.Auth.Core.Models.Users;

namespace R.Systems.Auth.Core.AutoMapperProfiles;

internal class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<UserEntity, UserDto>().ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id));
    }
}
