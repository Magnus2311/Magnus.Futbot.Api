namespace Magnus.Futbot.Api.Models.Requests
{
    public class AddBuyTradeRequest
    {
        public string CardId { get; set; }

        public string Quality { get; set; }

        public string Rarity { get; set; }

        public string Position { get; set; }

        public string Chemistry { get; set; }

        public string Nationallity { get; set; }

        public string League { get; set; }

        public string Club { get; set; }

        public bool IsBin { get; set; }

        public int Count { get; set; }

        public int Price { get; set; }

        public int MaxMinBuyPrice { get; set; }

        public int SellPrice { get; set; }
    }
}
