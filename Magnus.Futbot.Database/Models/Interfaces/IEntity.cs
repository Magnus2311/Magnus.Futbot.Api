using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Magnus.Futbot.Database.Models.Interfaces
{
    public interface IEntity
    {
        public ObjectId Id { get; set; }
        [BsonDefaultValue(false)]
        public bool IsDeleted { get; set; }
        public DateTime UpdatedDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public ObjectId UserId { get; set; }
    }
}
