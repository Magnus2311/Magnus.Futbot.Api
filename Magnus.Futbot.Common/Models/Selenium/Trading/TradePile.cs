using Magnus.Futbot.Common.Models.Database.Card;

namespace Magnus.Futbot.Common.Models.Selenium.Trading
{
    public class TradePile
    {
        public TransferList TransferList { get; set; } = new();
        public List<TransferCard> UnassignedItems { get; set; } = new List<TransferCard>();
        public List<TransferCard> TransferTargets { get; set; } = new List<TransferCard>();
        public List<TransferCard> ClubItems { get; set; } = new List<TransferCard>();
    }

    public class TransferCard
    {
        public Card? Card { get; set; }
        public int Count { get; set; }
    }

    public class TransferList
    {
        public List<TransferCard> SoldItems { get; set; } = new List<TransferCard>();
        public List<TransferCard> UnsoldItems { get; set; } = new List<TransferCard>();
        public List<TransferCard> AvailableItems { get; set; } = new List<TransferCard>();
        public List<TransferCard> ActiveTransfers { get; set; } = new List<TransferCard>();
    }
}
