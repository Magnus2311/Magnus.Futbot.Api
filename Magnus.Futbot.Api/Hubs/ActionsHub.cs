using Magnus.Futbot.Api.Hubs.Interfaces;
using Magnus.Futbot.Api.Services;
using Magnus.Futbot.Common;
using Magnus.Futbot.Common.Interfaces.Services;
using Magnus.Futbot.Common.Models.DTOs.Trading.Actions;
using Microsoft.AspNetCore.SignalR;

namespace Magnus.Futbot.Api.Hubs
{
    public class ActionsHub : Hub<IActionsClient>
    {
        private readonly IActionsService _actionsService;
        private readonly ActionsDeactivator _actionDeactivator;

        public ActionsHub(IActionsService actionsService,
            ActionsDeactivator actionDeactivator)
        {
            _actionsService = actionsService;
            _actionDeactivator = actionDeactivator;
        }

        public async Task CancelAllActionsById(string profileId)
        {
            var userId = Context.UserIdentifier ?? "";

            var allActions = await _actionsService.GetPendingActionsByProfileId(profileId);
            await _actionsService.DeactivateAllActionsByProfileId(profileId);
            await _actionDeactivator.DeactivateAllAction(profileId);

            allActions.BuyActions.ForEach(async ba => await Clients.Users(userId).OnActionCanceled(ba.Id));
            allActions.SellActions.ForEach(async ba => await Clients.Users(userId).OnActionCanceled(ba.Id));
            allActions.MoveActions.ForEach(async ba => await Clients.Users(userId).OnActionCanceled(ba.Id));
        }

        public Task<TradeActionsDTO> GetAllActionsByProfileId(string profileId)
            => _actionsService.GetPendingActionsByProfileId(profileId);

        public async Task CancelActionById(string profileId, string actionId, TradeActionType tradeActionType)
        {
            var userId = Context.UserIdentifier ?? "";
            await _actionsService.DeleteActionById(actionId, tradeActionType, userId);
            await _actionDeactivator.DeactivateAction(profileId, actionId);
            await Clients.Users(userId).OnActionCanceled(actionId);
        }

        public async Task PauseAction(string email, string selectedDuration)
        {
            var userId = Context.UserIdentifier ?? "";
            var action = await _actionsService.PauseProfile(email, selectedDuration, userId); ;
            await Clients.Users(userId).OnActionAdded(action);
        }
    }
}
