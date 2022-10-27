using MongoDB.Bson;

namespace Magnus.Futbot.Database.Models.Interfaces
{
    public interface IActionEntity
    {
        public ObjectId Id { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime UpdatedDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public ObjectId ProfileId { get; set; }
        public int Priority { get; set; }
        public string Description { get; set; } 
        public CancellationTokenSource? CancellationTokenSource { get; set; }
    }
}
