using Magnus.Futbot.Common.Models.Database.Interfaces;
using MongoDB.Bson;

namespace Magnus.Futbot.Database.Models
{
    public class Trade : IEntity
    {
        public ObjectId Id { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime UpdatedDate { get; set; } = DateTime.Now;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public ObjectId UserId { get; set; }
    }
}
