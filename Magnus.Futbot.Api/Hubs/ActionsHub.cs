using Magnus.Futbot.Api.Hubs.Interfaces;
using Magnus.Futbot.Api.Services;
using Magnus.Futbot.Common;
using Microsoft.AspNetCore.SignalR;

namespace Magnus.Futbot.Api.Hubs
{
    public class ActionsHub : Hub<IActionsClient>
    {
        private readonly ActionsService _actionsService;

        public ActionsHub(ActionsService actionsService)
        {
            _actionsService = actionsService;
        }

        public async Task GetAllActionsByProfileId(string profileId)
        {
            var actions = await _actionsService.GetPendingActionsByProfileId(profileId);

            var userId = Context.UserIdentifier ?? "";
            await Clients.Users(userId).OnActionsLoaded(actions);
        }

        public async Task CancelActionById(string actionId, TradeActionType tradeActionType)
        {
            var userId = Context.UserIdentifier ?? "";
            await _actionsService.CancelActionById(actionId, tradeActionType, userId);;
            await Clients.Users(userId).OnActionCanceled(actionId);
        }
    }
}
