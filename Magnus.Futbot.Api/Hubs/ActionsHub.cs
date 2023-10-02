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

        public ActionsHub(
            IActionsService actionsService,
            ActionsDeactivator actionDeactivator)
        {
            _actionsService = actionsService;
            _actionDeactivator = actionDeactivator;
        }

        public Task<TradeActionsDTO> GetAllActionsByProfileId(string profileId)
            => _actionsService.GetPendingActionsByProfileId(profileId);

        public async Task CancelActionById(string profileId, string actionId, TradeActionType tradeActionType)
        {
            var userId = Context.UserIdentifier ?? "";
            var actionIdStr = await _actionsService.DeleteActionById(actionId, tradeActionType, userId);
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
