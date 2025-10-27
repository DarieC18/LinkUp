using AutoMapper;
using LinkUp.Application.DTOs;
using LinkUp.Application.ViewModels.Profile;

namespace LinkUp.Application.Mappings
{
    public class ProfileProfile : Profile
    {
        public ProfileProfile()
        {
            CreateMap<ProfileDto, ProfileViewModel>();
            CreateMap<EditProfileViewModel, EditProfileDto>();
        }
    }
}
