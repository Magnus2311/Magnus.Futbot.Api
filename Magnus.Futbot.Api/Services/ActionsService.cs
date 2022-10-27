using AutoMapper;
using Magnus.Futbot.Common;
using Magnus.Futbot.Common.Interfaces.Services;
using Magnus.Futbot.Common.Models.DTOs.Trading.Actions;
using Magnus.Futbot.Common.Models.Selenium.Actions;
using Magnus.Futbot.Database.Repositories.Actions;
using MongoDB.Bson;

namespace Magnus.Futbot.Api.Services
{
    public class ActionsService : IActionsService
    {
        private readonly BuyActionRepository _buyActionRepository;
        private readonly SellActionRepository _sellActionRepository;
        private readonly MoveActionRepository _moveActionRepository;
        private readonly IMapper _mapper;

        public ActionsService(BuyActionRepository buyActionRepository,
            SellActionRepository sellActionRepository,
            MoveActionRepository moveActionRepository,
            IMapper mapper)
        {
            _buyActionRepository = buyActionRepository;
            _sellActionRepository = sellActionRepository;
            _moveActionRepository = moveActionRepository;
            _mapper = mapper;
        }

        public async Task<TradeActionsDTO> GetPendingActionsByProfileId(string profileId)
        {
            var actions = new TradeActions
            {
                BuyActions = _mapper.Map<IEnumerable<BuyAction>>(await _buyActionRepository.GetActionsByProfileId(new ObjectId(profileId))),
                SellActions = _mapper.Map<IEnumerable<SellAction>>(await _sellActionRepository.GetActionsByProfileId(new ObjectId(profileId))),
                MoveActions = _mapper.Map<IEnumerable<MoveAction>>(await _moveActionRepository.GetActionsByProfileId(new ObjectId(profileId)))
            };
            return _mapper.Map<TradeActionsDTO>(actions);
        }

        public async Task DeleteActionById(string actionId, TradeActionType actionType, string userId)
        {
            switch (actionType)
            {
                case TradeActionType.Buy:
                    var buyActionEntity = await _buyActionRepository.GetById(new ObjectId(actionId));
                    buyActionEntity.IsDeleted = true;
                    await _buyActionRepository.Update(buyActionEntity);
                    break;
                case TradeActionType.Sell:
                    var sellActionEntity = await _sellActionRepository.GetById(new ObjectId(actionId));
                    sellActionEntity.IsDeleted = true;
                    await _sellActionRepository.Update(sellActionEntity);
                    break;
                case TradeActionType.Move:
                    var moveActionEntity = await _moveActionRepository.GetById(new ObjectId(actionId));
                    moveActionEntity.IsDeleted = true;
                    await _moveActionRepository.Update(moveActionEntity);
                    break;
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
    }
}
