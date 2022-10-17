using Magnus.Futbot.Common.Models.Database.Interfaces;
using MongoDB.Bson;

namespace Magnus.Futbot.Database.Models.Actions
{
    public class MoveActionEntity
    {
        public ObjectId Id {get; set; } = ObjectId.Empty;
        public bool IsDeleted { get; set; }
        public DateTime UpdatedDate { get; set; } = DateTime.Now;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public ObjectId ProfileId { get; set; }
        public int Priority { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
