using Magnus.Futbot.Common.fcmodels;

namespace Magnus.Futtbot.Connections.Models
{
    public class AvailableTransferMarketCards
    {
        public List<Auctioninfo> auctionInfo { get; set; } = new();
        
        public Bidtokens bidTokens { get; set; }
    }
}
