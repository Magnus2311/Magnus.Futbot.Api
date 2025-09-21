using Magnus.Futbot.Common.Models.Database.Card;
using MongoDB.Bson;

namespace Magnus.Futbot.Api.Models.Responses
{
    public class SuccessfulPurchaseResponse
    {
        public ObjectId Id { get; set; }
        public string PidId { get; set; } = string.Empty;
        public string TradeId { get; set; } = string.Empty;
        public string ItemId { get; set; } = string.Empty;
        public Card Card { get; set; } = new();
        public int PurchasePrice { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
    }
}
