using AutoMapper;
using Magnus.Futbot.Api.Models.Requests;
using Magnus.Futbot.Api.Models.Responses;
using Magnus.Futbot.Common.Models.DTOs;
using Magnus.Futbot.Common.Models.DTOs.Trading;
using Magnus.Futbot.Common.Models.DTOs.Trading.Actions;
using Magnus.Futbot.Common.Models.Selenium.Actions;
using Magnus.Futbot.Common.Models.Selenium.Profiles;
using Magnus.Futbot.Database.Models;
using Magnus.Futbot.Database.Models.Actions;
using Magnus.Futbot.Initializer.Models.Players;
using MongoDB.Bson;
using Magnus.Futbot.Common.Models.Database.Card;

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

            CreateMap<Card, PlayerDTO>()
                .ForMember(dest => dest.Name, options => options.MapFrom(src => src.CommonName == null || src.CommonName == string.Empty ? src.Name : src.CommonName))
                .ForMember(dest => dest.Rating, options => options.MapFrom(src => src.OverallRating))
                .ForMember(dest => dest.Id, options => options.MapFrom(src => src.EAId));

            CreateMap<Player, PlayerDTO>()
                .ForMember(dest => dest.Name, options => options.MapFrom(src => src.c == null ? $"{src.f} {src.l}" : src.c))
                .ForMember(dest => dest.Rating, options => options.MapFrom(src => src.r))
                .ForMember(dest => dest.Id, options => options.MapFrom(src => src.id));

            CreateMap<LegendsPlayer, PlayerDTO>()
                .ForMember(dest => dest.Name, options => options.MapFrom(src => src.c == null ? $"{src.f} {src.l}" : src.c))
                .ForMember(dest => dest.Rating, options => options.MapFrom(src => src.r))
                .ForMember(dest => dest.Id, options => options.MapFrom(src => src.id));

            CreateMap<EaPlayerItem, PlayerDTO>()
                .ForMember(dest => dest.Name, options => options.MapFrom(src => string.IsNullOrWhiteSpace(src.CommonName)
                    ? $"{src.FirstName} {src.LastName}".Trim()
                    : src.CommonName!))
                .ForMember(dest => dest.Rating, options => options.MapFrom(src => src.OverallRating))
                .ForMember(dest => dest.Id, options => options.MapFrom(src => src.Id));

            CreateMap<BuyAndSellCardDTO, BuyCardDTO>().ReverseMap();
            CreateMap<BuyAndSellCardDTO, SellCardDTO>().ReverseMap();

            CreateMap<TradeActionDTO, TradeAction>().ReverseMap();

            CreateMap<BuyAction, BuyActionEntity>()
                .ForMember(dest => dest.Id, options => options.MapFrom(src => string.IsNullOrEmpty(src.Id) ? new ObjectId() : new ObjectId(src.Id)))
                .ForMember(dest => dest.ProfileId, options => options.MapFrom(src => string.IsNullOrEmpty(src.ProfileId) ? new ObjectId() : new ObjectId(src.ProfileId)))
                .ReverseMap()
                .ForMember(dest => dest.Id, options => options.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.ProfileId, options => options.MapFrom(src => src.ProfileId.ToString()));
            CreateMap<SellAction, SellActionEntity>()
                .ForMember(dest => dest.Id, options => options.MapFrom(src => string.IsNullOrEmpty(src.Id) ? new ObjectId() : new ObjectId(src.Id)))
                .ForMember(dest => dest.ProfileId, options => options.MapFrom(src => string.IsNullOrEmpty(src.ProfileId) ? new ObjectId() : new ObjectId(src.ProfileId)))
                .ReverseMap()
                .ForMember(dest => dest.Id, options => options.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.ProfileId, options => options.MapFrom(src => src.ProfileId.ToString()));
            CreateMap<MoveAction, MoveActionEntity>()
                .ForMember(dest => dest.Id, options => options.MapFrom(src => string.IsNullOrEmpty(src.Id) ? new ObjectId() : new ObjectId(src.Id)))
                .ForMember(dest => dest.ProfileId, options => options.MapFrom(src => string.IsNullOrEmpty(src.ProfileId) ? new ObjectId() : new ObjectId(src.ProfileId)))
                .ReverseMap()
                .ForMember(dest => dest.Id, options => options.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.ProfileId, options => options.MapFrom(src => src.ProfileId.ToString()));
            CreateMap<PauseAction, PauseActionEntity>()
                .ForMember(dest => dest.Id, options => options.MapFrom(src => string.IsNullOrEmpty(src.Id) ? new ObjectId() : new ObjectId(src.Id)))
                .ForMember(dest => dest.ProfileId, options => options.MapFrom(src => string.IsNullOrEmpty(src.ProfileId) ? new ObjectId() : new ObjectId(src.ProfileId)))
                .ReverseMap()
                .ForMember(dest => dest.Id, options => options.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.ProfileId, options => options.MapFrom(src => src.ProfileId.ToString()));

            CreateMap<TradeActionsDTO, TradeActions>().ReverseMap();
            CreateMap<BuyActionDTO, BuyAction>().ReverseMap();
            CreateMap<SellActionDTO, SellAction>().ReverseMap();
            CreateMap<MoveActionDTO, MoveAction>().ReverseMap();
            CreateMap<PauseActionDTO, PauseAction>().ReverseMap();
            CreateMap<Trade, TradeDTO>().ReverseMap();

            CreateMap<AddPlayerPricesRequest, PlayerPrice>()
                .ForMember(dest => dest.Prices, options => options.MapFrom(src => src.Prices.Select(p => new PriceEntry { Prize = p })));

            CreateMap<PlayerPrice, GetPriceResponse>()
                .ForMember(dest => dest.Prices, options => options.MapFrom(src => src.Prices.Select(p => p.Prize)))
                .ForMember(dest => dest.LastUpdated, options => options.MapFrom(src => src.Prices.OrderByDescending(p => p.CreatedDate).FirstOrDefault().CreatedDate));

            CreateMap<AddBuyTradeRequest, BuyCardDTO>();

            // Successful Purchase mappings
            CreateMap<AddSuccessfulPurchaseRequest, SuccessfulPurchase>();
            CreateMap<SuccessfulPurchase, SuccessfulPurchaseResponse>();

            // Filters mappings
            CreateMap<FiltersDto, Filters>();
            CreateMap<Filters, FiltersDto>();
        }
    }
}
