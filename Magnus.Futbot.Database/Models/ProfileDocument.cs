using Magnus.Futbot.Common;
using Magnus.Futbot.Database.Models.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Magnus.Futbot.Database.Models
{
    [BsonIgnoreExtraElements]
    public class ProfileDocument : IEntity
    {
        public ObjectId Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public ObjectId UserId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime UpdatedDate { get; set; } = DateTime.Now;
        public ProfileStatusType ProfilesStatus { get; set; }
        public int Coins { get; set; }
        public int ActiveBidsCount { get; set; }
        public int WonTargetsCount { get; set; }
        public int TransferListCount { get; set; }
        public int UnassignedCount { get; set; }
        public int Outbidded { get; set; }
    }
}
