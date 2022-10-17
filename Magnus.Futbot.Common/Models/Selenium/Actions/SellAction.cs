using Magnus.Futbot.Common.Models.DTOs.Trading;

namespace Magnus.Futbot.Common.Models.Selenium.Actions
{
    public class SellAction : TradeAction
    {
        public SellAction(
            string profileId,
            Func<Task> action,
            CancellationTokenSource cancellationTokenSource,
            SellCardDTO? sellCardDTO) : base(TradeActionType.Sell, $"{sellCardDTO?.Card?.Name} From Bid: {sellCardDTO?.FromBid} - From bin: {sellCardDTO?.FromBin}", profileId, action, cancellationTokenSource)
        {
            SellCardDTO = sellCardDTO;
            Priority = 2;
        }

        public SellCardDTO? SellCardDTO { get; }
    }
}
