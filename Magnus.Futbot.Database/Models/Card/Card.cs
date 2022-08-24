using Magnus.Futbot.Database.Models.Interfaces;
using MongoDB.Bson;

namespace Magnus.Futbot.Database.Models.Card
{
    public class Card : IEntity
    {
        public string Name { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string FullName { get; internal set; } = string.Empty;
        public string Club { get; internal set; } = string.Empty;
        public string Nation { get; internal set; } = string.Empty;
        public string League { get; internal set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public MainData MainData { get; set; } = new MainData();
        public Stats Stats { get; set; } = new Stats();
        public ObjectId Id { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime UpdatedDate { get; set; } = DateTime.Now;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public ObjectId UserId { get; set; }
    }
}
