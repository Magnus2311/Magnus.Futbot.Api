namespace Magnus.Futbot.Common.Models.DTOs.Trading.Actions
{
    public class TradeActionsDTO
    {
        public List<BuyActionDTO> BuyActions { get; set; } = new();
        public List<SellActionDTO> SellActions { get; set; } = new();
        public List<MoveActionDTO> MoveActions { get; set; } = new();
    }
}
