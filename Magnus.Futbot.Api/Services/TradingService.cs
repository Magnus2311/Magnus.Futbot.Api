using Magnus.Futbot.Api.Hubs;
using Magnus.Futbot.Api.Hubs.Interfaces;
using Magnus.Futbot.Api.Services.Interfaces;
using Magnus.Futbot.Common.Models.DTOs;
using Magnus.Futbot.Common.Models.DTOs.Trading;
using Magnus.Futbot.Selenium.Services.Players;
using Magnus.Futbot.Selenium.Services.Trade.Sell;
using Magnus.Futbot.Services.Trade.Buy;
using Microsoft.AspNetCore.SignalR;

namespace Magnus.Futbot.Api.Services
{
    public class TradingService : ITradingService
    {
        private readonly BidService _bidService;
        private readonly MovePlayersService _movePlayersService;
        private readonly ProfilesService _profilesService;
        private readonly SellService _sellService;
        private readonly IHubContext<ProfilesHub, IProfilesClient> _profilesHubContext;
        private readonly Action<ProfileDTO> _updateProfile;

        public TradingService(BidService bidService,
            MovePlayersService movePlayersService,
            ProfilesService profilesService,
            SellService sellService,
            IHubContext<ProfilesHub, IProfilesClient> profilesHubContext)
        {
            _bidService = bidService;
            _movePlayersService = movePlayersService;
            _profilesService = profilesService;
            _sellService = sellService;
            _profilesHubContext = profilesHubContext;
            _updateProfile = new Action<ProfileDTO>(
                (profileDTO) => _profilesHubContext.Clients.Users(profileDTO.UserId).OnProfileUpdated(profileDTO));
        }

        public async Task Buy(BuyCardDTO buyCardDTO)
        {
            var profileDTO = await _profilesService.GetByEmail(buyCardDTO.Email);

            profileDTO = _bidService.BidPlayer(profileDTO, buyCardDTO, _updateProfile);

            await _profilesService.UpdateProfile(profileDTO);
        }

        public async Task Sell(SellCardDTO sellCardDTO)
        {
            var profileDTO = await _profilesService.GetByEmail(sellCardDTO.Email);

            profileDTO = _sellService.SellPlayer(sellCardDTO, profileDTO, _updateProfile);

            await _profilesService.UpdateProfile(profileDTO);
        }

        public async Task MoveCardsFromTransferTargetsToTransferList(string email)
        {
            var profileDTO = await _profilesService.GetByEmail(email);

            profileDTO = _movePlayersService.SendTransferTargetsToTransferList(profileDTO, _updateProfile);

            await _profilesService.UpdateProfile(profileDTO);
        }
    }
}
