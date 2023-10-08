using Magnus.Futbot.Common.Models.DTOs;
using Magnus.Futbot.Common.Models.DTOs.Trading;

namespace Magnus.Futbot.Api.Services.Interfaces
{
    public interface ITradingService
    {
        Task Buy(BuyCardDTO buyCardDTO);
        Task Sell(SellCardDTO sellCardDTO);
        Task MoveCardsFromTransferTargetsToTransferList(string email);
        Task SendUnassignedItemsToTransferList(string email);
        Task BuyAndSell(BuyAndSellCardDTO buyAndSellCardDTO);
        Task RelistPlayers();
        Task RelistPlayersByProfile(ProfileDTO profileDTO);
    }
}
