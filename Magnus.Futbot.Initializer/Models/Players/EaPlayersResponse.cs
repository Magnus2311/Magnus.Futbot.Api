using Magnus.Futbot.Common.Models.Database.Card;
using Newtonsoft.Json;

namespace Magnus.Futbot.Initializer.Models.Players
{
    public class EaPlayersResponse
    {
        [JsonProperty("items")]
        public List<Card> Items { get; set; } = [];

        [JsonProperty("totalItems")]
        public int TotalItems { get; set; }
    }
}


