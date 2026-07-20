using AutoMapper;
using Domain.Entities;
using Application.DTOs.Auth;

namespace Application.Common.Mappers
{
    public class AuthMapping : Profile
    {
        public AuthMapping() 
        {
            CreateMap<RegisterRequest, AppUser>()
                .ForMember(x => x.UserName,opt => opt.MapFrom(x => x.Email));
        }
    }
}
