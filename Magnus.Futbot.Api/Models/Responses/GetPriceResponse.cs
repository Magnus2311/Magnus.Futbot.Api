namespace Magnus.Futbot.Api.Models.Responses
{
    public class GetPriceResponse
    {
        public List<int> Prices { get; set; } = [];

        public DateTime LastUpdated { get; set; }
    }
}
