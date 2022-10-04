using Magnus.Futbot.Common.Models.DTOs;
using Magnus.Futbot.Common.Models.Selenium.Actions;

namespace Magnus.Futbot.Common.Interfaces.Notifiers
{
    public interface IActionsNotifier
    {
        Task AddAction(ProfileDTO profileDTO, TradeAction tradeAction);
    }
}
