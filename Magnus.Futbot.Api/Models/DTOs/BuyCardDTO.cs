using Magnus.Futbot.Database.Models.Card;

namespace Magnus.Futbot.Api.Models.DTOs
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
