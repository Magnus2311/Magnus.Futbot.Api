using Magnus.Futbot.Common.Models.Database.Card;

namespace Magnus.Futbot.Common.Models.DTOs.Trading
{
    public class BuyCardDTO
    {
        public Card Card { get; set; } = new();
        public string Email { get; set; } = string.Empty;
        public bool IsBin { get; set; }
        public int Count { get; set; }
        public int Price { get; set; }
    }
}
