using Magnus.Futbot.Common.Models.Database.Card;

namespace Magnus.Futbot.Api.Models.Requests
{
    public class AddSuccessfulPurchaseRequest
    {
        public string PidId { get; set; } = string.Empty;
        public string TradeId { get; set; } = string.Empty;
        public string ItemId { get; set; } = string.Empty;
        public Card Card { get; set; } = new();
        public int PurchasePrice { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
