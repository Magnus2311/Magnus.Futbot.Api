using Magnus.Futbot.Common.Models.DTOs.Trading;

namespace Magnus.Futbot.Common.Models.Selenium.Actions
{
    public class BuyAction : TradeAction
    {
        public BuyAction(
            Func<Task> action, 
            CancellationTokenSource cancellationTokenSource,
            BuyCardDTO? buyCardDTO) : base($"Buy -> {buyCardDTO?.Card?.Name}", $"{buyCardDTO?.Price} - {buyCardDTO?.Count}", action, cancellationTokenSource)
        {
            BuyCardDTO = buyCardDTO;
        }

        public BuyCardDTO? BuyCardDTO { get; }
    }
}
