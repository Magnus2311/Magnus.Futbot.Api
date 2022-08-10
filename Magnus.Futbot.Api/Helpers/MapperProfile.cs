using AutoMapper;
using Magnus.Futbot.Common.Models.DTOs;
using Magnus.Futbot.Common.Models.Selenium.Profiles;
using Magnus.Futbot.Database.Models;
using Magnus.Futbot.Initializer.Models.Players;

namespace Magnus.Futbot.Api.Helpers
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<ProfileDTO, ProfileDocument>().ReverseMap();
            CreateMap<AddProfileDTO, ProfileDocument>().ReverseMap();
            CreateMap<ProfileDTO, AddProfileDTO>().ReverseMap();

            CreateMap<PlayerDTO, PlayerDocument>();

            CreateMap<Player, PlayerDTO>()
                .ForMember(dest => dest.Name, options => options.MapFrom(src => src.c == null ? $"{src.f} {src.l}" : src.c))
                .ForMember(dest => dest.Rating, options => options.MapFrom(src => src.r))
                .ForMember(dest => dest.Id, options => options.MapFrom(src => src.id));

            CreateMap<LegendsPlayer, PlayerDTO>()
                .ForMember(dest => dest.Name, options => options.MapFrom(src => src.c == null ? $"{src.f} {src.l}" : src.c))
                .ForMember(dest => dest.Rating, options => options.MapFrom(src => src.r))
                .ForMember(dest => dest.Id, options => options.MapFrom(src => src.id));
        }
    }
}
