using Magnus.Futbot.Api.Hubs;
using Magnus.Futbot.Api.Hubs.Interfaces;
using Magnus.Futbot.Api.Services.Interfaces;
using Magnus.Futbot.Common.Models.DTOs.Trading;
using Magnus.Futbot.Services.Trade.Buy;
using Microsoft.AspNetCore.SignalR;

namespace Magnus.Futbot.Api.Services
{
    public class TradingService : ITradingService
    {
        private readonly BidService _bidService;
        private readonly ProfilesService _profilesService;
        private readonly IHubContext<ProfilesHub, IProfilesClient> _profilesHubContext;

        public TradingService(BidService bidService,
            ProfilesService profilesService,
            IHubContext<ProfilesHub, IProfilesClient> profilesHubContext)
        {
            _bidService = bidService;
            _profilesService = profilesService;
            _profilesHubContext = profilesHubContext;
        }

        public async Task Buy(BuyCardDTO buyCardDTO)
        {
            var profile = await _profilesService.GetByEmail(buyCardDTO.Email);

            _bidService.BidPlayer(profile, buyCardDTO, (profileDTO) =>
            {
                _profilesHubContext.Clients.Users(profile.UserId).OnProfileUpdated(profileDTO);
            });
        }
    }
}
