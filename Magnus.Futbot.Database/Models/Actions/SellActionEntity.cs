using Magnus.Futbot.Common.Models.Database.Interfaces;
using Magnus.Futbot.Common.Models.DTOs.Trading;
using MongoDB.Bson;

namespace Magnus.Futbot.Database.Models.Actions
{
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
    }
}
