using Magnus.Futbot.Api.Hubs.Interfaces;
using Magnus.Futbot.Api.Services;
using Magnus.Futbot.Common;
using Magnus.Futbot.Common.Models.DTOs.Trading.Actions;
using Magnus.Futbot.Common.Models.Selenium.Actions;
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

        public Task<TradeActionsDTO> GetAllActionsByProfileId(string profileId)
            => _actionsService.GetPendingActionsByProfileId(profileId);

        public async Task CancelActionById(string actionId, TradeActionType tradeActionType)
        {
            var userId = Context.UserIdentifier ?? "";
            await _actionsService.CancelActionById(actionId, tradeActionType, userId);;
            await Clients.Users(userId).OnActionCanceled(actionId);
        }
    }
}
