namespace Magnus.Futbot.Api.Models.Requests
{
    public class AddSuccessfulPurchaseRequest
    {
        public string PidId { get; set; } = string.Empty;
        public long TradeId { get; set; }
        public long ItemId { get; set; }
        public int PurchasePrice { get; set; }
    }
}
