namespace Magnus.Futbot.Api.Models.Requests
{
    public class AddSuccessfulPurchaseRequest
    {
        public string PidId { get; set; } = string.Empty;
        public long TradeId { get; set; }
        public long ItemId { get; set; }
        public int PurchasePrice { get; set; }

        // Filter information for filtered purchases
        public bool IsFilteredPurchase { get; set; } = false;
        public string FilterDescription { get; set; } = string.Empty;
        public string PlayerName { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public string Quality { get; set; } = string.Empty;
        public string Rarity { get; set; } = string.Empty;
        public string League { get; set; } = string.Empty;
        public string Club { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string ChemistryStyle { get; set; } = string.Empty;
        public string PlayStyles { get; set; } = string.Empty;
        public int? MinOvr { get; set; }
        public int? MaxOvr { get; set; }
        public int? MinBidPrice { get; set; }
        public int? MaxBidPrice { get; set; }
        public int? MinBuyNowPrice { get; set; }
        public int? MaxBuyNowPrice { get; set; }
    }
}
