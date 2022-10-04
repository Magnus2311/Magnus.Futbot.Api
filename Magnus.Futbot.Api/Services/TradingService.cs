using Magnus.Futbot.Api.Hubs;
using Magnus.Futbot.Api.Hubs.Interfaces;
using Magnus.Futbot.Api.Services.Interfaces;
using Magnus.Futbot.Common.Interfaces.Notifiers;
using Magnus.Futbot.Common.Models.DTOs;
using Magnus.Futbot.Common.Models.DTOs.Trading;
using Magnus.Futbot.Common.Models.Selenium.Actions;
using Magnus.Futbot.Selenium.Services.Players;
using Magnus.Futbot.Selenium.Services.Trade.Buy;
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
        private readonly BinService _binService;
        private readonly IHubContext<ProfilesHub, IProfilesClient> _profilesHubContext;
        private readonly IActionsNotifier _actionsNotifier;
        private readonly Action<ProfileDTO> _updateProfile;

        public TradingService(BidService bidService,
            MovePlayersService movePlayersService,
            ProfilesService profilesService,
            SellService sellService,
            BinService binService,
            IHubContext<ProfilesHub, IProfilesClient> profilesHubContext,
            IActionsNotifier actionsNotifier)
        {
            _bidService = bidService;
            _movePlayersService = movePlayersService;
            _profilesService = profilesService;
            _sellService = sellService;
            _binService = binService;
            _profilesHubContext = profilesHubContext;
            _actionsNotifier = actionsNotifier;
            _updateProfile = new Action<ProfileDTO>(
                async (profileDTO) => 
                { 
                    await _profilesHubContext.Clients.Users(profileDTO.UserId).OnProfileUpdated(profileDTO);
                    await _profilesService.UpdateProfile(profileDTO);
                });
        }

        public async Task Buy(BuyCardDTO buyCardDTO)
        {
            var profileDTO = await _profilesService.GetByEmail(buyCardDTO.Email);

            var tknSrc = new CancellationTokenSource();

            TradeAction tradeAction;
            if (buyCardDTO.IsBin) tradeAction = _binService.BinPlayer(profileDTO, buyCardDTO, _updateProfile, tknSrc);
            else tradeAction = _bidService.BidPlayer(profileDTO, buyCardDTO, _updateProfile, tknSrc);

            await _actionsNotifier.AddAction(profileDTO, tradeAction);
        }

        public async Task BuyAndSell(BuyCardDTO buyCardDTO, SellCardDTO sellCardDTO)
        {
            var profileDTO = await _profilesService.GetByEmail(buyCardDTO.Email);

            var tknSrc = new CancellationTokenSource();

            var sellAction = new Action(async () =>
            {
                await _sellService.SellCurrentPlayer(sellCardDTO, profileDTO, tknSrc);
            });

            TradeAction tradeAction;
            if (buyCardDTO.IsBin) tradeAction = _binService.BinPlayer(profileDTO, buyCardDTO, _updateProfile, tknSrc, sellAction);
            else tradeAction = _bidService.BidPlayer(profileDTO, buyCardDTO, _updateProfile, tknSrc);

            await _actionsNotifier.AddAction(profileDTO, tradeAction);
        }

        public async Task Sell(SellCardDTO sellCardDTO)
        {
            var tknSrc = new CancellationTokenSource();
            var profileDTO = await _profilesService.GetByEmail(sellCardDTO.Email);

            var action = _sellService.SellPlayer(sellCardDTO, profileDTO, _updateProfile, tknSrc);
            await _actionsNotifier.AddAction(profileDTO, action);

            await _profilesService.UpdateProfile(profileDTO);
        }

        public async Task MoveCardsFromTransferTargetsToTransferList(string email)
        {
            var profileDTO = await _profilesService.GetByEmail(email);

            profileDTO = _movePlayersService.SendTransferTargetsToTransferList(profileDTO, _updateProfile);

            await _profilesService.UpdateProfile(profileDTO);
        }

        public async Task SendUnassignedItemsToTransferList(string email)
        {
            var profileDTO = await _profilesService.GetByEmail(email);

            profileDTO = _movePlayersService.SendUnassignedItemsToTransferList(profileDTO, _updateProfile);

            await _profilesService.UpdateProfile(profileDTO);
        }

        public async Task RelistPlayers()
        {
            var profiles = await _profilesService.GetRelistProfiles();
            Parallel.ForEach(profiles, (profile) =>
            {
                var tknSrc = new CancellationTokenSource();
                var action = _sellService.RelistPlayers(profile, tknSrc);
                _actionsNotifier.AddAction(profile, action);
            });
        }
    }
}
