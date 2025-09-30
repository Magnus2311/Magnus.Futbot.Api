namespace Magnus.Futbot.Api.Models.Responses
{
    public class PlayerListResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string PidId { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public List<string> PlayerIds { get; set; } = new List<string>();
        public int PlayerCount { get; set; }
    }
}
