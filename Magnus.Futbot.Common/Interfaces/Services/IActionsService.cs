using Magnus.Futbot.Common.Models.DTOs.Trading.Actions;

namespace Magnus.Futbot.Common.Interfaces.Services
{
    public interface IActionsService
    {
        Task<TradeActionsDTO> GetPendingActionsByProfileId(string profileId);
        Task<string> DeleteActionById(string actionId, TradeActionType actionType, string userId);
        Task DeactivateAllActionsOnStartUp();
        Task DeactivateAction(string actionId, TradeActionType actionType);
        Task<TradeActionDTO> PauseProfile(string email, string selectedDuration, string userId);
        Task DeactivateAllActionsByProfileId(string profileId);
    }
}
