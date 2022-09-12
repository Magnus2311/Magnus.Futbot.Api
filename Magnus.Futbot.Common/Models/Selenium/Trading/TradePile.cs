using Magnus.Futbot.Common.Models.Database.Card;

namespace Magnus.Futbot.Common.Models.Selenium.Trading
{
    public class TradePile
    {
        public List<TransferCard> TransferList { get; set; } = new List<TransferCard>();
        public List<TransferCard> UnassignedItems { get; set; } = new List<TransferCard>();
        public List<TransferCard> TransferTargets { get; set; } = new List<TransferCard>();
        public List<TransferCard> ClubItems { get; set; } = new List<TransferCard>();
    }

    public class TransferCard
    {
        public Card Card { get; set; } = new();
        public PlayerCardStatus PlayerCardStatus { get; set; }
        public int BougthFor { get; set; }
    }
}
