using Newtonsoft.Json;

namespace Magnus.Futbot.Initializer.Models.Players
{
    public class EaPlayerItem
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("overallRating")]
        public int OverallRating { get; set; }

        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("commonName")]
        public string CommonName { get; set; }
    }
}


