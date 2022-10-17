using Magnus.Futbot.Common.Models.DTOs.Trading.Actions;
using Magnus.Futbot.Common.Models.Selenium.Actions;

namespace Magnus.Futbot.Api.Hubs.Interfaces
{
    public interface IActionsClient
    {
        Task OnActionAdded(TradeActionDTO actiom);
        Task OnActionCanceled(string actionId);
        Task OnActionsLoaded(TradeActions actions);
    }
}
