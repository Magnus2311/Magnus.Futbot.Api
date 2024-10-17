using MongoDB.Bson.Serialization.Attributes;

namespace Magnus.Futbot.Database.Models
{
    [BsonIgnoreExtraElements]
    public class PlayerPrice
    {
        public string CardId { get; set; }

        public List<PriceEntry> Prices { get; set; } = [];
    }
}
