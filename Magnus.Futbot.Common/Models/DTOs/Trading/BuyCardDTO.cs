using Magnus.Futbot.Common.Models.Database.Card;

namespace Magnus.Futbot.Common.Models.DTOs.Trading
{
    public class BuyCardDTO
    {
        public Card Card { get; set; }

        public string Quality { get; set; } = string.Empty;

        public string Rarity { get; set; } = string.Empty;

        public string Position { get; set; } = string.Empty;

        public string Chemistry { get; set; } = string.Empty;

        public string Nationallity { get; set; } = string.Empty;

        public string League { get; set; } = string.Empty;

        public string Club { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public bool IsBin { get; set; }

        public int Count { get; set; }

        public int Price { get; set; }

        public int MaxMinBuyPrice { get; set; }
    }
}
