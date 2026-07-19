using AutoMapper;
using Ecommerce.Data.Entities;
using Ecommerce.DTOs.Auth;

namespace Ecommerce.Common.Mappers
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
