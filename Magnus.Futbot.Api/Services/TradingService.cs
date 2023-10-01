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
using Magnus.Futbot.Selenium.Services.Players;
using Magnus.Futtbot.Connections.Services;
using Microsoft.AspNetCore.SignalR;

namespace Magnus.Futbot.Api.Services
{
    public class TradingService : ITradingService
    {
        private readonly MovePlayersService _movePlayersService;
        private readonly ProfilesService _profilesService;
        private readonly BuyActionRepository _buyActionRepository;
        private readonly SellActionRepository _sellActionRepository;
        private readonly MoveActionRepository _moveActionRepository;
        private readonly IHubContext<ProfilesHub, IProfilesClient> _profilesHubContext;
        private readonly IActionsNotifier _actionsNotifier;
        private readonly IMapper _mapper;
        private readonly BuyService _buyService;
        private readonly SellService _sellService;
        private readonly Action<ProfileDTO> _updateProfile;

        public TradingService(MovePlayersService movePlayersService,
            ProfilesService profilesService,
            BuyActionRepository buyActionRepository,
            SellActionRepository sellActionRepository,
            MoveActionRepository moveActionRepository,
            IHubContext<ProfilesHub, IProfilesClient> profilesHubContext,
            IActionsNotifier actionsNotifier,
            IMapper mapper,
            BuyService buyService,
            SellService sellService)
        {
            _movePlayersService = movePlayersService;
            _profilesService = profilesService;
            _buyActionRepository = buyActionRepository;
            _sellActionRepository = sellActionRepository;
            _moveActionRepository = moveActionRepository;
            _profilesHubContext = profilesHubContext;
            _actionsNotifier = actionsNotifier;
            _mapper = mapper;
            _buyService = buyService;
            _sellService = sellService;
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
                await _buyService.Buy(profileDTO, buyCardDTO, tknSrc, null);
            });

            var tradeAction = new BuyAction(profileDTO.Id, buyAction, tknSrc, buyCardDTO);

            await tradeAction.Action.Invoke();
            await _buyActionRepository.Add(_mapper.Map<BuyActionEntity>(tradeAction));
            await _actionsNotifier.AddAction(profileDTO, tradeAction);
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
                await _buyService.Buy(profileDTO, buyCardDTO, tknSrc, sellAction);
            });

            var tradeAction = new BuyAction(profileDTO.Id, buyAction, tknSrc, buyCardDTO);
            await tradeAction.Action.Invoke();

            await _buyActionRepository.Add(_mapper.Map<BuyActionEntity>(tradeAction));
            await _actionsNotifier.AddAction(profileDTO, tradeAction);
        }

        public async Task Sell(SellCardDTO sellCardDTO)
        {
            var profileDTO = await _profilesService.GetByEmail(sellCardDTO.Email);
            var tknSrc = new CancellationTokenSource();

            var sellAction = new Func<Task>(async () =>
            {
                await _sellService.SellCard(profileDTO, sellCardDTO, tknSrc);
            });

            await sellAction.Invoke();
            var tradeAction = new SellAction(profileDTO.Id, sellAction, tknSrc, sellCardDTO);
            await _sellActionRepository.Add(_mapper.Map<SellActionEntity>(tradeAction));
            await _actionsNotifier.AddAction(profileDTO, tradeAction);
        }

        public async Task MoveCardsFromTransferTargetsToTransferList(string email)
        {
            var profileDTO = await _profilesService.GetByEmail(email);

            _movePlayersService.SendTransferTargetsToTransferList(profileDTO, _updateProfile);
        }

        public async Task SendUnassignedItemsToTransferList(string email)
        {
            var profileDTO = await _profilesService.GetByEmail(email);

            _movePlayersService.SendUnassignedItemsToTransferList(profileDTO, _updateProfile);
        }

        public async Task RelistPlayers()
        {
            //var profiles = await _profilesService.GetRelistProfiles();
            //Parallel.ForEach(profiles, async (profile) =>
            //{
            //    var tknSrc = new CancellationTokenSource();
            //    var action = _sellSeleniumService.RelistPlayers(profile, tknSrc);
            //    await _moveActionRepository.Add(_mapper.Map<MoveActionEntity>(action));
            //    await _actionsNotifier.AddAction(profile, action);
            //});
        }
    }
}
