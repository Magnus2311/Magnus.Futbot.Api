namespace Magnus.Futtbot.Connections.Models
{
    public class TradePileData
    {
        public int credits { get; set; }
        public Auctioninfo[] auctionInfo { get; set; }
        public Bidtokens bidTokens { get; set; }
    }

    public class Auctioninfo
    {
        public long tradeId { get; set; }
        public Itemdata itemData { get; set; }
        public string tradeState { get; set; }
        public int buyNowPrice { get; set; }
        public int currentBid { get; set; }
        public int offers { get; set; }
        public bool watched { get; set; }
        public string bidState { get; set; }
        public int startingBid { get; set; }
        public int confidenceValue { get; set; }
        public int expires { get; set; }
        public string sellerName { get; set; }
        public int sellerEstablished { get; set; }
        public int sellerId { get; set; }
        public bool tradeOwner { get; set; }
        public string tradeIdStr { get; set; }
    }
}
