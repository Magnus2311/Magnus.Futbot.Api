using MongoDB.Bson;

namespace Magnus.Futbot.Common.Models.DTOs.Trading
{
    public class TradeDTO
    {
        public BuyCardDTO BuyCardDTO { get; set; }

        public SellCardDTO SellCardDTO { get; set; }

        public BuyAndSellCardDTO BuyAndSellCardDTO { get; set; }

        public TradeActionType TradeType { get; set; }

        public ObjectId ProfileId { get; set; }

        public ObjectId Id { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime UpdatedDate { get; set; } = DateTime.Now;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public ObjectId UserId { get; set; }
    }
}
