using Magnus.Futbot.Api.Hubs.Interfaces;
using Magnus.Futbot.Common;
using Magnus.Futbot.Common.Interfaces.Services;
using Magnus.Futbot.Common.Models.DTOs.Trading.Actions;
using Microsoft.AspNetCore.SignalR;

namespace Magnus.Futbot.Api.Hubs
{
    public class ActionsHub : Hub<IActionsClient>
    {
        private readonly IActionsService _actionsService;

        public ActionsHub(IActionsService actionsService)
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
