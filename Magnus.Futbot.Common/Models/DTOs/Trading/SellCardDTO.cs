using Magnus.Futbot.Common.Models.Database.Card;

namespace Magnus.Futbot.Common.Models.DTOs.Trading
{
    public class SellCardDTO
    {
        public Card Card { get; set; } = new();
        public string Email { get; set; } = string.Empty;
        public int Count { get; set; }
        public int FromBid { get; set; }
        public int? ToBid { get; set; }
        public int FromBin { get; set; }
        public int? ToBin { get; set; }
    }
}
