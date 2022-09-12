using Magnus.Futbot.Common.Models.Database.Interfaces;
using MongoDB.Bson;

namespace Magnus.Futbot.Common.Models.Database.Card
{
    public class Card : IEntity
    {
        public string Name { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string FullName { get; internal set; } = string.Empty;
        public string Club { get; internal set; } = string.Empty;
        public string Nation { get; internal set; } = string.Empty;
        public string League { get; internal set; } = string.Empty;
        public string PlayerImage { get; set; } = string.Empty;
        public string BackgroundImage { get; set; } = string.Empty;
        public string Revision { get; set; } = string.Empty;
        public PromoType PromoType { get; set; }
        public MainData MainData { get; set; } = new MainData();
        public Stats Stats { get; set; } = new Stats();
        public ObjectId Id { get; set; }
        public string CardId
        {
            get
            {
                return Id.ToString();
            }
            set
            {
                Id = new ObjectId(value);
            }
        }
        public bool IsDeleted { get; set; }
        public DateTime UpdatedDate { get; set; } = DateTime.Now;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public ObjectId UserId { get; set; }
        public int AssetId { get; internal set; }
        public int ClubId { get; internal set; }
        public int LeagueId { get; internal set; }
    }
}
