using Magnus.Futbot.Api.Hubs.Interfaces;
using Magnus.Futbot.Api.Services;
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
            await Clients.All.OnActionsLoaded(actions);
        }

        public async Task CancelActionById(string profileId, string actionId)
        {
            await _actionsService.CancelActionById(profileId, actionId);
            var userId = Context.UserIdentifier ?? "";
            await Clients.Users(userId).OnActionCanceled(actionId);
        }
    }
}
