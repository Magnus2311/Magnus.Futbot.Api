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
        public long TradeId { get; set; }
        public long ItemId { get; set; }
        public int PurchasePrice { get; set; }
        public DateTime PurchaseDate { get; set; } = DateTime.Now;

        // Filter information for filtered purchases
        public bool IsFilteredPurchase { get; set; } = false;
        public string FilterDescription { get; set; } = string.Empty;
        public string PlayerName { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public string Quality { get; set; } = string.Empty;
        public string Rarity { get; set; } = string.Empty;
        public string League { get; set; } = string.Empty;
        public string Club { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string ChemistryStyle { get; set; } = string.Empty;
        public string PlayStyles { get; set; } = string.Empty;
        public int? MinOvr { get; set; }
        public int? MaxOvr { get; set; }
        public int? MinBidPrice { get; set; }
        public int? MaxBidPrice { get; set; }
        public int? MinBuyNowPrice { get; set; }
        public int? MaxBuyNowPrice { get; set; }
    }
}
