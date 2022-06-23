using AutoMapper;
using Magnus.Futbot.Api.Models.DTOs;
using Magnus.Futbot.Api.Models.EA;
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

            CreateMap<Player, PlayerDocument>()
                .ForMember(dest => dest.Name, options => options.MapFrom(src => src.c == null ? $"{src.f} {src.l}" : src.c))
                .ForMember(dest => dest.Rating, options => options.MapFrom(src => src.r))
                .ForMember(dest => dest.Id, options => options.MapFrom(src => src.id));

            CreateMap<LegendsPlayer, PlayerDocument>()
                .ForMember(dest => dest.Name, options => options.MapFrom(src => src.c == null ? $"{src.f} {src.l}" : src.c))
                .ForMember(dest => dest.Rating, options => options.MapFrom(src => src.r))
                .ForMember(dest => dest.Id, options => options.MapFrom(src => src.id));
        }
    }
}
