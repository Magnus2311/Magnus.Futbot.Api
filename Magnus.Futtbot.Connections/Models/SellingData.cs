namespace Magnus.Futtbot.Connections.Models
{
    public class SellingData
    {
        public HashSet<long> AlreadySoldTrades { get; } = new();
    }
}
