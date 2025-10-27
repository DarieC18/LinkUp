using AutoMapper;
using LinkUp.Application.Auth;
using LinkUp.Application.ViewModels.Account;

namespace LinkUp.Application.Mappings
{
    public class AuthProfile : Profile
    {
        public AuthProfile()
        {
            CreateMap<RegisterViewModel, RegisterDto>();
            CreateMap<LoginViewModel, LoginDto>();
        }
    }
}
