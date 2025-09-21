using Magnus.Futbot.Common.Models.Database.Interfaces;
using Magnus.Futbot.Common.Models.Database.Card;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Magnus.Futbot.Database.Models
{
    [BsonIgnoreExtraElements]
    public class SuccessfulPurchase : IEntity
    {
        public ObjectId Id { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime UpdatedDate { get; set; } = DateTime.Now;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public ObjectId UserId { get; set; }

        public string PidId { get; set; } = string.Empty;
        public string TradeId { get; set; } = string.Empty;
        public string ItemId { get; set; } = string.Empty;
        public Card Card { get; set; } = new();
        public int PurchasePrice { get; set; }
        public DateTime PurchaseDate { get; set; } = DateTime.Now;
        public string Description { get; set; } = string.Empty;
    }
}
