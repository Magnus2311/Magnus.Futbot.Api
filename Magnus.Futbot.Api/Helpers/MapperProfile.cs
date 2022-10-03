using AutoMapper;
using Magnus.Futbot.Common.Models.DTOs;
using Magnus.Futbot.Common.Models.DTOs.Trading.Actions;
using Magnus.Futbot.Common.Models.Selenium.Actions;
using Magnus.Futbot.Common.Models.Selenium.Profiles;
using Magnus.Futbot.Database.Models;
using Magnus.Futbot.Initializer.Models.Players;
using MongoDB.Bson;

namespace Magnus.Futbot.Api.Helpers
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<ProfileDTO, ProfileDocument>()
                .ForMember(dest => dest.Id, options => options.MapFrom(src => string.IsNullOrEmpty(src.Id) ? new ObjectId() : new ObjectId(src.Id)));
            CreateMap<ProfileDocument, ProfileDTO>()
                .ForMember(dest => dest.Id, options => options.MapFrom(src => src.Id.ToString()));
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

            CreateMap<TradeActionDTO, TradeAction>().ReverseMap();
        }
    }
}
