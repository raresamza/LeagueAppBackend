using ApplicationLayer.DTOs;
using AutoMapper;
using DomainLayer.Models;

namespace WebApi.Profiles.UserProfile;

public class UserProfile : Profile
{
    public UserProfile() 
    {
        CreateMap<AppUser, AppUserDTO>()
            .ForMember(dest => dest.PhoneNumber, src => src.MapFrom(x => x.PhoneNumber))
            .ForMember(dest => dest.Email, src => src.MapFrom(x => x.Email))
            .ForMember(dest => dest.Name, src => src.MapFrom(x => x.Name));
    }
}
