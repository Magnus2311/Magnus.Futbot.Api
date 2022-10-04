using Magnus.Futbot.Common.Models.DTOs.Trading.Actions;

namespace Magnus.Futbot.Api.Hubs.Interfaces
{
    public interface IActionsClient
    {
        Task OnActionAdded(TradeActionDTO actiom);
        Task OnActionCanceled(string actionId);
        Task OnActionsLoaded(IEnumerable<TradeActionDTO> actions);
    }
}
