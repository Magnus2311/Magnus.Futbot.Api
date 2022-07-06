using AutoMapper;
using Magnus.Futbot.Common.Models.DTOs;
using Magnus.Futbot.Common.Models.Selenium.Profiles;
using Magnus.Futbot.Database.Models;

namespace Magnus.Futbot.Api.Helpers
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<ProfileDTO, ProfileDocument>();
            CreateMap<ProfileDocument, ProfileDTO>();

            CreateMap<AddProfileDTO, ProfileDocument>();
            CreateMap<ProfileDocument, AddProfileDTO>();

            CreateMap<ProfileDTO, AddProfileDTO>();
            CreateMap<AddProfileDTO, ProfileDTO>();

            CreateMap<PlayerDTO, PlayerDocument>();
        }
    }
}
