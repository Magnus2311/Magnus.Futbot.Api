using Magnus.Futbot.Common.Models.DTOs.Trading;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Magnus.Futbot.Database.Models.Actions
{
    [BsonIgnoreExtraElements]
    public class SellActionEntity
    {
        public ObjectId Id {get; set; } = ObjectId.Empty;
        public bool IsDeleted { get; set; }
        public DateTime UpdatedDate { get; set; } = DateTime.Now;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public ObjectId ProfileId { get; set; }
        public SellCardDTO SellCardDTO { get; set; } = new();
        public int Priority { get; set; }
        public string Description { get; set; } = string.Empty;
        public CancellationTokenSource? CancellationTokenSource { get; set; }
    }
}
