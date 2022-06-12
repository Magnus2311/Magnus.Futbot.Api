using Magnus.Futbot.Database.Models.Interfaces;
using MongoDB.Bson;

namespace Magnus.Futbot.Database.Models
{
    public class ProfileDocument : IEntity
    {
        public ObjectId Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public ObjectId UserId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime UpdatedDate { get; set; } = DateTime.Now;
    }
}
