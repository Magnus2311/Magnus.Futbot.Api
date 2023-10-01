namespace Magnus.Futtbot.Connections.Models
{
    public class BuyingData
    {
        public int AlreadyBoughtCount { get; set; }

        public int LoginFailedAttempts { get; set; }

        public int PauseForAWhile { get; set; }

        public HashSet<long> AlreadyBiddedTrades { get; } = new();

        public int MinBin { get; set; }
    }
}
