namespace Magnus.Futbot.Api.Models.Requests
{
    public class AddPlayerPricesRequest
    {
        public string CardId { get; set; }

        public List<int> Prices { get; set; } = [];
    }
}
