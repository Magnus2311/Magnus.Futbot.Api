namespace Magnus.Futbot.Common.Models.Selenium.Trading
{
    public class PlayerCard
    {
        public string Name { get; set; } = string.Empty;
        public int Rating { get; set; }
        public PlayerCardStatus PlayerCardStatus {get; set;}
        public PlayerCardType PlayerType { get; set; }
    }

    public class TradePile
    {
        public int Credits { get; set; }
        public List<PlayerCard> TransferList { get; set; } = new List<PlayerCard>();
        public List<PlayerCard> UnassignedItems { get; set; } = new List<PlayerCard>();
        public List<PlayerCard> TransferTargets { get; set; } = new List<PlayerCard>();
        public List<PlayerCard> ClubItems { get; set; } = new List<PlayerCard>();
    }
}
