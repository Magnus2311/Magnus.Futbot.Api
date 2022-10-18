using Magnus.Futbot.Common.Models.DTOs.Trading;
using MongoDB.Bson;

namespace Magnus.Futbot.Database.Models.Actions
{
    public class BuyActionEntity
    {
        public ObjectId Id {get; set; } = ObjectId.Empty;
        public bool IsDeleted { get; set; }
        public DateTime UpdatedDate { get; set; } = DateTime.Now;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public ObjectId ProfileId { get; set; }
        public BuyCardDTO BuyCardDTO { get; set; } = new();
        public int Priority { get; set; }
        public string Description { get; set; } = string.Empty;
        public CancellationTokenSource? CancellationTokenSource { get; set; }
    }
}
