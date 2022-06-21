using Magnus.Futbot.Common;
using MongoDB.Bson;

namespace Magnus.Futbot.Api.Models.DTOs
{
    public class ProfileDTO
    {
        public string Email { get; set; } = string.Empty;
        public ProfileStatusType Status { get; set; }
        public int Coins { get; set; }
        public int ActiveBidsCount { get; set; }
        public int WonTargetsCount { get; set; }
        public int TransferListCount { get; set; }
        public int UnassignedCount { get; set; }
        public int Outbidded { get; set; }
        public ObjectId UserId { get; set; }

        public override int GetHashCode()
            => HashCode.Combine(Email);
    }
}
