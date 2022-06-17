using MongoDB.Bson;

namespace Magnus.Futbot.Api.Models.DTOs
{
    public class ProfileDTO
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public ObjectId UserId { get; set; }
    }
}
