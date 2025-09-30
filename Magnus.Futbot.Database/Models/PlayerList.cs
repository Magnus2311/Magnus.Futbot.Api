using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Magnus.Futbot.Database.Models
{
    public class PlayerList
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;
        public string PidId { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime UpdatedDate { get; set; } = DateTime.Now;
        public List<string> PlayerIds { get; set; } = new List<string>();
    }
}
