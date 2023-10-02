using AutoMapper;
using Magnus.Futbot.Common;
using Magnus.Futbot.Common.Interfaces.Notifiers;
using Magnus.Futbot.Common.Interfaces.Services;
using Magnus.Futbot.Common.Models.DTOs;
using Magnus.Futbot.Common.Models.DTOs.Trading.Actions;
using Magnus.Futbot.Common.Models.Selenium.Actions;
using Magnus.Futbot.Database.Models.Actions;
using Magnus.Futbot.Database.Repositories.Actions;
using MongoDB.Bson;

namespace Magnus.Futbot.Api.Services
{
    public class ActionsService : IActionsService
    {
        private readonly BuyActionRepository _buyActionRepository;
        private readonly SellActionRepository _sellActionRepository;
        private readonly MoveActionRepository _moveActionRepository;
        private readonly PauseActionRepository _pauseActionRepository;
        private readonly IActionsNotifier _actionsNotifier;
        private readonly IMapper _mapper;

        public ActionsService(BuyActionRepository buyActionRepository,
            SellActionRepository sellActionRepository,
            MoveActionRepository moveActionRepository,
            PauseActionRepository pauseActionRepository,
            IActionsNotifier actionsNotifier,
            IMapper mapper)
        {
            _buyActionRepository = buyActionRepository;
            _sellActionRepository = sellActionRepository;
            _moveActionRepository = moveActionRepository;
            _pauseActionRepository = pauseActionRepository;
            _actionsNotifier = actionsNotifier;
            _mapper = mapper;
        }

        public async Task<TradeActionsDTO> GetPendingActionsByProfileId(string profileId)
        {
            var actions = new TradeActions
            {
                BuyActions = _mapper.Map<IEnumerable<BuyAction>>(await _buyActionRepository.GetActionsByProfileId(new ObjectId(profileId))),
                SellActions = _mapper.Map<IEnumerable<SellAction>>(await _sellActionRepository.GetActionsByProfileId(new ObjectId(profileId))),
                MoveActions = _mapper.Map<IEnumerable<MoveAction>>(await _moveActionRepository.GetActionsByProfileId(new ObjectId(profileId))),
                PauseActions = _mapper.Map<IEnumerable<PauseAction>>(await _pauseActionRepository.GetActionsByProfileId(new ObjectId(profileId))),
            };
            return _mapper.Map<TradeActionsDTO>(actions);
        }

        public async Task<string> DeleteActionById(string actionId, TradeActionType actionType, string userId)
        {
            switch (actionType)
            {
                case TradeActionType.Buy:
                    var buyActionEntity = await _buyActionRepository.GetById(new ObjectId(actionId));
                    buyActionEntity.IsDeleted = true;
                    await _buyActionRepository.Update(buyActionEntity);
                    return buyActionEntity.Id.ToString();
                case TradeActionType.Sell:
                    var sellActionEntity = await _sellActionRepository.GetById(new ObjectId(actionId));
                    sellActionEntity.IsDeleted = true;
                    await _sellActionRepository.Update(sellActionEntity);
                    return sellActionEntity.Id.ToString();
                case TradeActionType.Move:
                    var moveActionEntity = await _moveActionRepository.GetById(new ObjectId(actionId));
                    moveActionEntity.IsDeleted = true;
                    await _moveActionRepository.Update(moveActionEntity);
                    return moveActionEntity.Id.ToString();
                default:
                    return "";
            }
        }

        public async Task DeactivateAllActionsOnStartUp()
        {
            await _buyActionRepository.DeactivateAllActions();
            await _sellActionRepository.DeactivateAllActions();
            await _moveActionRepository.DeactivateAllActions();
        }

        public async Task DeactivateAction(string actionId, TradeActionType actionType)
        {
            switch (actionType)
            {
                case TradeActionType.Buy:
                    await _buyActionRepository.DeactivateById(new ObjectId(actionId));
                    break;
                case TradeActionType.Sell:
                    await _sellActionRepository.DeactivateById(new ObjectId(actionId));
                    break;
                case TradeActionType.Move:
                    await _moveActionRepository.DeactivateById(new ObjectId(actionId));
                    break;
            }
        }

        public async Task<TradeActionDTO> PauseProfile(string email, string selectedDuration, string userId)
        {
            var profileDTO = new ProfileDTO();
            if (profileDTO.UserId != userId) return new TradeActionDTO();

            var tknSrc = new CancellationTokenSource();

            TradeAction tradeAction = new MoveAction(profileDTO.Id, new Func<Task>(async () =>
            {

            }), tknSrc, $"Pause for {selectedDuration}");

            await _pauseActionRepository.Add(_mapper.Map<PauseActionEntity>(tradeAction));
            await _actionsNotifier.AddAction(profileDTO, tradeAction);

            return _mapper.Map<TradeActionDTO>(tradeAction);
        }
    }
}
