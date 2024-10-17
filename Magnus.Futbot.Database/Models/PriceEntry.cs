using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Magnus.Futbot.Database.Models
{
    [BsonIgnoreExtraElements]
    public class PriceEntry
    { 
        public ObjectId Id { get; set; }

        public int Prize { get; set; }

        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
