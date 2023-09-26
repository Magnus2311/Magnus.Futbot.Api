namespace Magnus.Futbot.Common.Models.Selenium.Actions
{
    public class TradeActions
    {
        public IEnumerable<BuyAction> BuyActions { get; set; } = new List<BuyAction>();
        public IEnumerable<SellAction> SellActions { get; set; } = new List<SellAction>();
        public IEnumerable<MoveAction> MoveActions { get; set; } = new List<MoveAction>();
        public IEnumerable<PauseAction> PauseActions { get; set; } = new List<PauseAction>();
    }
}
