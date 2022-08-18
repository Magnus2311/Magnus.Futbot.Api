namespace Magnus.Futbot.Common.Models.Selenium.Trading
{
    public class AuctionInfo
    {
        public int TradeId { get; set; }
        public ItemData ItemData { get; set; } = new ItemData();
        public int BuyNowPrice { get; set; }
        public int CurrentBid { get; set; }
        public int Offers { get; set; }
        public bool Watched { get; set; }
        public int? BidState { get; set; }
        public int StartingBid { get; set; }
        public int ConfidenceValue { get; set; }
        public int Expires { get; set; }
        public int SellerEstablished { get; set; }
        public int SellerId { get; set; }
        public string TradeIdStr { get; set; } = string.Empty;
    }

    public class BidTokens
    {
    }

    public class DuplicateItemIdList
    {
        public long ItemId { get; set; }
        public long DuplicateItemId { get; set; }
    }

    public class ItemData
    {
        public long Id { get; set; }
        public int Timestamp { get; set; }
        public string Formation { get; set; } = string.Empty;
        public bool Untradeable { get; set; }
        public int AssetId { get; set; }
        public int Rating { get; set; }
        public string ItemType { get; set; } = string.Empty;
        public int ResourceId { get; set; }
        public int Owners { get; set; }
        public int DiscardValue { get; set; }
        public string ItemState { get; set; } = string.Empty;
        public int Cardsubtypeid { get; set; }
        public int LastSalePrice { get; set; }
        public string InjuryType { get; set; } = string.Empty;
        public int InjuryGames { get; set; }
        public string PreferredPosition { get; set; } = string.Empty;
        public int Contract { get; set; }
        public int Teamid { get; set; }
        public int Rareflag { get; set; }
        public int PlayStyle { get; set; }
        public int LeagueId { get; set; }
        public int Assists { get; set; }
        public int LifetimeAssists { get; set; }
        public int LoyaltyBonus { get; set; }
        public int Pile { get; set; }
        public int Nation { get; set; }
        public int MarketDataMinPrice { get; set; }
        public int MarketDataMaxPrice { get; set; }
        public int ResourceGameYear { get; set; }
        public string GuidAssetId { get; set; } = string.Empty;
        public List<int> Groups { get; set; } = new List<int>();
        public List<int> AttributeArray { get; set; } = new List<int>();
        public List<int> StatsArray { get; set; } = new List<int>();
        public List<int> LifetimeStatsArray { get; set; } = new List<int>();
        public int Skillmoves { get; set; }
        public int Weakfootabilitytypecode { get; set; }
        public int Attackingworkrate { get; set; }
        public int Defensiveworkrate { get; set; }
        public int Preferredfoot { get; set; }
        public int? rankId { get; set; }
    }

    public class TradePile
    {
        public int Credits { get; set; }
        public List<AuctionInfo> AuctionInfo { get; set; } = new List<AuctionInfo>();
        public List<DuplicateItemIdList> DuplicateItemIdList { get; set; } = new List<DuplicateItemIdList>();
        public BidTokens BidTokens { get; set; } = new BidTokens();
    }
}
