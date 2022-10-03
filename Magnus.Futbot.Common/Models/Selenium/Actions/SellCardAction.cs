using Magnus.Futbot.Common.Models.DTOs.Trading;

namespace Magnus.Futbot.Common.Models.Selenium.Actions
{
    public class SellCardAction : TradeAction
    {
        public SellCardAction(
            Func<Task> action,
            CancellationTokenSource cancellationTokenSource,
            SellCardDTO? sellCardDTO) : base($"Sell -> {sellCardDTO?.Card?.Name}", $"From Bid: {sellCardDTO?.FromBid} - From bin: {sellCardDTO?.FromBin}", action, cancellationTokenSource)
        {
            SellCardDTO = sellCardDTO;
        }

        public SellCardDTO? SellCardDTO { get; }
    }
}
