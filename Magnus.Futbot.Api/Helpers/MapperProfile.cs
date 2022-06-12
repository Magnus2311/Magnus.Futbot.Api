using AutoMapper;
using Magnus.Futbot.Api.Models.DTOs;
using Magnus.Futbot.Database.Models;

namespace Magnus.Futbot.Api.Helpers
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<ProfileDTO, ProfileDocument>();
            CreateMap<ProfileDocument, ProfileDTO>();
        }
    }
}
