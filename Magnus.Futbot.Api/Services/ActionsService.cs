using AutoMapper;
using Magnus.Futbot.Common;
using Magnus.Futbot.Common.Models.DTOs.Trading.Actions;
using Magnus.Futbot.Common.Models.Selenium.Actions;
using Magnus.Futbot.Database.Repositories.Actions;
using Magnus.Futbot.Services;
using MongoDB.Bson;

namespace Magnus.Futbot.Api.Services
{
    public class ActionsService
    {
        private readonly ProfilesService _profilesService;
        private readonly BuyActionRepository _buyActionRepository;
        private readonly SellActionRepository _sellActionRepository;
        private readonly MoveActionRepository _moveActionRepository;
        private readonly IMapper _mapper;

        public ActionsService(ProfilesService profilesService,
            BuyActionRepository buyActionRepository,
            SellActionRepository sellActionRepository,
            MoveActionRepository moveActionRepository,
            IMapper mapper)
        {
            _profilesService = profilesService;
            _buyActionRepository = buyActionRepository;
            _sellActionRepository = sellActionRepository;
            _moveActionRepository = moveActionRepository;
            _mapper = mapper;
        }

        public async Task<TradeActions> GetPendingActionsByProfileId(string profileId)
        {
            var actions = new TradeActions
            {
                BuyActions = _mapper.Map<IEnumerable<BuyAction>>(await _buyActionRepository.GetActionsByProfileId(new ObjectId(profileId))),
                SellActions = _mapper.Map<IEnumerable<SellAction>>(await _sellActionRepository.GetActionsByProfileId(new ObjectId(profileId))),
                MoveActions = _mapper.Map<IEnumerable<MoveAction>>(await _moveActionRepository.GetActionsByProfileId(new ObjectId(profileId)))
            };
            return actions;
        }

        public async Task CancelActionById(string actionId, TradeActionType actionType, string userId)
        {
            TradeAction action;
            switch (actionType)
            {
                case TradeActionType.Buy:
                    var buyActionEntity = await _buyActionRepository.GetById(new ObjectId(actionId));
                    buyActionEntity.IsDeleted = true;
                    await _buyActionRepository.Update(buyActionEntity);

                    action = _mapper.Map<BuyAction>(buyActionEntity);
                    action?.CancellationTokenSource.Cancel();
                    break;
                case TradeActionType.Sell:
                    var sellActionEntity = await _sellActionRepository.GetById(new ObjectId(actionId));
                    sellActionEntity.IsDeleted = true;
                    await _sellActionRepository.Update(sellActionEntity);

                    action = _mapper.Map<SellAction>(sellActionEntity);
                    action?.CancellationTokenSource.Cancel();
                    break;
                case TradeActionType.Move:
                    var moveActionEntity = await _moveActionRepository.GetById(new ObjectId(actionId));
                    moveActionEntity.IsDeleted = true;
                    await _moveActionRepository.Update(moveActionEntity);

                    action = _mapper.Map<MoveAction>(moveActionEntity);
                    action?.CancellationTokenSource.Cancel();
                    break;
            }
        }
    }
}
