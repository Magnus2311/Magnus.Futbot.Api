using AutoMapper;
using Magnus.Futbot.Api.Hubs;
using Magnus.Futbot.Api.Hubs.Interfaces;
using Magnus.Futbot.Api.Services.Interfaces;
using Magnus.Futbot.Common.Interfaces.Notifiers;
using Magnus.Futbot.Common.Models.DTOs;
using Magnus.Futbot.Common.Models.DTOs.Trading;
using Magnus.Futbot.Common.Models.Selenium.Actions;
using Magnus.Futbot.Database.Models.Actions;
using Magnus.Futbot.Database.Repositories.Actions;
using Magnus.Futtbot.Connections.Connection;
using Magnus.Futtbot.Connections.Enums;
using Magnus.Futtbot.Connections.Services;
using Microsoft.AspNetCore.SignalR;

namespace Magnus.Futbot.Api.Services
{
    public class TradingService : ITradingService
    {
        private readonly ProfilesService _profilesService;
        private readonly BuyActionRepository _buyActionRepository;
        private readonly SellActionRepository _sellActionRepository;
        private readonly MoveActionRepository _moveActionRepository;
        private readonly IHubContext<ProfilesHub, IProfilesClient> _profilesHubContext;
        private readonly IActionsNotifier _actionsNotifier;
        private readonly IMapper _mapper;
        private readonly BuyService _buyService;
        private readonly SellService _sellService;
        private readonly UserActionsService _userActionsService;
        private readonly TradeHistoryService _tradeHistoryService;
        private readonly GetUserPileConnection _getUserPileConnection;
        private readonly Action<ProfileDTO> _updateProfile;

        public TradingService(ProfilesService profilesService, BuyActionRepository buyActionRepository,
            SellActionRepository sellActionRepository, MoveActionRepository moveActionRepository,
            IHubContext<ProfilesHub, IProfilesClient> profilesHubContext, IActionsNotifier actionsNotifier,
            IMapper mapper, BuyService buyService, SellService sellService, UserActionsService userActionsService,
            TradeHistoryService tradeHistoryService, GetUserPileConnection getUserPileConnection)
        {
            _profilesService = profilesService;
            _buyActionRepository = buyActionRepository;
            _sellActionRepository = sellActionRepository;
            _moveActionRepository = moveActionRepository;
            _profilesHubContext = profilesHubContext;
            _actionsNotifier = actionsNotifier;
            _mapper = mapper;
            _buyService = buyService;
            _sellService = sellService;
            _userActionsService = userActionsService;
            _tradeHistoryService = tradeHistoryService;
            _getUserPileConnection = getUserPileConnection;
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

            var buyAction = new Func<Task>(async () =>
            {
                await _buyService.Buy(profileDTO, buyCardDTO, tknSrc, null, _updateProfile);
            });

            var tradeAction = new BuyAction(profileDTO.Id, buyAction, tknSrc, buyCardDTO);

            _userActionsService.AddAction(profileDTO.Email, tradeAction);

            await _buyActionRepository.Add(_mapper.Map<BuyActionEntity>(tradeAction));
            await _actionsNotifier.AddAction(profileDTO, tradeAction);
            await _tradeHistoryService.AddTradeAsync(profileDTO, buyCardDTO);
        }

        public async Task BuyAndSell(BuyAndSellCardDTO buyAndSellCardDTO)
        {
            var buyCardDTO = _mapper.Map<BuyCardDTO>(buyAndSellCardDTO);
            var sellCardDTO = _mapper.Map<SellCardDTO>(buyAndSellCardDTO);

            var profileDTO = await _profilesService.GetByEmail(buyCardDTO.Email);

            var tknSrc = new CancellationTokenSource();

            var sellAction = new Func<long, Task>(async (tradeId) =>
            {
                await _sellService.SellCardById(profileDTO, sellCardDTO, tradeId);
            });

            var buyAction = new Func<Task>(async () =>
            {
                await _buyService.Buy(profileDTO, buyCardDTO, tknSrc, sellAction, _updateProfile);
            });

            var tradeAction = new BuyAction(profileDTO.Id, buyAction, tknSrc, buyCardDTO);
            _userActionsService.AddAction(profileDTO.Email, tradeAction);

            await _buyActionRepository.Add(_mapper.Map<BuyActionEntity>(tradeAction));
            await _actionsNotifier.AddAction(profileDTO, tradeAction);
            await _tradeHistoryService.AddTradeAsync(profileDTO, buyAndSellCardDTO);
        }

        public async Task Sell(SellCardDTO sellCardDTO)
        {
            var profileDTO = await _profilesService.GetByEmail(sellCardDTO.Email);
            var tknSrc = new CancellationTokenSource();

            var sellAction = new Func<Task>(async () =>
            {
                await _sellService.SellCard(profileDTO, sellCardDTO, tknSrc, _updateProfile);
            });

            var tradeAction = new SellAction(profileDTO.Id, sellAction, tknSrc, sellCardDTO);
            _userActionsService.AddAction(profileDTO.Email, tradeAction);

            await _sellActionRepository.Add(_mapper.Map<SellActionEntity>(tradeAction));
            await _actionsNotifier.AddAction(profileDTO, tradeAction);
            await _tradeHistoryService.AddTradeAsync(profileDTO, sellCardDTO);
        }

        public async Task MoveCardsFromTransferTargetsToTransferList(string email)
        {
            var profileDTO = await _profilesService.GetByEmail(email);
            //TO DO
        }

        public async Task SendUnassignedItemsToTransferList(string email)
        {
            var profileDTO = await _profilesService.GetByEmail(email);
            //TO DO
        }

        public async Task RelistPlayers()
        {
            var profiles = await _profilesService.GetRelistProfiles();
            foreach (var profileDTO in profiles)
            {
                var tknSrc = new CancellationTokenSource();

                var sellAction = new Func<Task>(async () =>
                {
                    await _sellService.RelistAll(profileDTO, tknSrc);
                });

                var action = new MoveAction(profileDTO.Id, sellAction, tknSrc, "Relisting all");

                _userActionsService.AddAction(profileDTO.Email, action);

                await _moveActionRepository.Add(_mapper.Map<MoveActionEntity>(action));
                await _actionsNotifier.AddAction(profileDTO, action);
                Thread.Sleep(2000);
            }
        }

        public async Task RelistPlayersByProfile(string email)
        {
            var profileDTO = await _profilesService.GetByEmail(email);
            var tknSrc = new CancellationTokenSource();
            await _sellService.RelistAll(profileDTO, tknSrc);
            await _profilesService.RefreshProfile(profileDTO);
        }

        public async Task ClearSoldCards(string email)
        {
            var profileDTO = await _profilesService.GetByEmail(email);
            var tknSrc = new CancellationTokenSource();

            profileDTO = await _profilesService.RefreshProfile(profileDTO);

            var clearSoldCardsResponse = await _sellService.ClearSoldCards(profileDTO, tknSrc);
            if (clearSoldCardsResponse != ConnectionResponseType.Success)
                return;

            var tradePileResponse = await _getUserPileConnection.GetUserTradePile(email);
            if (tradePileResponse == null 
                || tradePileResponse.Data == null 
                || tradePileResponse.ConnectionResponseType != ConnectionResponseType.Success) return;

            var soldCards = tradePileResponse.Data.auctionInfo
                .Where(ai => ai.tradeState == "closed");

            profileDTO.History.AddRange(soldCards);

            await _profilesService.RefreshProfile(profileDTO);
        }
    }
}
